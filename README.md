# PropertyTree

Runtime Structured Properties

```c#
root = new PropertyGroup("Root", new[] {
    new PropertyGroup("GroupA", new[] {
        new IntProperty("i1", 0, 127),
        new BoolProperty("b1"),
        new FloatProperty("f1", -1f, +1f)
    },
    new PropertyGroup("GroupB", new[] {
        new StringProperty("s1"),
        new BoolProperty("b1")
    }
});
```

# About AI Generation

- This document has been machine translated.
- This repo contains generated code by ChatGPT and Cursor.
