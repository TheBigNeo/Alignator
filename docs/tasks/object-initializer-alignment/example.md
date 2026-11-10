
Füge eine weitere Setting hinzu, welche aus so so einem code:


```csharp
  public void MyTest()
  {
    List<objext> Conditions = new List<objext>();
    Conditions =
    [
      new NumberCondition() { SourceName   = "Studio 1", CompareValue = 0 },
      new NumberCondition() { SourceName   = "Studio 2", CompareValue = 0 },
      new NumberCondition() { SourceName   = "Studio 3", CompareValue = 0 },
      new NumberCondition() { SourceName   = "Studio 4", CompareValue = 0 },
      new CheckStatusCondition() { ModelId = Guid.Parse("3095a366-b1fe-42a1-a8f5-789c955eee3f") },
      new JsonPathCondition() { JsonPath   = "$.fellowship.members[8]", IsNegated = true },
      new RegexCondition() { Pattern       = "Studio 1", SourceName               = "StudioName" },
    ];
  }
```


Das hier macht: (mit before after für die `{` )

```csharp
  public void MyTest()
  {
    List<objext> Conditions = new List<objext>();
    Conditions =
    [
      new NumberCondition()      { SourceName = "Studio 1", CompareValue = 0 },
      new NumberCondition()      { SourceName = "Studio 2", CompareValue = 0 },
      new NumberCondition()      { SourceName = "Studio 3", CompareValue = 0 },
      new NumberCondition()      { SourceName = "Studio 4", CompareValue = 0 },
      new CheckStatusCondition() { ModelId    = Guid.Parse("3095a366-b1fe-42a1-a8f5-789c955eee3f") },
      new JsonPathCondition()    { JsonPath   = "$.fellowship.members[8]", IsNegated = true },
      new RegexCondition()       { Pattern    = "Studio 1", SourceName               = "StudioName" },
    ];
  }
```


Beachte, dass das nicht das gleiche ist wie folgednes was eine zusätzliche setting sein soll


```csharp
  public void MyTest()
  {
    List<objext> Conditions = new List<objext>();
    Conditions =
    [
      new NumberCondition()      { SourceName = "Studio 1",                                        CompareValue = 0 },
      new NumberCondition()      { SourceName = "Studio 2",                                        CompareValue = 0 },
      new NumberCondition()      { SourceName = "Studio 3",                                        CompareValue = 0 },
      new NumberCondition()      { SourceName = "Studio 4",                                        CompareValue = 0 },
      new CheckStatusCondition() { ModelId    = Guid.Parse("3095a366-b1fe-42a1-a8f5-789c955eee3f") },
      new JsonPathCondition()    { JsonPath   = "$.fellowship.members[8]",                         IsNegated    = true },
      new RegexCondition()       { Pattern    = "Studio 1",                                        SourceName   = "StudioName" },
    ];
  }
```


Und dann noch eine Setting für die closing klammern


```csharp
  public void MyTest()
  {
    List<objext> Conditions = new List<objext>();
    Conditions =
    [
      new NumberCondition()      { SourceName = "Studio 1",                                        CompareValue = 0            },
      new NumberCondition()      { SourceName = "Studio 2",                                        CompareValue = 0            },
      new NumberCondition()      { SourceName = "Studio 3",                                        CompareValue = 0            },
      new NumberCondition()      { SourceName = "Studio 4",                                        CompareValue = 0            },
      new CheckStatusCondition() { ModelId    = Guid.Parse("3095a366-b1fe-42a1-a8f5-789c955eee3f")                             },
      new JsonPathCondition()    { JsonPath   = "$.fellowship.members[8]",                         IsNegated    = true         },
      new RegexCondition()       { Pattern    = "Studio 1",                                        SourceName   = "StudioName" },
    ];
  }
```