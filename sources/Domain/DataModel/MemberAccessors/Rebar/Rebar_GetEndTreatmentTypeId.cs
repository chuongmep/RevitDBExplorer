﻿using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Rebar_GetEndTreatmentTypeId : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Rebar x) => x.GetEndTreatmentTypeId(7); } }
        IMemberAccessor ICanCreateMemberAccessor.Create() => new Rebar_GetEndTreatmentTypeId();


        protected override bool CanBeSnoooped(Document document, Rebar rebar) => true;
        protected override string GetLabel(Document document, Rebar rebar) => $"[{nameof(Element)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Rebar rebar)
        {
            for (int i = 0; i < 2; ++i)
            {
                var result = rebar.GetEndTreatmentTypeId(i);
                yield return SnoopableObject.CreateInOutPair(document, i, result, "end");
            }
        }
    }
}