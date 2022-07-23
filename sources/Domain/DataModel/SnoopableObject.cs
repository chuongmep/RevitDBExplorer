﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableObject : BaseViewModel
    {        
        private readonly List<SnoopableObject> items;         

        public object Object { get; }
        public Document Document { get; }
        public string Name { get; init; }
        public string NamePrefix { get; init; }
        public string TypeName { get; }       
        public IEnumerable<SnoopableObject> Items => items;
        public int Index { get; init; } = -1;
        

        public SnoopableObject(Document document, object @object, IEnumerable<SnoopableObject> subObjects = null)
        {
            this.Object = @object;
            this.Document = document;            
            this.Name = @object is not null ? Labeler.GetLabelForObject(@object, document) : "";
            this.TypeName = @object?.GetType().GetCSharpName();

            if (subObjects != null)
            {
                items = new List<SnoopableObject>(subObjects);
            }
            else
            {
                if (@object is IEnumerable enumerable && @object is not string && @object is not IDictionary)
                {
                    items = new List<SnoopableObject>();
                    foreach (var item in enumerable)
                    {
                        items.Add(new SnoopableObject(document, item));
                    }
                }
            }
        }

                
        public IEnumerable<SnoopableMember> GetMembers(UIApplication app)
        {
            if (Object == null)
            {
                yield break;
            }
            foreach (var member in GetMembersFromStreams(app))
            {
                member.ReadValue();
                if (!member.HasExceptionCouldNotResolveAllArguments)
                {
                    yield return member;
                }
            }
        }

        private static readonly SystemTypeStream SystemTypeHandler = new();
        private IEnumerable<SnoopableMember> GetMembersFromStreams(UIApplication app)
        {
            var type = Object.GetType();            
                
            foreach (var member in SystemTypeHandler.Stream(this))
            {
                yield return member;
            }
            if (SystemTypeHandler.ShouldEndAllStreaming())
            {
                yield break;
            }            

            foreach (var member in FactoryOfFactories.CreateSnoopableMembersFor(this))
            {
                yield return member;
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in properties)
            {
                if (prop.Name == "Parameter")
                {
                    continue;
                }

                var getMethod = prop.GetGetGetMethod();
                if (getMethod == null)
                {
                    continue;
                }

                var comments = () => RevitDocumentationReader.GetPropertyComments(prop);
                var memberAccessor = FactoryOfFactories.CreateMemberAccessor(getMethod, null);
                var member = new SnoopableMember(this, SnoopableMember.Kind.Property, prop.Name, prop.DeclaringType, memberAccessor, comments);              
                yield return member;
            }

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(void)) continue;
                if (method.IsSpecialName) continue;
                if (method.DeclaringType == typeof(object)) continue;               

                var comments = () => RevitDocumentationReader.GetMethodComments(method);
                var memberAccessor = FactoryOfFactories.CreateMemberAccessor(method, null);
                var member = new SnoopableMember(this, SnoopableMember.Kind.Method, method.Name, method.DeclaringType, memberAccessor, comments);
                yield return member;
            }
        }


        public static SnoopableObject CreateKeyValuePair(Document document, object key, object value, string keyPrefix = "key:", string valuePrefix = "value:")
        {
            return new SnoopableObject(document, key, new[] { new SnoopableObject(document, value) { NamePrefix = valuePrefix } }) { NamePrefix = keyPrefix };
        }
        public static SnoopableObject CreateInOutPair(Document document, object key, object value, string keyPrefix = "in:", string valuePrefix = "out:")
        {
            return new SnoopableObject(document, key, new[] { new SnoopableObject(document, value) { NamePrefix = valuePrefix } }) { NamePrefix = keyPrefix };
        }
    }
}