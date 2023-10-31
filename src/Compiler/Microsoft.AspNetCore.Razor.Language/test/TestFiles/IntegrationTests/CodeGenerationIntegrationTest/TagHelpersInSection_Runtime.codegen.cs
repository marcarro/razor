﻿#pragma checksum "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "66a1fd3b9b40b304a7d1864845d2b7ab0f5f5c8a98d810ff8f97a245644661a9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(Microsoft.AspNetCore.Razor.Language.IntegrationTests.TestFiles.TestFiles_IntegrationTests_CodeGenerationIntegrationTest_TagHelpersInSection_Runtime), @"default", @"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml")]
namespace Microsoft.AspNetCore.Razor.Language.IntegrationTests.TestFiles
{
    #line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"66a1fd3b9b40b304a7d1864845d2b7ab0f5f5c8a98d810ff8f97a245644661a9", @"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml")]
    public class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_TagHelpersInSection_Runtime
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::TestNamespace.MyTagHelper __TestNamespace_MyTagHelper;
        private global::TestNamespace.NestedTagHelper __TestNamespace_NestedTagHelper;
        #pragma warning disable 1998
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml"
  
    var code = "some code";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("MySection", async() => {
                WriteLiteral("\r\n    <div>\r\n        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("mytaghelper", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "test", async() => {
                    WriteLiteral("\r\n            In None ContentBehavior.\r\n            ");
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("nestedtaghelper", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "test", async() => {
                        WriteLiteral("Some buffered values with ");
#nullable restore
#line (11,57)-(11,61) 6 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml"
Write(code);

#line default
#line hidden
#nullable disable
                    }
                    );
                    __TestNamespace_NestedTagHelper = CreateTagHelper<global::TestNamespace.NestedTagHelper>();
                    __tagHelperExecutionContext.Add(__TestNamespace_NestedTagHelper);
                    await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                    if (!__tagHelperExecutionContext.Output.IsContentModified)
                    {
                        await __tagHelperExecutionContext.SetOutputContentAsync();
                    }
                    Write(__tagHelperExecutionContext.Output);
                    __tagHelperExecutionContext = __tagHelperScopeManager.End();
                    WriteLiteral("\r\n        ");
                }
                );
                __TestNamespace_MyTagHelper = CreateTagHelper<global::TestNamespace.MyTagHelper>();
                __tagHelperExecutionContext.Add(__TestNamespace_MyTagHelper);
                BeginWriteTagHelperAttribute();
                WriteLiteral("Current Time: ");
#nullable restore
#line (9,52)-(9,64) 13 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml"
WriteLiteral(DateTime.Now);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __TestNamespace_MyTagHelper.BoundProperty = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("boundproperty", __TestNamespace_MyTagHelper.BoundProperty, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "unboundproperty", 3, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                AddHtmlAttributeValue("", 188, "Current", 188, 7, true);
                AddHtmlAttributeValue(" ", 195, "Time:", 196, 6, true);
#nullable restore
#line (9,96)-(9,110) 32 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/TagHelpersInSection.cshtml"
AddHtmlAttributeValue(" ", 201, DateTime.Now, 202, 13, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    </div>\r\n\r\n");
                WriteLiteral("\r\n        <div>\r\n            ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("nestedtaghelper", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "test", async() => {
                }
                );
                __TestNamespace_NestedTagHelper = CreateTagHelper<global::TestNamespace.NestedTagHelper>();
                __tagHelperExecutionContext.Add(__TestNamespace_NestedTagHelper);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n        </div>\r\n    ");
            }
            );
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
