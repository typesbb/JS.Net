using System;
using System.Diagnostics;
using JS.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void JExpression()
        {
            Assert.AreEqual(J.use.a = "a", @"a=""a""");
            Assert.AreEqual(J.syntax("abcd"), "abcd");
            Assert.AreEqual((Jsyntax)"abcd", @"""abcd""");
            Assert.AreEqual((Jsyntax)3, @"3");
            Assert.AreEqual((Jsyntax)false, @"false");

            Assert.AreEqual(J.syntax(new { name = "abcd", age = 20 }), @"{name:""abcd"",age:20}");

            var syntax = J.syntax("abcd");
            Assert.AreEqual(++syntax, @"++abcd");
            Assert.AreEqual(++J.use.i, @"++i");
            Assert.AreEqual(J.use.i++, @"i++");
            Assert.AreEqual(--J.use.i, @"--i");
            Assert.AreEqual(J.use.i--, @"i--");

            Assert.AreEqual(!J.syntax("abcd"), @"!abcd");
            Assert.AreEqual(~J.syntax("abcd"), @"~abcd");

            Assert.AreEqual(J.syntax("abcd") + J.syntax("efg"), @"abcd+efg");
            Assert.AreEqual(J.syntax("abcd") - J.syntax("efg"), @"abcd-efg");
            Assert.AreEqual(J.syntax("abcd") * J.syntax("efg"), @"abcd*efg");
            Assert.AreEqual(J.syntax("abcd") / J.syntax("efg"), @"abcd/efg");
            Assert.AreEqual(J.syntax("abcd") % J.syntax("efg"), @"abcd%efg");
            Assert.AreEqual(J.syntax("abcd") ^ J.syntax("efg"), @"abcd^efg");

            Assert.AreEqual(J.syntax("abcd") == J.syntax("efg"), @"abcd==efg");
            Assert.AreEqual(J.syntax("abcd") != J.syntax("efg"), @"abcd!=efg");
            Assert.AreEqual(J.syntax("abcd") > J.syntax("efg"), @"abcd>efg");
            Assert.AreEqual(J.syntax("abcd") >= J.syntax("efg"), @"abcd>=efg");
            Assert.AreEqual(J.syntax("abcd") < J.syntax("efg"), @"abcd<efg");
            Assert.AreEqual(J.syntax("abcd") <= J.syntax("efg"), @"abcd<=efg");
            Assert.AreEqual(J.and(J.syntax("abcd"), J.syntax("efg")), @"abcd&&efg");
            Assert.AreEqual(J.or(J.syntax("abcd"), J.syntax("efg")), @"abcd||efg");

            Assert.AreEqual(J.syntax("abcd") & J.syntax("efg"), @"abcd&efg");
            Assert.AreEqual(J.syntax("abcd") | J.syntax("efg"), @"abcd|efg");
            Assert.AreEqual(J.syntax("abcd") >> 2, @"abcd>>2");
            Assert.AreEqual(J.syntax("abcd") << 2, @"abcd<<2");

            Assert.AreEqual(J.use(J.use.a & J.use.b) != J.use.c, @"(a&b)!=c");
            var id = Guid.NewGuid();
            var str = J.set(J.use.str,
                J.use.str + @"<input id='" + id.ToString() + "' type='checkbox' value='" + J.use.dataItem.Call("aa")[J.use.i] +
                "' /><label for='" + id.ToString() + "'>" + J.use.dataItem.Call("aa")[J.use.i] + "</label>").ToString();
            if ((string)str == "")
            {

            }

            Assert.AreEqual(J.syntax("abcd").name, @"abcd.name");
            Assert.AreEqual(J.syntax("abcd").getName(), @"abcd.getName()");
            Assert.AreEqual(J.syntax("abcd").getName(J.syntax("efg")), @"abcd.getName(efg)");
            Assert.AreEqual(J.syntax("abcd").getName("efg"), @"abcd.getName(""efg"")");
            Assert.AreEqual(J.syntax("abcd")[1], @"abcd[1]");
            Assert.AreEqual(J.syntax("abcd")(J.syntax("efg")), @"abcd(efg)");
            Assert.AreEqual(J.syntax("abcd")("efg"), @"abcd(""efg"")");

            Assert.AreEqual(J.syntax("abcd").Call("efg"), @"abcd.efg");
            Assert.AreEqual(J.syntax("abcd").Call("efg", 2), @"abcd.efg(2)");
        }

        [TestMethod]
        public void JSyntax()
        {
            Assert.AreEqual(new JObject(), "new Object()");
            Assert.AreEqual(new JArray(), "new Array()");
            Assert.AreEqual(new JArray(5), "new Array(5)");
            Assert.AreEqual(new JArray(5, 6, 9), "new Array(5,6,9)");
            Assert.AreEqual(new JArray("a", "b", 9), @"new Array(""a"",""b"",9)");

            Assert.AreEqual(J.var("abcd"), "var abcd");
            Assert.AreEqual(J.var(J.use.abcd, 3), "var abcd=3");
            Assert.AreEqual(J.var(J.use.abcd).var(J.use.efg, J.use.node.Id).var(J.use.hi, new JArray()).var(J.use.jk), "var abcd,efg=node.Id,hi=new Array(),jk");
            Assert.AreEqual(J.var(J.use.abcd, J.or(J.use.a, J.use.b)), "var abcd=a||b");

            Assert.AreEqual(J.@return("abcd"), @"return ""abcd""");
            Assert.AreEqual(J.@return(new { id = "d60ccc72-23e2-e311-a9b3-2c59e5355b8f" }), @"return {id:""d60ccc72-23e2-e311-a9b3-2c59e5355b8f""}");
            Assert.AreEqual(J.@return(new Jfunction(J.use.e) { J.console.log("test%d", 2) }), @"return function(e){console.log(""test%d"",2);}");

            Assert.AreEqual(J.console.log("test%d", 1), @"console.log(""test%d"",1)");

            Assert.AreEqual(J.@return(
                new Jswitch(J.use.e) { 
                    new Jcase(1)
                    {
                        J.console.log("test%d", 1),
                        J.@break
                    },
                    new Jcase(2)
                    {
                        J.console.log("test%d", 2),
                        J.@break
                    },
                    new Jdefault()
                    {
                        J.console.log("test%d", 0),
                    }
                }), @"return switch(e){case 1:console.log(""test%d"",1);break;case 2:console.log(""test%d"",2);break;default:console.log(""test%d"",0);}");

            Assert.AreEqual(J.@return(
                new Jdowhile(J.use.e > 0)
                {
                    J.console.log("test%d", 1),
                }
            ), @"return do{console.log(""test%d"",1);}while(e>0)");

            Assert.AreEqual(J.jquery("bb"), @"$(""bb"")");
            Assert.AreEqual(J.jquery("#bb"), @"$(""#bb"")");
            Assert.AreEqual(J.jqueryById("bb"), @"$(""#bb"")");
            Assert.AreEqual(J.jqueryByClass("bb"), @"$("".bb"")");
            Assert.AreEqual(J.jquery("bb").bind("key", new Jfunction(J.use.e) { J.console.log("123") }), @"$(""bb"").bind(""key"",function(e){console.log(""123"");})");
            Assert.AreEqual(J.jquery("bb").data("aa").bind("key", new Jfunction(J.use.e) { J.console.log("abc") }), @"$(""bb"").data(""aa"").bind(""key"",function(e){console.log(""abc"");})");
        }

        [TestMethod]
        public void JPerformance()
        {
            Stopwatch _addWatch = new Stopwatch();
            _addWatch.Start();

            var a = new Jbody
            {
                 new Jswitch(J.use.e.item.textContent)
                {
                    new Jcase("组织"){J.var(J.use.addNodeType, J.use.NodeType.Org),J.@break},
                    new Jcase("生产线"){J.var(J.use.addNodeType, J.use.NodeType.Line),J.@break},
                    new Jcase("区域"){J.var(J.use.addNodeType, J.use.NodeType.Area),J.@break},
                    new Jcase("设备"){J.var(J.use.addNodeType, J.use.NodeType.Device),J.@break},
                    new Jcase("分部设备"){J.var(J.use.addNodeType, J.use.NodeType.Component),J.@break},
                    new Jcase("零部件"){J.var(J.use.addNodeType, J.use.NodeType.Part),J.@break},
                    new Jcase("零部件细分"){J.var(J.use.addNodeType, J.use.NodeType.Parts),J.@break},
                    new Jcase("观察量"){J.var(J.use.addNodeType, J.use.NodeType.Observe),J.@break},
                    new Jcase("转速量"){J.var(J.use.addNodeType, J.use.NodeType.Temperature),J.@break},
                    new Jcase("温度量"){J.var(J.use.addNodeType, J.use.NodeType.Speed),J.@break},
                    new Jcase("工艺量"){J.var(J.use.addNodeType, J.use.NodeType.Technique),J.@break},
                    new Jcase("动态量"){J.var(J.use.addNodeType, J.use.NodeType.Dynamic),J.@break},
                    new Jcase("测量定义"){J.var(J.use.addNodeType, J.use.NodeType.Wave),J.@break},
                    new Jdefault{J.var(J.use.addNodeType, -1),J.@break}
                },
                new Jif(J.use.addNodeType!=-1)
                {
                    J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use.JView.showWindow(J.use.UIConfigTree.ViewId, "添加"+J.use.getNameByNodeType(J.use.addNodeType), "")
                },
                new Jelse_if(J.use.e.item.textContent=="删除")
                {
                    J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use._deviceTree.DataSource.remote.Delete(new {J.use.selectNode.ID,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                            J.use._deviceTree.RemoveNode(J.use._deviceTree.FindById(J.use.e.Data[0].ID)),
                        }})
                },
                new Jelse_if(J.use.e.item.textContent=="编辑")
                {
                    J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.var(J.use.title, "编辑" + J.use.getNameByNodeType(J.use.selectNode.NodeType)),
                    J.use.JView.showWindow(J.use.UIConfigTree.ViewId, J.use.title, "")
                },
                 new Jelse_if(J.use.e.item.textContent=="剪切")
                {
                     J.debugger,
                     J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                     new Jif(!J.use.selectNode.ParentID)
                     {
                         J.alert("根节点禁止此操作！"),
                     },
                     new Jelse
                     {
                        J.set(J.use.sourceNode,J.use.selectNode),
                        J.set(J.use.IsCut,true),
                     },
                },
                 new Jelse_if(J.use.e.item.textContent=="复制")
                {
                    J.debugger,
                     J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                     new Jif(!J.use.selectNode.ParentID)
                     {
                         J.alert("根节点禁止此操作！"),
                     },
                     new Jelse
                     {
                        J.set(J.use.sourceNode,J.use.selectNode),
                        J.set(J.use.IsCut,false),
                     },
                },
                 new Jelse_if(J.use.e.item.textContent=="粘贴")
                {
                      J.debugger,
                    new Jif(J.use.sourceNode!=J.undefined)
                   {                          
                     J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                     J.use._deviceTree.DataSource.remote.Paste(new {SourceID=J.use.sourceNode.ID,SourceNodeType=J.use.sourceNode.NodeType,J.use.selectNode.ID,J.use.IsCut,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                             J.set(J.use.sourceNode,J.undefined),
                           //J.var(J.use.tree,TreeView.GetJQuery(_deviceTree.Id)),
                           //J.use.tree.append(J.use.sourceNode,J.use.selectNode),
                           
                        }}) ,  
                    }, 
                },
                 new Jelse_if(J.use.e.item.textContent=="置为生产线")
                {
                    J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use._deviceTree.DataSource.remote.SetToLine(new {J.use.selectNode.ID,J.use.selectNode.ParentID,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                            J.use.selectNode.set("NodeType", J.use.NodeType.Line),
                            J.use.selectNode.set("ImageName", "Line"+J.use.selectNode.AlarmLevel),
                        }}) ,     
                },
                 new Jelse_if(J.use.e.item.textContent=="置为组织")
                {
                     J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use._deviceTree.DataSource.remote.SetToOrg(new {J.use.selectNode.ID,J.use.selectNode.ParentID,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                            J.use.selectNode.set("NodeType", J.use.NodeType.Org),
                            J.use.selectNode.set("ImageName", "Org"+J.use.selectNode.AlarmLevel),
                        }}) ,     
                },
                 new Jelse_if(J.use.e.item.textContent=="置为区域")
                {
                     J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use._deviceTree.DataSource.remote.SetToArea(new {J.use.selectNode.ID,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                            J.use.selectNode.set("NodeType", J.use.NodeType.Area),
                            J.use.selectNode.set("ImageName", "Area"+J.use.selectNode.AlarmLevel),
                        }}) ,          
                },
                 new Jelse_if(J.use.e.item.textContent=="置为零部件")
                {
                       J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use._deviceTree.DataSource.remote.SetToPart(new {J.use.selectNode.ID,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                            J.use.selectNode.set("NodeType", J.use.NodeType.Part),
                            J.use.selectNode.set("ImageName", "Part"+J.use.selectNode.AlarmLevel),
                        }}) ,          
                },
                 new Jelse_if(J.use.e.item.textContent=="置为分部设备")
                {
                      J.var(J.use.selectNode, J.use._deviceTree.GetNodeData(J.use._deviceTree.SelectNode())),
                    J.use._deviceTree.DataSource.remote.SetToComponent(new {J.use.selectNode.ID,J.use.selectNode.NodeType}).then(new Jfunction(J.use.e){
                        new Jif(J.use.e.Success)
                        {
                            J.use.selectNode.set("NodeType", J.use.NodeType.Component),
                            J.use.selectNode.set("ImageName", "Component"+J.use.selectNode.AlarmLevel),
                        }}) ,          
                },
                 new Jelse_if(J.use.e.item.textContent=="文档")
                {
                },
                 new Jelse_if(J.use.e.item.textContent=="日志")
                {
                },
            };
            _addWatch.Stop();
            var str = a.ToString();

        }
    }
}
