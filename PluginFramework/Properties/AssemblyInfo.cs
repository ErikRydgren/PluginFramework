﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("PluginRepository")]
[assembly: AssemblyDescription("Framework for finding and instanciating plugins from a dynamic and possibly remote plugin repository")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Erik Rydgren")]
[assembly: AssemblyProduct("PluginRepository")]
[assembly: AssemblyCopyright("Copyright © Erik Rydgren 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en-US")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3d553cc2-5492-4fae-bafc-01a0e3874e64")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Allow unit tests access to internal stuff
[assembly: InternalsVisibleTo("PluginFramework.Tests")]
