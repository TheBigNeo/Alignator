"""Deterministically generates the .cs / .cs.gold test data files for Alignator-3.

Gold content derives from the alignment spec (shared column = max over entries), never
from formatter output. Run from the repo root:

    python doc/tasks/Alignator-3-Dictionary-Collection-Initializer/generate-test-data.py
"""

from pathlib import Path

DIR = Path("src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments")
IND = "      "  # 2-space indent, 3 levels: class -> method -> initializer entries

ENTRIES = [
    ("Name", "Indominus Rex"),
    ("Diet", "Carnivore"),
    ("Period", "Cretaceous"),
    ("Weight", "8,000 kg"),
    ("Enclosure", "Paddock 11"),
    ("Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"),
]

def q(s: str) -> str:
    return '"' + s + '"'


def key_part(k: str) -> str:
    return '{"' + k + '"'


def prefix(k: str) -> str:
    return '{"' + k + '",'


def plain(e) -> str:
    return prefix(e[0]) + " " + q(e[1])


def after(e) -> str:
    col = len(IND) + max(len(prefix(k)) for k, _ in ENTRIES) + 1
    return prefix(e[0]) + " " * (col - len(IND) - len(prefix(e[0]))) + q(e[1])


def before(e) -> str:
    col = len(IND) + max(len(key_part(k)) for k, _ in ENTRIES)
    return key_part(e[0]) + " " * (col - len(IND) - len(key_part(e[0]))) + ", " + q(e[1])


def block(content_fn, pad_brace: bool) -> str:
    contents = [content_fn(e) for e in ENTRIES]
    col = max(len(IND + c) for c in contents)
    lines = []
    for c in contents:
        line = IND + c
        if pad_brace:
            line += " " * (col - len(line))
            line += "}"
        else:
            line += "}"
        lines.append(line + ",")
    return "\n".join(lines) + "\n"


def wrapper(class_name: str, seealso: list, body: str, is_gold: bool) -> str:
    doc = "\n".join(f"/// <seealso cref=\"AlignatorSettingsKey.{s}\"/>" for s in seealso)
    text = (
        "namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;\n\n"
        + doc
        + "\ninternal class "
        + class_name
        + "\n{\n"
        + body
        + "}"
    )
    # Gold files are compared byte-exact against formatter output: no trailing newline.
    return text if is_gold else text + "\n"


def dino_body(entries: str, method: str = "IndominusRex") -> str:
    return (
        "  static void " + method + "()\n"
        "  {\n"
        "    Dictionary<string, string?> indominusRexFacts = new()\n"
        "    {\n"
        + entries
        + "    };\n"
        "  }\n"
    )


INLINE_BODY = (
    "  static void InlineFacts()\n"
    "  {\n"
    "    Dictionary<string, string?> dinoFacts = new() {{\"Name\", \"Indominus Rex\"}, {\"Diet\", \"Carnivore\"}, {\"Period\", \"Cretaceous\"}};\n"
    "  }\n"
)

KEY_SETTING = "INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER"
BRACE_SETTING = "INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER"

FILES = {
    "AlignKeyValuesInCollectionInitializer_SpaceAfterComma": (
        dino_body(block(plain, False)),
        dino_body(block(after, False)),
        [KEY_SETTING],
    ),
    "AlignKeyValuesInCollectionInitializer_SpaceBeforeComma": (
        dino_body(block(plain, False)),
        dino_body(block(before, False)),
        [KEY_SETTING],
    ),
    "AlignClosingBracesInCollectionInitializer": (
        dino_body(block(plain, False)),
        dino_body(block(plain, True)),
        [BRACE_SETTING],
    ),
    "AlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces": (
        dino_body(block(plain, False)),
        dino_body(block(after, True)),
        [KEY_SETTING, BRACE_SETTING],
    ),
    "AlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces": (
        dino_body(block(plain, False)),
        dino_body(block(before, True)),
        [KEY_SETTING, BRACE_SETTING],
    ),
    "AlignKeyValuesInCollectionInitializer_DoNotChange": (
        dino_body(block(plain, False)),
        dino_body(block(plain, False)),
        [KEY_SETTING],
    ),
    "AlignKeyValuesInCollectionInitializer_InlineNoChange": (
        INLINE_BODY,
        INLINE_BODY,
        [KEY_SETTING, BRACE_SETTING],
    ),
}


def main() -> None:
    for name, (body, gold_body, seealso) in FILES.items():
        (DIR / (name + ".cs")).write_text(wrapper(name, seealso, body, is_gold=False), encoding="utf-8", newline="\n")
        (DIR / (name + ".cs.gold")).write_text(wrapper(name, seealso, gold_body, is_gold=True), encoding="utf-8", newline="\n")
        print("wrote", name)


if __name__ == "__main__":
    main()
