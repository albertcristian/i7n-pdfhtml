/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Text;
using iText.Html2pdf.Css;
using iText.Html2pdf.Css.Resolve.Shorthand;
using iText.Html2pdf.Css.Util;
using iText.IO.Log;

namespace iText.Html2pdf.Css.Resolve.Shorthand.Impl {
    public class FontShorthandResolver : IShorthandResolver {
        private static readonly ICollection<String> UNSUPPORTED_VALUES_OF_FONT_SHORTHAND = new HashSet<String>(iText.IO.Util.JavaUtil.ArraysAsList
            (CssConstants.CAPTION, CssConstants.ICON, CssConstants.MENU, CssConstants.MESSAGE_BOX, CssConstants.SMALL_CAPTION
            , CssConstants.STATUS_BAR));

        private static readonly ICollection<String> FONT_WEIGHT_NOT_DEFAULT_VALUES = new HashSet<String>(iText.IO.Util.JavaUtil.ArraysAsList
            (CssConstants.BOLD, CssConstants.BOLDER, CssConstants.LIGHTER, "100", "200", "300", "400", "500", "600"
            , "700", "800", "900"));

        private static readonly ICollection<String> FONT_SIZE_VALUES = new HashSet<String>(iText.IO.Util.JavaUtil.ArraysAsList
            (CssConstants.MEDIUM, CssConstants.XX_SMALL, CssConstants.X_SMALL, CssConstants.SMALL, CssConstants.LARGE
            , CssConstants.X_LARGE, CssConstants.XX_LARGE, CssConstants.SMALLER, CssConstants.LARGER));

        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (UNSUPPORTED_VALUES_OF_FONT_SHORTHAND.Contains(shorthandExpression)) {
                ILogger logger = LoggerFactory.GetLogger(typeof(FontShorthandResolver));
                logger.Error(String.Format("The \"{0}\" value of CSS shorthand property \"font\" is not supported", shorthandExpression
                    ));
            }
            if (CssConstants.INITIAL.Equals(shorthandExpression) || CssConstants.INHERIT.Equals(shorthandExpression)) {
                return iText.IO.Util.JavaUtil.ArraysAsList(new CssDeclaration(CssConstants.FONT_STYLE, shorthandExpression
                    ), new CssDeclaration(CssConstants.FONT_VARIANT, shorthandExpression), new CssDeclaration(CssConstants
                    .FONT_WEIGHT, shorthandExpression), new CssDeclaration(CssConstants.FONT_SIZE, shorthandExpression), new 
                    CssDeclaration(CssConstants.LINE_HEIGHT, shorthandExpression), new CssDeclaration(CssConstants.FONT_FAMILY
                    , shorthandExpression));
            }
            String fontStyleValue = null;
            String fontVariantValue = null;
            String fontWeightValue = null;
            String fontSizeValue = null;
            String lineHeightValue = null;
            String fontFamilyValue = null;
            IList<String> properties = GetFontProperties(iText.IO.Util.StringUtil.ReplaceAll(shorthandExpression, "\\s*,\\s*"
                , ","));
            foreach (String value in properties) {
                int slashSymbolIndex = value.IndexOf('/');
                if (CssConstants.ITALIC.Equals(value) || CssConstants.OBLIQUE.Equals(value)) {
                    fontStyleValue = value;
                }
                else {
                    if (CssConstants.SMALL_CAPS.Equals(value)) {
                        fontVariantValue = value;
                    }
                    else {
                        if (FONT_WEIGHT_NOT_DEFAULT_VALUES.Contains(value)) {
                            fontWeightValue = value;
                        }
                        else {
                            if (slashSymbolIndex > 0) {
                                fontSizeValue = value.JSubstring(0, slashSymbolIndex);
                                lineHeightValue = value.JSubstring(slashSymbolIndex + 1, value.Length);
                            }
                            else {
                                if (FONT_SIZE_VALUES.Contains(value) || CssUtils.IsMetricValue(value) || CssUtils.IsNumericValue(value) ||
                                     CssUtils.IsRelativeValue(value)) {
                                    fontSizeValue = value;
                                }
                                else {
                                    fontFamilyValue = value;
                                }
                            }
                        }
                    }
                }
            }
            IList<CssDeclaration> cssDeclarations = iText.IO.Util.JavaUtil.ArraysAsList(new CssDeclaration(CssConstants
                .FONT_STYLE, fontStyleValue == null ? CssConstants.INITIAL : fontStyleValue), new CssDeclaration(CssConstants
                .FONT_VARIANT, fontVariantValue == null ? CssConstants.INITIAL : fontVariantValue), new CssDeclaration
                (CssConstants.FONT_WEIGHT, fontWeightValue == null ? CssConstants.INITIAL : fontWeightValue), new CssDeclaration
                (CssConstants.FONT_SIZE, fontSizeValue == null ? CssConstants.INITIAL : fontSizeValue), new CssDeclaration
                (CssConstants.LINE_HEIGHT, lineHeightValue == null ? CssConstants.INITIAL : lineHeightValue), new CssDeclaration
                (CssConstants.FONT_FAMILY, fontFamilyValue == null ? CssConstants.INITIAL : fontFamilyValue));
            return cssDeclarations;
        }

        private IList<String> GetFontProperties(String shorthandExpression) {
            bool doubleQuotesAreSpotted = false;
            bool singleQuoteIsSpotted = false;
            IList<String> properties = new List<String>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < shorthandExpression.Length; i++) {
                char currentChar = shorthandExpression[i];
                if (currentChar == '\"') {
                    doubleQuotesAreSpotted = !doubleQuotesAreSpotted;
                    sb.Append(currentChar);
                }
                else {
                    if (currentChar == '\'') {
                        singleQuoteIsSpotted = !singleQuoteIsSpotted;
                        sb.Append(currentChar);
                    }
                    else {
                        if (!doubleQuotesAreSpotted && !singleQuoteIsSpotted && iText.IO.Util.TextUtil.IsWhiteSpace(currentChar)) {
                            if (sb.Length > 0) {
                                properties.Add(sb.ToString());
                                sb = new StringBuilder();
                            }
                        }
                        else {
                            sb.Append(currentChar);
                        }
                    }
                }
            }
            if (sb.Length > 0) {
                properties.Add(sb.ToString());
            }
            return properties;
        }
    }
}
