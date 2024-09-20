# JS.Net
A .net lib that you can writing javascript use c#.

## Sample

| C# Code | output js |
|---------|-----------|
|J.use.a.Call("b", 1)| a.b(1)|
|J.use.a = "a"| a="a"    |
|J.syntax("abcd")| abcd       |
|(Jsyntax)"abcd"| "abcd"       |
|(Jsyntax)3|  3      |
|(Jsyntax)false| false       |
|J.use.fn["a"] = new Jfunction(J.use.ct) { }|fn["a"]=function(ct){}        |
|J.syntax(new { name = "abcd", age = 20 })| {name:"abcd",age:20}       |
|++J.syntax("abcd")| ++abcd       |
|++J.use.i| ++i       |
|J.use.i++| i++       |
|--J.use.i| --i       |
|J.use.i--| i--       |
|!J.syntax("abcd")| !abcd       |
|~J.syntax("abcd")| ~abcd       |
|J.syntax("abcd") + J.syntax("efg")| abcd+efg       |
|J.syntax("abcd") - J.syntax("efg")| abcd-efg       |
|J.syntax("abcd") * J.syntax("efg")| abcd*efg       |
|J.syntax("abcd") / J.syntax("efg")| abcd/efg       |
|J.syntax("abcd") % J.syntax("efg")| abcd%efg       |
|J.syntax("abcd") ^ J.syntax("efg")| abcd^efg       |
|J.syntax("abcd") == J.syntax("efg")| abcd==efg       |
|J.syntax("abcd") != J.syntax("efg")| abcd!=efg       |
|J.syntax("abcd") > J.syntax("efg")| abcd>efg       |
|J.syntax("abcd") >= J.syntax("efg")| abcd>=efg       |
|J.syntax("abcd") < J.syntax("efg")| abcd<efg       |
|J.syntax("abcd") <= J.syntax("efg")| abcd<=efg       |
|J.and(J.syntax("abcd"), J.syntax("efg"))| abcd&&efg       |
|J.or(J.syntax("abcd"), J.syntax("efg"))| abcd||efg       |
|!J.use.abcd| !abcd       |
|~J.use.abcd| ~abcd       |
|J.use.abcd + J.use.efg| abcd+efg       |
|J.use.abcd - J.use.efg| abcd-efg       |
|J.use.abcd * J.use.efg| abcd*efg       |
|J.use.abcd / J.use.efg| abcd/efg       |
|J.use.abcd % J.use.efg| abcd%efg       |
|J.use.abcd ^ J.use.efg| abcd^efg       |
|J.use.abcd == J.use.efg| abcd==efg       |
|J.use.abcd != J.use.efg| abcd!=efg       |
|J.use.abcd > J.use.efg| abcd>efg       |
|J.use.abcd >= J.use.efg| abcd>=efg       |
|J.use.abcd < J.use.efg| abcd<efg       |
|J.use.abcd <= J.use.efg| abcd<=efg       |
|J.and(J.use.abcd, J.use.efg)| abcd&&efg       |
|J.or(J.use.abcd, J.use.efg)| abcd||efg       |
|J.syntax("abcd") & J.syntax("efg")| abcd&efg       |
|J.syntax("abcd") | J.syntax("efg")| abcd|efg       |
|J.syntax("abcd") >> 2| abcd>>2       |
|J.syntax("abcd") << 2| abcd<<2       |
|J.use(J.use.a & J.use.b) != J.use.c| (a&b)!=c       |
|J.syntax("abcd").name| abcd.name       |
|J.syntax("abcd").getName()| abcd.getName()       |
|J.syntax("abcd").getName(J.syntax("efg"))| abcd.getName(efg)       |
|J.syntax("abcd").getName("efg")| abcd.getName("efg")       |
|J.syntax("abcd")[1]|abcd[1]        |
|J.syntax("abcd")(J.syntax("efg"))| abcd(efg)       |
|J.syntax("abcd")("efg")| abcd("efg")       |
|J.syntax("abcd").Call("efg")| abcd.efg       |
|J.syntax("abcd").Call("efg", 2)| abcd.efg(2)       |
|new JObject()| new Object()       |
|new JArray()| new Array()       |
|new JArray(5)| new Array(5)       |
|new JArray(5, 6, 9)| new Array(5,6,9)       |
|new JArray("a", "b", 9)| new Array("a","b",9)       |
|J.var.i| var i       |
|J.var.i = 3| var i=3       |
|new Jbody { J.debugger, (J.var.i = 1), (J.var.j = false), (J.var.k = new JArray()) }| debugger;var i=1,j=false,k=new Array();       |
|J.var.abcd = J.or(J.use.a, J.use.b)| var abcd=a||b       |
|new Jbody { new Jfor(J.var.i = 0, J.use.i < 10, J.use.i++) { } }| for(var i=0;i<10;i++){}       |
|new Jbody { new Jfor(J.var.i = 0, J.use.i < 10, ++J.use.i) { } }| for(var i=0;i<10;++i){}       |
|J.@return(J.use.i++)| return i++       |
|J.@return(++J.use.i)| return ++i       |
|J.@return("abcd")| return "abcd"       |
|J.@return(new Jfunction(J.use.e) { J.console.log("test%d", 2) })| return function(e){console.log("test%d",2);}       |
|J.console.log("test%d", 1)| console.log("test%d",1)       |
|J.@return(<br>new Jswitch(J.use.e) { <br>new Jcase(1)<br>{<br>J.console.log("test%d", 1),<br>J.@break<br>},<br>new Jcase(2)<br>{<br>J.console.log("test%d", 2),<br>J.@break<br>},<br>new Jdefault()<br>{<br>J.console.log("test%d", 0),<br>}<br>})| return switch(e){case 1:console.log("test%d",1);break;case 2:console.log("test%d",2);break;default:console.log("test%d",0);}|
|J.@return(<br>new Jdowhile(J.use.e > 0)<br>{<br>J.console.log("test%d", 1),<br>}<br>)| return do{console.log("test%d",1);}while(e>0)       |
|J.jquery("bb")| $("bb")       |
|J.jquery("#bb")| $("#bb")       |
|J.jqueryById("bb")| $("#bb")       |
|J.jqueryByClass("bb")| $(".bb")       |
|J.jquery("bb").bind("key", new Jfunction(J.use.e) { J.console.log("123") })| $("bb").bind("key",function(e){console.log("123");})       |
|J.jquery("bb").data("aa").bind("key", new Jfunction(J.use.e) { J.console.log("abc") })|$("bb").data("aa").bind("key",function(e){console.log("abc"");})        |
