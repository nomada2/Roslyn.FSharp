﻿namespace Roslyn.FSharp.Tests

open System.Linq
open Microsoft.CodeAnalysis
open Roslyn.FSharp
open NUnit.Framework

module ``Compilation tests`` =
    [<Test>]
    let ``can get type by metadata name``() =
        let compilation = 
            """
            namespace MyNamespace
            type MyType() = class end
            """
            |> getCompilation

        let namedType = compilation.GetTypeByMetadataName("MyNamespace.MyType")
        Assert.AreEqual("MyType", namedType.Name)
        Assert.AreEqual("MyType", namedType.MetadataName)


    [<Test>]
    let ``can get compilation name``() =
        let compilation = getCompilation ""
        let mscorlib =
            compilation.References |> Seq.find(fun r -> r.Display.EndsWith "mscorlib.dll")
        let name = compilation.GetAssemblyOrModuleSymbol(mscorlib).Name
        Assert.AreEqual("mscorlib", name)

    [<Test>]
    let ``global namespace``() =
        let compilation =
            """
            type GlobalType() =
                member x.X = 1
            """
            |> getCompilation

        let mscorlib =
            compilation.References |> Seq.find(fun r -> r.Display.EndsWith "mscorlib.dll")
        let assembly = compilation.GetAssemblyOrModuleSymbol(mscorlib) :?> IAssemblySymbol
        Assert.AreEqual(assembly.GlobalNamespace.IsGlobalNamespace, true)

    [<Test>]
    let ``Global namespace GetNamespaceMembers``() =
        let compilation = getCompilation ""
        let mscorlib =
            compilation.References |> Seq.find(fun r -> r.Display.EndsWith "mscorlib.dll")

        let asm = compilation.GetAssemblyOrModuleSymbol(mscorlib) :?> IAssemblySymbol
        let namespaces =
            asm.GlobalNamespace.GetNamespaceMembers()

        let namespaces =
            namespaces
            |> Seq.map(fun n -> n.Name)
            |> List.ofSeq

        CollectionAssert.AreEqual(["Internal"; "Microsoft"; "Mono"; "System"; "XamMac"], namespaces)


    [<Test>]
    let ``Assembly NamespaceNames``() =
        let compilation = getCompilation ""
        let mscorlib = compilation.References.First()

        let asm = compilation.GetAssemblyOrModuleSymbol(mscorlib) :?> IAssemblySymbol
        let namespaces =
            asm.NamespaceNames
            |> List.ofSeq
            |> List.sort

        let expected =
            ["AccessControl"; "Activation"; "Assemblies"; "Augments"; "Authenticode";
             "Binary"; "Buffers"; "Channels"; "Claims"; "CodeAnalysis"; "Collections";
             "ComTypes"; "CompilerServices"; "Concurrent"; "Configuration";
             "ConstrainedExecution"; "Contexts"; "Contracts"; "CoreFoundation";
             "Cryptography"; "Deployment"; "Diagnostics"; "Emit"; "ExceptionServices";
             "Expando"; "Extensions"; "Formatters"; "Generator"; "Generic"; "Globalization";
             "Hashing"; "Hosting"; "IO"; "Internal"; "Interop"; "InteropServices";
             "IsolatedStorage"; "Lifetime"; "Math"; "Messaging"; "Metadata"; "Microsoft";
             "Mono"; "Numerics"; "ObjectModel"; "Permissions"; "Policy"; "Prime";
             "Principal"; "Private"; "Proxies"; "Reflection"; "Remoting"; "Resources";
             "Runtime"; "SafeHandles"; "Security"; "Serialization"; "Services";
             "SymbolStore"; "System"; "Tasks"; "Text"; "Threading"; "Tracing"; "Unicode";
             "Util"; "Versioning"; "W3cXsd2001"; "Win32"; "WindowsRuntime"; "X509";
             "X509Certificates"; "XamMac"; "Xml"]

        CollectionAssert.AreEqual(expected, namespaces)