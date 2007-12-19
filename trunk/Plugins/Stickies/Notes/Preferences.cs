// Copyright 2006 Bret Taylor
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain
// a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations
// under the License.

using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Stickies.Notes {
  /// <summary>
  /// Serializable struct representing Stickies preferences.
  /// </summary>
  public class Preferences : Settings {
    /// <summary>
    /// The default note settings.
    /// </summary>
    [XmlElement("DefaultNoteSettings")]
    public Note Note;

    public const string Path = "preferences.xml";

    public Preferences()
        : base( )
    {
        this.Note = new Note();
    }

    /// <summary>
    /// Returns the path to the Stickies preferences file.
    /// </summary>
    public override string GetPath() {
      return GetSettingsPath(Path);
    }
  }
}