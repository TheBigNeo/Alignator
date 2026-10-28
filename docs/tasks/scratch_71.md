Ich scha

ue mir zuerst die aktuellen Keys in `AlignatorSettingsKey` und die `Initialize`-Methode von `AlignArgumentsProvider` an.

Ich prüfe noch kurz

, wo die Keys noch referenziert werden (Rename-Auswirkung), dann stelle ich die Vorschläge zusammen.

Ich habe alle 11 Keys in `AlignatorSettingsKey`,

die `Initialize()`-Aufrufe, `AlignatorSettingsSchema` (UI-Seite) und die EditorConfig-Aliase geprüft. Hier die Analyse und die Namens-Vorschläge.

## Ist-Zustand: warum es nicht gruppiert

Das Namensschema ist `ALIGN_<WAS>_IN_<KONTEXT>` — der Kontext steht hinten. Alphabetisch sortiert ergibt das:

```
ALIGN_ARGUMENT_NAMES
ALIGN_ARGUMENTS_IN_ATTRIBUTE
ALIGN_ARGUMENTS_IN_CONSTRUCTOR
ALIGN_ARGUMENTS_IN_FUNCTION
ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION
ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER
ALIGN_CLOSING_PAREN_IN_ATTRIBUTE
ALIGN_FIRST_ARGUMENT_IN_ATTRIBUTE
ALIGN_FIRST_ARGUMENT_IN_CONSTRUCTOR
ALIGN_FIRST_ARGUMENT_IN_FUNCTION
ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER
```

Konstruktor, Funktion und At

tribut zerfallen in drei getrennte Blöcke, weil `ARGUMENTS`, `FIRST_ARGUMENT` und `CLOSING_PAREN` verschiedene "WAS"-Wörter sind. Nur wenn der **Kontext vorne** steht, sortieren zusammengehörige Keys automatisch nebeneinander.

Alle drei Lösungen folgen dem Muster `[Präfix] → [Kontext] → [Objekt] → [Variante]` — sie unterscheiden sich darin, wie fein die Varianten im Namen codiert sind.

## Lösung 1: Kontext zuerst — einfache Umstellung

Deine Quick-Idee, konsequent durchgeführt. Ein Key pro Zeile, alphabetisch sortiert wie er dann dastünde:

| Alt                                              | Neu (sortiert)                                       | EditorConfig-Alias neu                                  |
|--------------------------------------------------|------------------------------------------------------|---------------------------------------------------------|
| `ALIGN_ARGUMENT_NAMES`                           | `ALIGN_ARGUMENT_NAMES` (bleibt, kontextübergreifend) | `alignator_align_argument_names`                        |
| `ALIGN_ARGUMENTS_IN_ATTRIBUTE`                   | `ALIGN_ATTRIBUTE_ARGUMENTS`                          | `alignator_align_attribute_arguments`                   |
| `ALIGN_CLOSING_PAREN_IN_ATTRIBUTE`               | `ALIGN_ATTRIBUTE_CLOSING_PAREN`                      | `alignator_align_attribute_closing_paren`               |
| `ALIGN_FIRST_ARGUMENT_IN_ATTRIBUTE`              | `ALIGN_ATTRIBUTE_FIRST_ARGUMENT`                     | `alignator_align_attribute_first_argument`              |
| `ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER` | `ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES`        | `alignator_align_collection_initializer_closing_braces` |
| `ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER`     | `ALIGN_COLLECTION_INITIALIZER_KEY_VALUES`            | `alignator_align_collection_initializer_key_values`     |
| `ALIGN_ARGUMENTS_IN_CONSTRUCTOR`                 | `ALIGN_CONSTRUCTOR_ARGUMENTS`                        | `alignator_align_constructor_arguments`                 |
| `ALIGN_FIRST_ARGUMENT_IN_CONSTRUCTOR`            | `ALIGN_CONSTRUCTOR_FIRST_ARGUMENT`                   | `alignator_align_constructor_first_argument`            |
| `ALIGN_ARGUMENTS_IN_FUNCTION`                    | `ALIGN_FUNCTION_ARGUMENTS`                           | `alignator_align_function_arguments`                    |
| `ALIGN_FIRST_ARGUMENT_IN_FUNCTION`               | `ALIGN_FUNCTION_FIRST_ARGUMENT`                      | `alignator_align_function_first_argument`               |
| `ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION`           | `ALIGN_WITH_EXPRESSION_ASSIGNMENTS`                  | `alignator_align_with_expression_assignments`           |

Vorteil: geringste Änderungstiefe, Klartext-Lesbarkeit ("im Attribut: Argumente / erstes Argument / schließende Klammer" ist direkt ablesbar). Nachteil: "zusammengehörig" ist nur über den Kontext-Block erkennbar, die Verwandtschaft von `ARGUMENTS` und `FIRST_ARGUMENT` liegt im zweiten Wort — einem späteren `CLOSING_PAREN` für Konstruktor/Function fehlt eine offensichtliche Stelle.

## Lösung 2: Kontext zuerst + gemeinsamer Familien-Stamm (empfohlen)

Wie Lösung 1, aber alles Argument-bezogene eines Kontexts hängt am selben Stamm `ARGUMENTS`; Varianten sind Suffixe. Damit ist die Familie auch im Namen sichtbar, und neue Varianten ("closing paren in constructor") sloten sich später eindeutig ein:

| Alt                                              | Neu (sortiert)                                | EditorConfig-Alias neu                                  |
|--------------------------------------------------|-----------------------------------------------|---------------------------------------------------------|
| `ALIGN_ARGUMENT_NAMES`                           | `ALIGN_ARGUMENTS_NAMED`                       | `alignator_align_arguments_named`                       |
| `ALIGN_ARGUMENTS_IN_ATTRIBUTE`                   | `ALIGN_ATTRIBUTE_ARGUMENTS`                   | `alignator_align_attribute_arguments`                   |
| `ALIGN_CLOSING_PAREN_IN_ATTRIBUTE`               | `ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN`     | `alignator_align_attribute_arguments_closing_paren`     |
| `ALIGN_FIRST_ARGUMENT_IN_ATTRIBUTE`              | `ALIGN_ATTRIBUTE_ARGUMENTS_FIRST`             | `alignator_align_attribute_arguments_first`             |
| `ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER` | `ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES` | `alignator_align_collection_initializer_closing_braces` |
| `ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER`     | `ALIGN_COLLECTION_INITIALIZER_KEY_VALUES`     | `alignator_align_collection_initializer_key_values`     |
| `ALIGN_ARGUMENTS_IN_CONSTRUCTOR`                 | `ALIGN_CONSTRUCTOR_ARGUMENTS`                 | `alignator_align_constructor_arguments`                 |
| `ALIGN_FIRST_ARGUMENT_IN_CONSTRUCTOR`            | `ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST`           | `alignator_align_constructor_arguments_first`           |
| `ALIGN_ARGUMENTS_IN_FUNCTION`                    | `ALIGN_FUNCTION_ARGUMENTS`                    | `alignator_align_function_arguments`                    |
| `ALIGN_FIRST_ARGUMENT_IN_FUNCTION`               | `ALIGN_FUNCTION_ARGUMENTS_FIRST`              | `alignator_align_function_arguments_first`              |
| `ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION`           | `ALIGN_WITH_EXPRESSION_ASSIGNMENTS`           | `alignator_align_with_expression_assignments`           |

Die Idee: `ALIGN_<KONTEXT>_ARGUMENTS` ist die Basis, `..._FIRST`, `..._CLOSING_PAREN` sind Varianten davon. Das passt auch zum späteren Plan, `AlignArgumentsProvider` aufzuteilen — dann entsteht pro Kontext eine Datei (`ConstructorArgumentsProvider`, `AttributeArgumentsProvider`, …) und der Stamm entspricht genau der Datei-Zuständigkeit. Der kontextübergreifende Key `ALIGN_ARGUMENT_NAMES` wird zu `ALIGN_ARGUMENTS_NAMED` und sortiert damit an den Kopf der ganzen Argument-Familie; wenn er später pro Kontext aufgesplittet wird (`ALIGN_CONSTRUCTOR_ARGUMENTS_NAMED` etc.), fügt er sich ohne
Schema-Bruch ein.

## Lösung 3: Objekt zuerst, Kontext als Suffix

Umkehrung: alles Arguments-Formatierungs-Keys global in einem Block, Kontexte sortieren innerhalb:

| Alt                                              | Neu (sortiert)                                           | EditorConfig-Alias neu                                             |
|--------------------------------------------------|----------------------------------------------------------|--------------------------------------------------------------------|
| `ALIGN_ARGUMENTS_IN_ATTRIBUTE`                   | `ALIGN_ARGUMENTS_ATTRIBUTE`                              | `alignator_align_arguments_attribute`                              |
| `ALIGN_CLOSING_PAREN_IN_ATTRIBUTE`               | `ALIGN_ARGUMENTS_ATTRIBUTE_CLOSING_PAREN`                | `alignator_align_arguments_attribute_closing_paren`                |
| `ALIGN_FIRST_ARGUMENT_IN_ATTRIBUTE`              | `ALIGN_ARGUMENTS_ATTRIBUTE_FIRST`                        | `alignator_align_arguments_attribute_first`                        |
| `ALIGN_ARGUMENTS_IN_CONSTRUCTOR`                 | `ALIGN_ARGUMENTS_CONSTRUCTOR`                            | `alignator_align_arguments_constructor`                            |
| `ALIGN_FIRST_ARGUMENT_IN_CONSTRUCTOR`            | `ALIGN_ARGUMENTS_CONSTRUCTOR_FIRST`                      | `alignator_align_arguments_constructor_first`                      |
| `ALIGN_ARGUMENTS_IN_FUNCTION`                    | `ALIGN_ARGUMENTS_FUNCTION`                               | `alignator_align_arguments_function`                               |
| `ALIGN_FIRST_ARGUMENT_IN_FUNCTION`               | `ALIGN_ARGUMENTS_FUNCTION_FIRST`                         | `alignator_align_arguments_function_first`                         |
| `ALIGN_ARGUMENT_NAMES`                           | `ALIGN_ARGUMENTS_NAMED`                                  | `alignator_align_arguments_named`                                  |
| `ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION`           | `ALIGN_ASSIGNMENTS_WITH_EXPRESSION`                      | `alignator_align_assignments_with_expression`                      |
| `ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER`     | `ALIGN_KEY_VALUES_COLLECTION_INITIALIZER`                | `alignator_align_key_values_collection_initializer`                |
| `ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER` | `ALIGN_KEY_VALUES_COLLECTION_INITIALIZER_CLOSING_BRACES` | `alignator_align_key_values_collection_initializer_closing_braces` |

Vorteil: die .editorconfig-Aliase ändern sich kaum (nur `IN_` fällt weg). Nachteil: Kontexte stehen hinten, "was gilt für Attribute?" erfordert Scannen; das `IN`-Lesbarkeitsmuster geht verloren.

## Vergleich

| Kriterium                                                   | Lösung 1 (Kontext zuerst) | Lösung 2 (Kontext + Stamm)                | Lösung 3 (Objekt zuerst)                  |
|-------------------------------------------------------------|---------------------------|-------------------------------------------|-------------------------------------------|
| Alphabetisch gruppiert                                      | ja                        | ja                                        | ja, nach Objekt                           |
| Familie im Namen sichtbar                                   | nur über Kontext-Block    | ja, über Stamm `ARGUMENTS`                | ja, über Objekt-Block                     |
| Neue Variante einfügbar (z.B. closing paren in constructor) | intuitiv                  | eindeutig (`..._ARGUMENTS_CLOSING_PAREN`) | eindeutig                                 |
| Lesbarkeit "Kontext vorne"                                  | gut                       | gut                                       | mittel (Suffix-Kontext)                   |
| Passt zu späterem Datei-Split pro Kontext                   | ja                        | am besten (Stamm = Datei)                 | mittel (Split müsste am 2. Wort andocken) |
| Änderungsumfang                                             | mittel                    | mittel                                    | kleinste Alias-Änderung                   |

Empfehlung: **Lösung 2**. Sie ist die einzige, bei der Zugehörigkeit sowohl im Namen als auch in der Sortierung sichtbar ist, und sie strukturiert bereits für den geplanten Provider-Split vor.

## Übergreifende Punkte (unabhängig von der Wahl)

- **Persistenz:** Die Feldnamen sind die Settings-Entry-Keys im JetBrains Settings-Store. Ein Rename verwirft benutzerdefinierte Werte in bestehenden Layers (Settings-Speicher, `.editorconfig` mit alten `alignator_align_*`-Aliases). Da der Store-Fallback auf den `[SettingsEntry]`-Default greift, ist das kein Bruch, aber User-Einstellungen müssen neu gesetzt werden.
- **`Initialize()`-Methodennamen:** Die folgen heute dem Key-Namen als Präfix (`ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma` → künftig z.B. `ALIGN_CONSTRUCTOR_ARGUMENTS_SpaceBeforeComma`) plus dem `nameof(...) + "_Assignment"`-Suffix in `SettingName` — beides folgt automatisch mit und muss bei der Umsetzung nur konsequent mit-umbenannt werden. Eine Sortierung der Methoden selbst ist, wie von dir gesagt, nicht Teil dieses Schritts.
- **Datei-Ordnung:** In `AlignatorSettingsKey.cs` sollten die Felder danach in exakt der alphabetischen Reihenfolge stehen, dann stimmen Code-Ansicht, Sortierung und UI-Seite (`AlignatorSettingsSchema`, deren Reihenfolge über die `ItemFor`-Aufrufe definiert wird) überein.

Wenn du dich für eine Lösung entscheidest, setze ich das Rename in `AlignatorSettingsKey.cs`, `AlignatorSettingsSchema.cs` und den `Initialize()`-Methodennamen um.

Configured authentication: Managed by agent