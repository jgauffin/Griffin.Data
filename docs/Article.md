Introduction
===========

This article is about version 2.0 of my object/relation mapper and data mapper Griffin.Data. It's a library which is designed to work in both the write and read side of your application.

Before we get into the library itself, let's talk about business entities, or domain entities as they are called in Domain Driven Design. Business entities are classes which are used to ensure that the logic is encapsulated and that changes are driven by behavior (invoking methods). 

This is a poco (Plain Old CLR Object):

```csharp
public class Account
{
    public int Id { get; set; }
    public AccontState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LockedAt { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

It has no control over it's sttate. An part of the application can 