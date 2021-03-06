﻿using System;
using System.Collections.Generic;
using System.Text;
using Bara.Abstract.Tag;
using Bara.Common;
using Bara.Core.Context;

namespace Bara.Core.Tags
{
    public class Switch : Tag
    {
        public override TagType Type => TagType.Switch;

        public override bool IsNeedShow(RequestContext context)
        {
            return true;
        }

        public class Case : CompareTag
        {
            public override TagType Type => TagType.Case;

            public override bool IsNeedShow(RequestContext context)
            {
                var reqVal = context.GetValue(Property);
                if (reqVal == null)
                {
                    return false;
                }
                String caseValue = string.Empty;
                if (reqVal is Enum)
                {
                    caseValue = reqVal.GetHashCode().ToString();
                }
                else
                {
                    caseValue = reqVal.ToString();
                }
                return caseValue.Equals(CompareValue);
            }
        }

        public class Default : Tag
        {
            public override TagType Type => TagType.SwitchDefault;

            public override bool IsNeedShow(RequestContext context)
            {
                return true;
            }
        }
    }
}
