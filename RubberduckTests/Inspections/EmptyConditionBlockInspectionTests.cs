﻿using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using RubberduckTests.Mocks;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Inspections.Resources;
using Rubberduck.Inspections.QuickFixes;

namespace RubberduckTests.Inspections
{
    [TestClass]
    class EmptyConditionBlockInspectionTests
    {
        [TestMethod]
        [TestCategory("Inspections")]
        public void InspectionType()
        {
            var inspection = new EmptyConditionBlockInspection(null);
            Assert.AreEqual(CodeInspectionType.CodeQualityIssues, inspection.InspectionType);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void InspectionName()
        {
            const string inspectionName = nameof(EmptyConditionBlockInspection);

            var inspection = new EmptyConditionBlockInspection(null);

            Assert.AreEqual(inspectionName, inspection.Name);
        }

        #region EmptyIfBlock
        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyElseIfBlock()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    ElseIf False Then
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(2, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_ElseBlock()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptySingleLineIfStmt()
        {
            const string inputCode =
@"Sub Foo()
    If True Then Else Bar
End Sub

Sub Bar()
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_HasNonEmptyElseBlock()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        Dim d
        d = 0
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_HasQuoteComment()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
        ' Im a comment
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_HasRemComment()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
        Rem Im a comment
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_HasVariableDeclaration()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
        Dim d
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_HasConstDeclaration()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
        Const c = 0
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_FiresOnEmptyIfBlock_HasWhitespace()
        {
            const string inputCode =
@"Sub Foo()
    If True Then

    	
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_IfBlockHasExecutableStatement()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
        Dim d
        d = 0
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.IsFalse(inspectionResults.Any());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_SingleLineIfBlockHasExecutableStatement()
        {
            const string inputCode =
@"Sub Foo()
    If True Then Bar
End Sub

Sub Bar()
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.IsFalse(inspectionResults.Any());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyIfBlock_IfAndElseIfBlockHaveExecutableStatement()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
        Dim d
        d = 0
    ElseIf False Then
        Dim b
        b = 0
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyConditionBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var inspectionResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.IsFalse(inspectionResults.Any());
        }
        #endregion

        #region EmptyElseBlock
        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_DoesntFireOnEmptyIfBlock()
        {
            const string inputcode =
@"Sub Foo()
    If True Then
    EndIf
End Sub";
            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputcode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 0;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasNoContent()
        {
            const string inputcode =
@"Sub Foo()
    If True Then
    Else
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputcode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasQuoteComment()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        'Some Comment
    End If
End Sub";
            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasRemComment()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        Rem Some Comment
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasVariableDeclaration()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        Dim d
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasConstDeclaration()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        Const c = 0
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasWhitespace()
        {
            const string inputcode =
@"Sub Foo()
    If True Then
    Else
    
    
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputcode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasDeclarationStatement()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        Dim d
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 1;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_HasExecutableStatement()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
        Dim d
        d = 0
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;
            const int expectedCount = 0;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_QuickFixRemovesElse()
        {
            const string inputCode =
@"Sub Foo()
    If True Then
    Else
    End If
End Sub";

            const string expectedCode =
@"Sub Foo()
    If True Then
    End If
End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            new RemoveEmptyElseBlockQuickFix(state).Fix(actualResults.First());

            string actualRewrite = state.GetRewriter(component).GetText();

            Assert.AreEqual(expectedCode, actualRewrite);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyElseBlock_QuickFixRemoveInLineIfThenElse()
        {
            const string inputCode =
    @"Sub Foo()
        If True Then Else End If
    End Sub";

            const string expectedCode =
    @"If True Then End If
    End Sub";

            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyElseBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            new RemoveEmptyElseBlockQuickFix(state).Fix(actualResults.First());
            
            string actualRewrite = state.GetRewriter(component).GetText();

            Assert.AreEqual(expectedCode, actualRewrite);
        }
        #endregion
    }
}
