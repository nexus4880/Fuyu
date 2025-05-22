# Contributing

## Code convention

If it is not listed here and you're in doubt, follow C#'s Code Convention
[link](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

### Limitations

- Don't use `struct`: too many odd rules around it being copied or referenced.
  Ask for exemption and give the reason why you need it in your PR.
- Don't use closures except for inside LINQ. If you need it outside of that,
  ask for exemption and give the reason why you need it in your PR.
- Don't use reflection except inside `Fuyu.Client.*`. If you need it outside
  of that, ask for exemption and and give the reason why you need it in your
  PR.
- Keep range operations simple: only `[..16]`, `[16..]`, `[^1]`, `[0..^1]`.
- Don't use `ref` outside of `Fuyu.Client.*`. If you need to use this outside
  of reflection context, you did something wrong.
- Models: no methods unless you are certain they will serve a purpose and will
  be widely consumed
- Linq: use where elegant, but make it readable (one operation per line instead
  of chaining on the same line, break complex statements into multiple
  variables)
- NEVER EVER use variable chain initialization (`var a = 10, b = 20, ...;`).
- Do not run a method inside a constructor, unless you got a good reason to and
  mention it in the PR.
- Do not use `async void` unless you can explain in the PR why you do it.
  Saying "because I cannot call it in a synchronized method otherwise" is often
  NOT a valid reason.

### Naming convention

#### Naming

In general, use fully-written names instead of abbreviations. This helps with
finding desired types via autocomplete.

When dealing with EFT or EFT: Arena types, prefer to use non-obfuscated
`Assembly-CSharp` type names. Otherwise use contextual logical names.

#### Class

- class: `CamelCase`

```cs
class MyClass
{
}
```

- `abstract` class: prefix with Abstract

```cs
abstract class AbstractMyClass
{
}
```

#### Interface

- interface: `CamelCase`, prefix with I

```cs
interface IMyInterface
{
}
```

#### Enum

- enum: `CamelCase`, prefix with E

```cs
enum EMyEnum
{
}
```

#### Fields, properties

- `private` fields/properties: `_lowerCase`

```cs
class MyClass
{
    private int _a;
    private int _b { get; set; }
}
```

- `protected` fields/properties: `CamelCase`

```cs
class MyClass
{
    protected int A;
    protected int B { get; set; }
}
```

- `internal` fields/properties: `CamelCase`

```cs
class MyClass
{
    internal int A;
    internal int B { get; set; }
}
```

- `public` fields/properties: `CamelCase`

```cs
class MyClass
{
    public int A;
    public int B { get; set; }
}
```

- `const` fields: `SNAKE_CASE`

```cs
class MyClass
{
    private const int CONSTANT_A = 1;
    protected const int CONSTANT_B = 5;
    internal const int CONSTANT_C = 10;
    public const int CONSTANT_D = 20;
}
```

#### Methods

`CamelCase`

```cs
class MyClass
{
    void MyMethod()
    {
    }
}
```

- `async` method: `CamelCase`, postfix Async

```cs
class MyClass
{
    async void MyMethodAsync()
    {
    }
}
```

#### Variables

- variable: `lowerCase`

```cs
class MyClass
{
    void MyMethod()
    {
        var myVariable = 10;
    }
}
```

- stream variable: abbriviated `lowerCase`

```cs
class MyClass
{
    void MyMethod()
    {
        using var ms = new MemoryStream();
    }
}
```

### Specific naming

- A `*Server` should be postfixed with `Server`

```cs
class MyServer : HttpServer
{
    // code here
}
```

- A `*Router` should be postfixed with `Router`

```cs
class MyRouter : HttpRouter
{
    // code here
}
```

- A `*Request` should be postfixed with `Request`

```cs
class MyResponse : HttpResponse
{
    // code here
}
```

- A `*Response` should be postfixed with `Response`

```cs
class MyRequest : HttpRequest
{
    // code here
}
```

- A `*Service` should be postfixed with `Service`

```cs
class MyService
{
    // code here
}
```

- A `*Message` should be postfixed with `Message`

```cs
class MyMessage : Message
{
    // code here
}
```

- A `*Reply` should be postfixed with `Reply`

```cs
class MyReply : Reply
{
    // code here
}
```

- A `*Page` should be postfixed with `Page`

```cs
class MyPage : Page
{
    // code here
}
```

### Structures

#### Code spacing

Have newlines after using control statements and variable block initialization

> This is WRONG

```cs
class MyClass
{
    void MyMethod()
    {
        var a = 10;
        if (a > 9)
        {
            var b = 20;
            if (b > 10)
            {
                // do somethign
            }
        }
        var c = 20;
        var d = 3;
        if (a < 9 && c > 5)
        {
            // do something
        }
        if (d == 3)
        {
            // do something
        }
    }
}
```

It is hard to read as it is hard to separate the logic visually.

> This is CORRECT

```cs
class MyClass
{
    void MyMethod()
    {
        var a = 10;

        if (a > 9)
        {
            var b = 20;
            
            if (b > 10)
            {
                // do somethign
            }
        }

        var c = 20;
        var d = 3;

        if (a < 9 && c > 5)
        {
            // do something
        }

        if (d == 3)
        {
            // do something
        }
    }
}
```

Because of the separation, it is visually easier to break down and read the
code, allowing for less mistakes to be made.

#### Methods in models

Allowed, but must bind to "static" implementation when the code is expected to be resued

```cs
class MyType
{
    ItemInstance FindItem()
    {
        return ItemService.Find("id1");
    }
}

class MyService
{
    void DoSomething()
    {
        var itemInstance = ItemService.Find("id3");
    }
}

public class ItemService
{
    public static ItemInstance Find(string id)
    {
        // do something here
        return null;
    }
}
```

#### Variables

Use `var` wherever possible

```cs
var myVariable = 10;
```

Do **NOT** have uninitialized variables

> This is WRONG

```cs
MongoId id; 

// ...

var a = true;

if (a)
{
    id = new MongoId(true);
}
```

> This is CORRECT

```cs
var a = true;

// ...

var id = switch a
{
    true => new MongoId(true)
};
```

#### Static classes

Use `Lazy<T>` singletons

```cs
public static MyClass Instance => instance.Value;
private static readonly Lazy<MyClass> instance = new(() => new MyClass());
```

#### Initializing classes

- Fields should always be assigned in the constructor and never on their own.
- Properties should almost always be assigned in the constructor and never on
  their own unless you have a good reason for this (mention it in the PR).
- Store a reference to singletons inside your class and assign the instance in
  the constructor.
- Anything that is not set outside of the constructor must be marked
  `readonly`.

```cs
public class MyClass
{
    private readonly MyService _myService;
    private readonly int _value;
    public int _value2 { get; private set; }

    public MyClass()
    {
        _myService = MyService.Instance;
        _value = 3;
        _value2 = 5;
    }

    public DoSomething()
    {
        _value = 4;
    }
}


#### One operation per line

To improve ease of debugging and visually parsing the code, be verbose about
your code flow and keep it simple.

> This is WRONG

```cs
var dictionary = new Dictionary<string, string>();
var value = string.Empty;

if (!dictionary.TryGet(key, out value))
{
    value = dictionary[key] = "Hello, world!";
}
```

Here we assign the value `"Hello, world!"` on both `value` and
`dictionary[key]`, which is hard to parse as you need to read it in your head
recursively.

```
value           = dictionary[key]
dictionary[key] = "Hello, world!"
value           = "Hello, world!"
```

> This is CORRECT

```cs
var dictionary = new Dictionary<string, string>();
var value = string.Empty;

if (!dictionary.TryGet(key, out value))
{
    dictionary[key] = "Hello, world!";
    value = "Hello, world!";
}
```

Here it is immediately obvious what value is being assigned. The code is easier
to refactor and more specific to debug.

```
dictionary[key] = "Hello, world!"
value           = "Hello, world!"
```

### Models

A model is a class that describes what parsed data like a json file looks like.

#### General

When creating models, use `DataContract` and `DataMember`.

If your model is like this:

```json
{
    "IsEnabled": true
}
```

Then it converts like this:

```cs
[DataContract]
public class Config
{
    [DataMember]
    public bool IsEnabled;
}
```

#### Renaming

When the json looks like this:

```json
{
    "isEnabled": true
}
```

...and still want to comply with our naming convention, you can specify the
name of the property in the `DataMember` attribute.

```cs
[DataContract]
public class Config
{
    [DataMember(Name = "isEnabled")]
    public bool IsEnabled;
}
```

#### Enums

An enum is normally parsed as `int`.

```cs
public enum Option
{
    First,
    Second,
    Third
}

[DataContract]
public class Config
{
    [DataMember]
    public Option Option;
}
```

```json
{
    "Option": 1
}
```

If you want it to be parsed as `string`, use `DataMember`.

```cs
public enum Option
{
    [DataMember(Name = "First")]
    First,
    [DataMember(Name = "Second")]
    Second,
    [DataMember(Name = "Third")]
    Third
}

[DataContract]
public class Config
{
    [DataMember]
    public Option Option;
}
```

```json
{
    "Option": "First"
}
```

#### Maybe-nullable data

Sometimes an object might be inconsistent by having missing properties.

```json
{
    "entries": [
        {
            "a": 1
        },
        {
            "a": 2,
            "b": 3
        }
    ]
}
```

This can be parsed by setting `EmitDefaultValue = false` and mark the type
as nullable (`T?`).

```cs
[DataContract]
public class ConfigEnty
{
    [DataMember(Name = "a")]
    public int A;

    [DataMember(Name = "b", EmitDefaultValue = false)]
    public int? B;
}

[DataContract]
public class Config
{
    [DataMember(Name = "entries")]
    public ConfigEntry[] Entries;
}
```

### Goto

It is allowed and even preferred but only under very specific circumstances,
with additional rules:

- You are **ONLY** allowed to jump **FORWARD** inside a method
- `goto` must be the last statement in a scope
- Always comment usage
- Only use it for the following patterns:
  - Labled break
  - Error handling
  - Cleanup routine

#### Labled breaks

> This is WRONG

```cs
for (var i = 0; i < 10; i++)
{
    var isFound = false;

    for (var j = 0; j < 10; j++)
    {
        if (i == 4 && j == 8)
        {
            isFound = true;
        }
    }

    if (isfound == true)
    {
        break;
    }
}
```

Why is this wrong? Because the additional control statements convolute the
logic you're trying to express

> this is CORRECT

```cs
for (var i = 0; i < 10; i++)
{
    for (var j = 0; j < 10; j++)
    {
        if (i == 4 && j == 8)
        {
            // found, labled break
            goto loop_end:
        }
    }
}
loop_end:
```

Why is this right? It is immediately clear what you're trying to achieve and is
more concise.

#### Error handling

> This is WRONG

```cs
var profile = _eftOrm.GetActiveProfile(context.SessionId);

var found = profile.Builds.EquipmentBuilds.RemoveAll(x => x.Id == request.Id);
if (found == 0)
{
    found = profile.Builds.WeaponBuilds.RemoveAll(x => x.Id == request.Id);
}
if (found == 0)
{
    found = profile.Builds.MagazineBuilds.RemoveAll(x => x.Id == request.Id);
}
if (found == 0)
{
    throw new Exception($"Could not find a build with the id {request.Id}");
}

return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
```

While this would be shorter than the `goto` variant, the control flow is harder
to folow as they are chained together. It is also heavier to run as always has
to branch 3 times in CPU instructions.

> This is CORRECT

```cs
var profile = _eftOrm.GetActiveProfile(context.SessionId);

var found = profile.Builds.EquipmentBuilds.RemoveAll(x => x.Id == request.Id);
if (found > 0)
{
    // found entry, handle result
    goto completed;
}

found = profile.Builds.WeaponBuilds.RemoveAll(x => x.Id == request.Id);
if (found > 0)
{
    // found entry, handle result
    goto completed;
}

found = profile.Builds.MagazineBuilds.RemoveAll(x => x.Id == request.Id);
if (found > 0)
{
    // found entry, handle result
    goto completed;
}

throw new Exception($"Could not find a build with the id {request.Id}");

completed:
return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
```

While this is longer than the other version, you immediately stop when you
found something and it only generate CPU branch instructions as many times as
it fails.

#### Cleanup routine

> This is WRONG

```cs
var stream1 = GetStream("firstStream");

if (stream1 == null)
{
    return;
}

var stream2 = GetStream("secondStream");

if (stream2 == null)
{
    stream1.Close();
    return;
}

var stream3 = GetStream("thirdStream");

if (stream3 == null)
{
    stream2.Close();
    stream1.Close();
    return;
}

// ...

stream3.Close();
stream2.Close();
stream1.Close();
```

This is bad because you end up repeating yourself many times and have to
rearrange it for every `if` statement.

> This is CORRECT

```cs
var stream1 = GetStream("firstStream");
if (stream1 == null)
{
    // a stream failed to instantiate, cleanup streams
    goto cleanup;
}

var stream2 = GetStream("secondStream");
if (stream2 == null)
{
    // a stream failed to instantiate, cleanup streams
    goto cleanup;
}

var stream3 = GetStream("thirdStream");
if (stream3 == null)
{
    // a stream failed to instantiate, cleanup streams
    goto cleanup;
}

// ...

cleanup:
if (stream3 != null)
{
    stream3.Close();
}

if (stream2 != null)
{
    stream2.Close();
}

if (stream1 != null)
{
    stream1.Close();
}
```

Here we don't duplicate the cleanup routine, it's reselient for refactors as
it handles all streams in the cleanup, and it's less prone to making mistakes.

## Converting BSG packet data

### Type conversion table

TS type          | C# type
---------------- | -----------------------------
`number`         | `int`/`long`/`float`/`double`
`string`         | `string`
`object`         | `class`
`array`          | `List<T>` or `T[]`
`Record<T1, T2>` | `Dictionary<T1, T2>`

### Notes

- If the model is a request or response model (the type of `data`) and is `[]`,
  it is `T[]`. In any other case, it's `List<T>`.
- If the entry might be missing, use `[DataMember(EmitDefaultValue = false)]`
  and make it a nullable type (example: `HideoutInfo? HideoutInfo`).
```cs
[DataMember(Name = "prestigeLevel", EmitDefaultValue = false)]
public int? PrestigeLevel { get; set; }
```

- If you do not know the correct type, use `object` and add the comment
  `// TODO: proper type`.

```cs
// TODO: proper type
// -- seionmoya, 2024-01-09
[DataMember(Name = "exits")]
public object[] Exits { get; set; }
```

- If the member can be two possible types, use `Union<T1, T2>`. For example:

```cs
/* version 1:
{
    "TraderInfo": {
        "totalSalesSum": 10
    }
}
*/
/* Version 2:
{
    "TraderInfo": []
}
*/

[DataMember(Name = "TraderInfo")]
public Union<TraderInfo, object[]> TraderInfo { get; set; }
```
