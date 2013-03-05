// 
// Copyright (c) 2013, Erik Rydgren, et al. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//  - Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution.
//  - Neither the name of PluginFramework nor the names of its contributors may be used to endorse or promote products derived from this 
//    software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL ERIK RYDGREN OR OTHER CONTRIBUTORS 
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
//
namespace PluginFramework
{
  using System;

  /// <summary>
  /// Utility class for managing fully assembly qualified type names
  /// </summary>
  [Serializable]
  public struct QualifiedName
  {
    public QualifiedName(string qualifiedName)
      : this()
    {
      var parts = Split(qualifiedName);
      this.TypeFullName = parts[0];
      this.AssemblyFullName = parts[1];
    }

    public string TypeFullName
    {
      get; private set; 
    }

    public string AssemblyFullName
    {
      get; private set;
    }

    public override string ToString()
    {
      return this;
    }

    public static string[] Split(string qualifiedName)
    {
      string[] delimiters = new string[] { ", " };
      string[] nameParts = qualifiedName.Split(delimiters, 2, StringSplitOptions.None);
      return nameParts;
    }

    public static implicit operator QualifiedName(string name)
    {
      return new QualifiedName(name);
    }

    public static implicit operator string(QualifiedName name)
    {
      return name.TypeFullName + ", " + name.AssemblyFullName;
    }
  }
}
