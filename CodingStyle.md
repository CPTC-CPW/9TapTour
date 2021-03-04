# Coding Styles and Guidelines for 9Tap

## C# Coding Style
```csharp
    // Variables are camelCased
    // C# data types (not .NET Types) are used for variables
    int myVariable = 0; 

    // Methods are PascalCased
    // Curly braces are vertically aligned
    public MyMethod()
    {

    }

    // Types are PascalCased
    class MyClass
    {

    }
```
### Whitespace Guidelines
Methods have a single empty line between them

Spaces are used before and after comparison operators
and logical operators. A space follows any decision/loop keyword
```csharp
    if (inputValue > 5 || score < 10)
    {

    }
    for (...)
    {

    }
```

### Commenting
Comments should start with a space, and the first word
with a capital letter. Ending a comment with a period
is optional. Spelling and grammar is used correctly.
```csharp
    // This is an example comment.
```

## Naming controls

Controls should begin with a prefix

| Control | Prefix|
| :---- | :----: |
| Button | btn  |
| CheckBox | cb  |
| ComboBox | cbx |
| FlowLayoutPanel | flp |
| Form | frm |
| GroupBox | grp |
| Label | lbl |
| ListBox | lbx |
| Panel | pnl |
| RadioButton | rdo |
| RichTextBox | rtxt |
| TextBox | txt |

## WinForms Styles/Guidelines
The default control backcolor is 
SystemColors.Control