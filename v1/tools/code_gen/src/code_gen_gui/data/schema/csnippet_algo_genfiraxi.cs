﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.81.0.
// 
namespace SchematicScriptCreator {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/VisualStudio/2008/CodeSnippet")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/VisualStudio/2008/CodeSnippet", IsNullable=false)]
    public partial class CodeSnippets {
        
        private CodeSnippetsCodeSnippet[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CodeSnippet")]
        public CodeSnippetsCodeSnippet[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/VisualStudio/2008/CodeSnippet")]
    public partial class CodeSnippetsCodeSnippet {
        
        private CodeSnippetsCodeSnippetHeader[] headerField;
        
        private CodeSnippetsCodeSnippetSnippetCode[][] snippetField;
        
        private string formatField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Header")]
        public CodeSnippetsCodeSnippetHeader[] Header {
            get {
                return this.headerField;
            }
            set {
                this.headerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Code", typeof(CodeSnippetsCodeSnippetSnippetCode))]
        public CodeSnippetsCodeSnippetSnippetCode[][] Snippet {
            get {
                return this.snippetField;
            }
            set {
                this.snippetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Format {
            get {
                return this.formatField;
            }
            set {
                this.formatField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/VisualStudio/2008/CodeSnippet")]
    public partial class CodeSnippetsCodeSnippetHeader {
        
        private string titleField;
        
        /// <remarks/>
        public string Title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.81.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://schemas.microsoft.com/VisualStudio/2008/CodeSnippet")]
    public partial class CodeSnippetsCodeSnippetSnippetCode {
        
        private string languageField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language {
            get {
                return this.languageField;
            }
            set {
                this.languageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
}
