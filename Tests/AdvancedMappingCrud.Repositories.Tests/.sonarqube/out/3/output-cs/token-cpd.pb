�&
|D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Startup.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 5
,5 6
Version7 >
=? @
$strA F
)F G
]G H
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
) 
] 
public 

class 
Startup 
{ 
public 
Startup 
( 
IConfiguration %
configuration& 3
)3 4
{ 	
Configuration 
= 
configuration )
;) *
} 	
public!! 
IConfiguration!! 
Configuration!! +
{!!, -
get!!. 1
;!!1 2
}!!3 4
public## 
void## 
ConfigureServices## %
(##% &
IServiceCollection##& 8
services##9 A
)##A B
{$$ 	
services%% 
.%% 
AddControllers%% #
(%%# $
opt&& 
=>&& 
{'' 
opt(( 
.(( 
Filters(( 
.((  
Add((  #
<((# $
ExceptionFilter(($ 3
>((3 4
(((4 5
)((5 6
;((6 7
})) 
))) 
.** 
AddOData** 
(** 
options** 
=>**  
{++ 
options,, 
.,, 
Filter,, 
(,, 
),,  
.,,  !
OrderBy,,! (
(,,( )
),,) *
.,,* +
Expand,,+ 1
(,,1 2
),,2 3
.,,3 4
	SetMaxTop,,4 =
(,,= >
$num,,> A
),,A B
;,,B C
}-- 
)-- 
;-- 
services.. 
... 
AddApplication.. #
(..# $
Configuration..$ 1
)..1 2
;..2 3
services// 
.// (
ConfigureApplicationSecurity// 1
(//1 2
Configuration//2 ?
)//? @
;//@ A
services00 
.00 !
ConfigureHealthChecks00 *
(00* +
Configuration00+ 8
)008 9
;009 :
services11 
.11 #
ConfigureProblemDetails11 ,
(11, -
)11- .
;11. /
services22 
.22 "
ConfigureApiVersioning22 +
(22+ ,
)22, -
;22- .
services33 
.33 
AddInfrastructure33 &
(33& '
Configuration33' 4
)334 5
;335 6
services44 
.44 
ConfigureSwagger44 %
(44% &
Configuration44& 3
)443 4
;444 5
}55 	
public88 
void88 
	Configure88 
(88 
IApplicationBuilder88 1
app882 5
,885 6
IWebHostEnvironment887 J
env88K N
)88N O
{99 	
if:: 
(:: 
env:: 
.:: 
IsDevelopment:: !
(::! "
)::" #
)::# $
{;; 
app<< 
.<< %
UseDeveloperExceptionPage<< -
(<<- .
)<<. /
;<</ 0
}== 
app>> 
.>> $
UseSerilogRequestLogging>> (
(>>( )
)>>) *
;>>* +
app?? 
.?? 
UseExceptionHandler?? #
(??# $
)??$ %
;??% &
app@@ 
.@@ 
UseHttpsRedirection@@ #
(@@# $
)@@$ %
;@@% &
appAA 
.AA 

UseRoutingAA 
(AA 
)AA 
;AA 
appBB 
.BB 
UseAuthenticationBB !
(BB! "
)BB" #
;BB# $
appCC 
.CC 
UseAuthorizationCC  
(CC  !
)CC! "
;CC" #
appDD 
.DD 
UseEndpointsDD 
(DD 
	endpointsDD &
=>DD' )
{EE 
	endpointsFF 
.FF "
MapDefaultHealthChecksFF 0
(FF0 1
)FF1 2
;FF2 3
	endpointsGG 
.GG 
MapControllersGG (
(GG( )
)GG) *
;GG* +
}HH 
)HH 
;HH 
appII 
.II 
UseSwashbuckleII 
(II 
ConfigurationII ,
)II, -
;II- .
}JJ 	
}KK 
}LL �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Services\CurrentUserService.cs
[		 
assembly		 	
:			 
 
DefaultIntentManaged		 
(		  
Mode		  $
.		$ %
Fully		% *
)		* +
]		+ ,
[

 
assembly

 	
:

	 

IntentTemplate

 
(

 
$str

 J
,

J K
Version

L S
=

T U
$str

V [
)

[ \
]

\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Services5 =
{ 
public 

class 
CurrentUserService #
:$ %
ICurrentUserService& 9
{ 
private 
readonly 
ClaimsPrincipal (
?( )
_claimsPrincipal* :
;: ;
private 
readonly !
IAuthorizationService .!
_authorizationService/ D
;D E
public 
CurrentUserService !
(! " 
IHttpContextAccessor" 6
httpContextAccessor7 J
,J K!
IAuthorizationServiceL a 
authorizationServiceb v
)v w
{ 	
_claimsPrincipal 
= 
httpContextAccessor 2
?2 3
.3 4
HttpContext4 ?
?? @
.@ A
UserA E
;E F!
_authorizationService !
=" # 
authorizationService$ 8
;8 9
} 	
public 
string 
? 
UserId 
=>  
_claimsPrincipal! 1
?1 2
.2 3
	FindFirst3 <
(< =
JwtClaimTypes= J
.J K
SubjectK R
)R S
?S T
.T U
ValueU Z
;Z [
public 
string 
? 
UserName 
=>  "
_claimsPrincipal# 3
?3 4
.4 5
	FindFirst5 >
(> ?
JwtClaimTypes? L
.L M
NameM Q
)Q R
?R S
.S T
ValueT Y
;Y Z
public 
async 
Task 
< 
bool 
> 
AuthorizeAsync  .
(. /
string/ 5
policy6 <
)< =
{ 	
if 
( 
_claimsPrincipal  
==! #
null$ (
)( )
{ 
return   
false   
;   
}!! 
return## 
(## 
await## !
_authorizationService## /
.##/ 0
AuthorizeAsync##0 >
(##> ?
_claimsPrincipal##? O
,##O P
policy##Q W
)##W X
)##X Y
.##Y Z
	Succeeded##Z c
;##c d
}$$ 	
public&& 
async&& 
Task&& 
<&& 
bool&& 
>&& 
IsInRoleAsync&&  -
(&&- .
string&&. 4
role&&5 9
)&&9 :
{'' 	
return(( 
await(( 
Task(( 
.(( 

FromResult(( (
(((( )
_claimsPrincipal(() 9
?((9 :
.((: ;
IsInRole((; C
(((C D
role((D H
)((H I
??((J L
false((M R
)((R S
;((S T
})) 	
}** 
}++ �
|D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Program.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[		 
assembly		 	
:			 

IntentTemplate		 
(		 
$str		 5
,		5 6
Version		7 >
=		? @
$str		A F
)		F G
]		G H
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
{ 
public 

class 
Program 
{ 
public 
static 
void 
Main 
(  
string  &
[& '
]' (
args) -
)- .
{ 	
Log 
. 
Logger 
= 
new 
LoggerConfiguration 0
(0 1
)1 2
. 
MinimumLevel 
. 
Override &
(& '
$str' 2
,2 3
LogEventLevel4 A
.A B
InformationB M
)M N
. 
Enrich 
. 
FromLogContext &
(& '
)' (
. 
WriteTo 
. 
Console  
(  !
)! "
. !
CreateBootstrapLogger &
(& '
)' (
;( )
try 
{ 
Log 
. 
Information 
(  
$str  3
)3 4
;4 5
CreateHostBuilder !
(! "
args" &
)& '
.' (
Build( -
(- .
). /
./ 0
Run0 3
(3 4
)4 5
;5 6
} 
catch 
( 
	Exception 
ex 
)  
{ 
Log 
. 
Fatal 
( 
ex 
, 
$str <
)< =
;= >
} 
finally   
{!! 
Log"" 
."" 
CloseAndFlush"" !
(""! "
)""" #
;""# $
}## 
}$$ 	
public&& 
static&& 
IHostBuilder&& "
CreateHostBuilder&&# 4
(&&4 5
string&&5 ;
[&&; <
]&&< =
args&&> B
)&&B C
=>&&D F
Host'' 
.''  
CreateDefaultBuilder'' %
(''% &
args''& *
)''* +
.(( 

UseSerilog(( 
((( 
((( 
context(( $
,(($ %
services((& .
,((. /
configuration((0 =
)((= >
=>((? A
configuration((B O
.)) 
ReadFrom)) 
.)) 
Configuration)) +
())+ ,
context)), 3
.))3 4
Configuration))4 A
)))A B
.** 
ReadFrom** 
.** 
Services** &
(**& '
services**' /
)**/ 0
)**0 1
.++ $
ConfigureWebHostDefaults++ )
(++) *

webBuilder++* 4
=>++5 7
{,, 

webBuilder-- 
.-- 

UseStartup-- )
<--) *
Startup--* 1
>--1 2
(--2 3
)--3 4
;--4 5
}.. 
).. 
;.. 
}// 
}00 �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\TypeSchemaFilter.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace

 	
AdvancedMappingCrud


 
.

 
Repositories

 *
.

* +
Tests

+ 0
.

0 1
Api

1 4
.

4 5
Filters

5 <
{ 
public 

class 
TypeSchemaFilter !
:" #
ISchemaFilter$ 1
{ 
public 
void 
Apply 
( 
OpenApiSchema '
schema( .
,. /
SchemaFilterContext0 C
contextD K
)K L
{ 	
if 
( 
context 
. 
Type 
== 
typeof  &
(& '
TimeSpan' /
)/ 0
||1 3
context4 ;
.; <
Type< @
==A C
typeofD J
(J K
TimeSpanK S
?S T
)T U
)U V
{ 
schema 
. 
Example 
=  
new! $
OpenApiString% 2
(2 3
$str3 =
)= >
;> ?
schema 
. 
Type 
= 
$str &
;& '
} 
if 
( 
context 
. 
Type 
== 
typeof  &
(& '
DateOnly' /
)/ 0
||1 3
context4 ;
.; <
Type< @
==A C
typeofD J
(J K
DateOnlyK S
?S T
)T U
)U V
{ 
schema 
. 
Example 
=  
new! $
OpenApiString% 2
(2 3
DateTime3 ;
.; <
Today< A
.A B
ToStringB J
(J K
$strK W
)W X
)X Y
;Y Z
schema 
. 
Type 
= 
$str &
;& '
} 
} 	
} 
} �,
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\ODataQueryFilter.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Filters5 <
{ 
public 

class 
ODataQueryFilter !
:" #
IOperationFilter$ 4
{ 
public 
void 
Apply 
( 
OpenApiOperation *
	operation+ 4
,4 5"
OperationFilterContext6 L
contextM T
)T U
{ 	
var  
hasODataQueryOptions $
=% &
Array' ,
., -
Exists- 3
(3 4
context4 ;
.; <

MethodInfo< F
.F G
GetParametersG T
(T U
)U V
,V W"
MatchODataQueryOptionsX n
)n o
;o p
if 
( 
!  
hasODataQueryOptions %
)% &
{ 
return 
; 
} 
int 
index 
= 
context 
.  

MethodInfo  *
.* +
GetParameters+ 8
(8 9
)9 :
.  !
Select! '
(' (
(( )
param) .
,. /
idx0 3
)3 4
=>5 7
new8 ;
{< =
Param> C
=D E
paramF K
,K L
IndexM R
=S T
idxU X
}Y Z
)Z [
.  !
FirstOrDefault! /
(/ 0
x0 1
=>2 4"
MatchODataQueryOptions5 K
(K L
xL M
.M N
ParamN S
)S T
)T U
?U V
.V W
IndexW \
??] _
-` a
$numa b
;b c
var 
	parameter 
= 
	operation %
.% &

Parameters& 0
[0 1
index1 6
]6 7
;7 8
if!! 
(!! 
	parameter!! 
==!! 
null!! !
)!!! "
{"" 
return## 
;## 
}$$ 
	operation&& 
.&& 

Parameters&&  
.&&  !
Remove&&! '
(&&' (
	parameter&&( 1
)&&1 2
;&&2 3
	operation'' 
.'' 

Parameters''  
.''  !
Add''! $
(''$ %
OdataParameter''% 3
(''3 4
$str''4 =
,''= >
$str''? a
)''a b
)''b c
;''c d
	operation(( 
.(( 

Parameters((  
.((  !
Add((! $
((($ %
OdataParameter((% 3
(((3 4
$str((4 :
,((: ;
$str((< g
)((g h
)((h i
;((i j
	operation)) 
.)) 

Parameters))  
.))  !
Add))! $
())$ %
OdataParameter))% 3
())3 4
$str))4 ;
,)); <
$str))= l
)))l m
)))m n
;))n o
	operation** 
.** 

Parameters**  
.**  !
Add**! $
(**$ %
OdataParameter**% 3
(**3 4
$str**4 =
,**= >
$str	**? �
)
**� �
)
**� �
;
**� �
	operation++ 
.++ 

Parameters++  
.++  !
Add++! $
(++$ %
OdataParameter++% 3
(++3 4
$str++4 >
,++> ?
$str	++@ �
)
++� �
)
++� �
;
++� �
},, 	
private.. 
static.. 
bool.. "
MatchODataQueryOptions.. 2
(..2 3
ParameterInfo..3 @
	parameter..A J
)..J K
{// 	
return00 
	parameter00 
.00 
ParameterType00 *
.00* +
IsGenericType00+ 8
&&009 ;
	parameter11 
.11 
ParameterType11 '
.11' ($
GetGenericTypeDefinition11( @
(11@ A
)11A B
==11C E
typeof11F L
(11L M
ODataQueryOptions11M ^
<11^ _
>11_ `
)11` a
;11a b
}22 	
private44 
static44 
OpenApiParameter44 '
OdataParameter44( 6
(446 7
string447 =
name44> B
,44B C
string44D J
description44K V
)44V W
{55 	
return66 
new66 
(66 
)66 
{77 
Name88 
=88 
name88 
,88 
Description99 
=99 
description99 )
,99) *
Required:: 
=:: 
false::  
,::  !
Schema;; 
=;; 
new;; 
OpenApiSchema;; *
{;;+ ,
Type;;- 1
=;;2 3
$str;;4 <
};;= >
,;;> ?
In<< 
=<< 
ParameterLocation<< &
.<<& '
Query<<' ,
}== 
;== 
}>> 	
}?? 
}@@ �*
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\ExceptionFilter.cs
[

 
assembly

 	
:

	 
 
DefaultIntentManaged

 
(

  
Mode

  $
.

$ %
Fully

% *
)

* +
]

+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str I
,I J
VersionK R
=S T
$strU Z
)Z [
][ \
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Filters5 <
{ 
public 

class 
ExceptionFilter  
:! "
	Microsoft# ,
., -

AspNetCore- 7
.7 8
Mvc8 ;
.; <
Filters< C
.C D
IExceptionFilterD T
{ 
public 
void 
OnException 
(  
ExceptionContext  0
context1 8
)8 9
{ 	
switch 
( 
context 
. 
	Exception %
)% &
{ 
case 
ValidationException (
	exception) 2
:2 3
foreach 
( 
var  
error! &
in' )
	exception* 3
.3 4
Errors4 :
): ;
{ 
context 
.  

ModelState  *
.* +
AddModelError+ 8
(8 9
error9 >
.> ?
PropertyName? K
,K L
errorM R
.R S
ErrorMessageS _
)_ `
;` a
} 
context 
. 
Result "
=# $
new% ("
BadRequestObjectResult) ?
(? @
new@ C$
ValidationProblemDetailsD \
(\ ]
context] d
.d e

ModelStatee o
)o p
)p q
. !
AddContextInformation *
(* +
context+ 2
)2 3
;3 4
context 
. 
ExceptionHandled ,
=- .
true/ 3
;3 4
break 
; 
case $
ForbiddenAccessException -
:- .
context   
.   
Result   "
=  # $
new  % (
ForbidResult  ) 5
(  5 6
)  6 7
;  7 8
context!! 
.!! 
ExceptionHandled!! ,
=!!- .
true!!/ 3
;!!3 4
break"" 
;"" 
case## '
UnauthorizedAccessException## 0
:##0 1
context$$ 
.$$ 
Result$$ "
=$$# $
new$$% (
UnauthorizedResult$$) ;
($$; <
)$$< =
;$$= >
context%% 
.%% 
ExceptionHandled%% ,
=%%- .
true%%/ 3
;%%3 4
break&& 
;&& 
case'' 
NotFoundException'' &
	exception''' 0
:''0 1
context(( 
.(( 
Result(( "
=((# $
new((% ( 
NotFoundObjectResult(() =
(((= >
new((> A
ProblemDetails((B P
{)) 
Detail** 
=**  
	exception**! *
.*** +
Message**+ 2
}++ 
)++ 
.,, !
AddContextInformation,, *
(,,* +
context,,+ 2
),,2 3
;,,3 4
context-- 
.-- 
ExceptionHandled-- ,
=--- .
true--/ 3
;--3 4
break.. 
;.. 
default// 
:// 
break00 
;00 
}11 
}22 	
}33 
internal55 
static55 
class55 $
ProblemDetailsExtensions55 2
{66 
public77 
static77 
IActionResult77 #!
AddContextInformation77$ 9
(779 :
this77: >
ObjectResult77? K
objectResult77L X
,77X Y
ExceptionContext77Z j
context77k r
)77r s
{88 	
if99 
(99 
objectResult99 
.99 
Value99 "
is99# %
not99& )
ProblemDetails99* 8
problemDetails999 G
)99G H
{:: 
return;; 
objectResult;; #
;;;# $
}<< 
problemDetails== 
.== 

Extensions== %
.==% &
Add==& )
(==) *
$str==* 3
,==3 4
Activity==5 =
.=== >
Current==> E
?==E F
.==F G
Id==G I
??==J L
context==M T
.==T U
HttpContext==U `
.==` a
TraceIdentifier==a p
)==p q
;==q r
problemDetails>> 
.>> 
Type>> 
=>>  !
$str>>" <
+>>= >
(>>? @
objectResult>>@ L
.>>L M

StatusCode>>M W
??>>X Z
problemDetails>>[ i
.>>i j
Status>>j p
)>>p q
;>>q r
return?? 
objectResult?? 
;??  
}@@ 	
}AA 
}BB �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\BinaryContentFilter.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str M
,M N
VersionO V
=W X
$strY ^
)^ _
]_ `
	namespace

 	
AdvancedMappingCrud


 
.

 
Repositories

 *
.

* +
Tests

+ 0
.

0 1
Api

1 4
.

4 5
Filters

5 <
{ 
public 

class 
BinaryContentFilter $
:% &
IOperationFilter' 7
{ 
public 
void 
Apply 
( 
OpenApiOperation *
	operation+ 4
,4 5"
OperationFilterContext6 L
contextM T
)T U
{ 	
var 
	attribute 
= 
context #
.# $

MethodInfo$ .
.. /
GetCustomAttributes/ B
(B C
typeofC I
(I J"
BinaryContentAttributeJ `
)` a
,a b
falsec h
)h i
.i j
FirstOrDefaultj x
(x y
)y z
;z {
if 
( 
	attribute 
== 
null !
)! "
{ 
return 
; 
} 
	operation 
. 

Parameters  
.  !
Add! $
($ %
new% (
OpenApiParameter) 9
{ 
Name 
= 
$str ,
,, -
In 
= 
ParameterLocation &
.& '
Header' -
,- .
Required 
= 
false  
,  !
Schema   
=   
new   
OpenApiSchema   *
{!! 
Type"" 
="" 
$str"" #
}## 
,## 
Description$$ 
=$$ 
$str$$ S
}%% 
)%% 
;%% 
	operation&& 
.&& 
RequestBody&& !
=&&" #
new&&$ '
OpenApiRequestBody&&( :
(&&: ;
)&&; <
{&&= >
Required&&? G
=&&H I
true&&J N
}&&O P
;&&P Q
	operation'' 
.'' 
RequestBody'' !
.''! "
Content''" )
.'') *
Add''* -
(''- .
$str''. H
,''H I
new''J M
OpenApiMediaType''N ^
(''^ _
)''_ `
{(( 
Schema)) 
=)) 
new)) 
OpenApiSchema)) *
())* +
)))+ ,
{** 
Type++ 
=++ 
$str++ #
,++# $
Format,, 
=,, 
$str,, %
,,,% &
}-- 
,-- 
}.. 
).. 
;.. 
}// 	
}00 
}11 �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\AuthorizeCheckOperationFilter.cs
[

 
assembly

 	
:

	 
 
DefaultIntentManaged

 
(

  
Mode

  $
.

$ %
Fully

% *
)

* +
]

+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str `
,` a
Versionb i
=j k
$strl q
)q r
]r s
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Filters5 <
{ 
public 

class )
AuthorizeCheckOperationFilter .
:/ 0
IOperationFilter1 A
{ 
public 
void 
Apply 
( 
OpenApiOperation *
	operation+ 4
,4 5"
OperationFilterContext6 L
contextM T
)T U
{ 	
if 
( 
! 
HasAuthorize 
( 
context %
)% &
)& '
{ 
return 
; 
} 
	operation 
. 
Security 
. 
Add "
(" #
new# &&
OpenApiSecurityRequirement' A
{ 
[ 
new !
OpenApiSecurityScheme *
{ 
	Reference 
= 
new  #
OpenApiReference$ 4
{ 
Type 
= 
ReferenceType ,
., -
SecurityScheme- ;
,; <
Id 
= 
$str %
} 
}   
]   
=   
Array   
.   
Empty    
<    !
string  ! '
>  ' (
(  ( )
)  ) *
}!! 
)!! 
;!! 
}"" 	
private$$ 
static$$ 
bool$$ 
HasAuthorize$$ (
($$( )"
OperationFilterContext$$) ?
context$$@ G
)$$G H
{%% 	
if&& 
(&& 
context&& 
.&& 

MethodInfo&& "
.&&" #
GetCustomAttributes&&# 6
(&&6 7
true&&7 ;
)&&; <
.&&< =
OfType&&= C
<&&C D
AuthorizeAttribute&&D V
>&&V W
(&&W X
)&&X Y
.&&Y Z
Any&&Z ]
(&&] ^
)&&^ _
)&&_ `
{'' 
return(( 
true(( 
;(( 
})) 
return** 
context** 
.** 

MethodInfo** %
.**% &
DeclaringType**& 3
!=**4 6
null**7 ;
&&++ 
context++ 
.++ 

MethodInfo++ %
.++% &
DeclaringType++& 3
.++3 4
GetCustomAttributes++4 G
(++G H
true++H L
)++L M
.++M N
OfType++N T
<++T U
AuthorizeAttribute++U g
>++g h
(++h i
)++i j
.++j k
Any++k n
(++n o
)++o p
;++p q
},, 	
}-- 
}.. ��
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\UsersController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[   
ApiController   
]   
public!! 

class!! 
UsersController!!  
:!!! "
ControllerBase!!# 1
{"" 
private## 
readonly## 
ISender##  
	_mediator##! *
;##* +
public%% 
UsersController%% 
(%% 
ISender%% &
mediator%%' /
)%%/ 0
{&& 	
	_mediator'' 
='' 
mediator''  
??''! #
throw''$ )
new''* -!
ArgumentNullException''. C
(''C D
nameof''D J
(''J K
mediator''K S
)''S T
)''T U
;''U V
}(( 	
[.. 	
HttpPost..	 
(.. 
$str.. )
)..) *
]..* +
[// 	
Produces//	 
(// 
MediaTypeNames//  
.//  !
Application//! ,
.//, -
Json//- 1
)//1 2
]//2 3
[00 	 
ProducesResponseType00	 
(00 
typeof00 $
(00$ %
JsonResponse00% 1
<001 2
Guid002 6
>006 7
)007 8
,008 9
StatusCodes00: E
.00E F
Status201Created00F V
)00V W
]00W X
[11 	 
ProducesResponseType11	 
(11 
StatusCodes11 )
.11) *
Status400BadRequest11* =
)11= >
]11> ?
[22 	 
ProducesResponseType22	 
(22 
typeof22 $
(22$ %
ProblemDetails22% 3
)223 4
,224 5
StatusCodes226 A
.22A B(
Status500InternalServerError22B ^
)22^ _
]22_ `
public33 
async33 
Task33 
<33 
ActionResult33 &
<33& '
JsonResponse33' 3
<333 4
Guid334 8
>338 9
>339 :
>33: ;
CreateUserAddress33< M
(33M N
[44 
FromBody44 
]44 $
CreateUserAddressCommand44 /
command440 7
,447 8
CancellationToken55 
cancellationToken55 /
=550 1
default552 9
)559 :
{66 	
var77 
result77 
=77 
await77 
	_mediator77 (
.77( )
Send77) -
(77- .
command77. 5
,775 6
cancellationToken777 H
)77H I
;77I J
return88 
CreatedAtAction88 "
(88" #
nameof88# )
(88) *
GetUserById88* 5
)885 6
,886 7
new888 ;
{88< =
id88> @
=88A B
result88C I
}88J K
,88K L
new88M P
JsonResponse88Q ]
<88] ^
Guid88^ b
>88b c
(88c d
result88d j
)88j k
)88k l
;88l m
}99 	
[?? 	
HttpPost??	 
(?? 
$str?? 
)?? 
]?? 
[@@ 	
Produces@@	 
(@@ 
MediaTypeNames@@  
.@@  !
Application@@! ,
.@@, -
Json@@- 1
)@@1 2
]@@2 3
[AA 	 
ProducesResponseTypeAA	 
(AA 
typeofAA $
(AA$ %
JsonResponseAA% 1
<AA1 2
GuidAA2 6
>AA6 7
)AA7 8
,AA8 9
StatusCodesAA: E
.AAE F
Status201CreatedAAF V
)AAV W
]AAW X
[BB 	 
ProducesResponseTypeBB	 
(BB 
StatusCodesBB )
.BB) *
Status400BadRequestBB* =
)BB= >
]BB> ?
[CC 	 
ProducesResponseTypeCC	 
(CC 
typeofCC $
(CC$ %
ProblemDetailsCC% 3
)CC3 4
,CC4 5
StatusCodesCC6 A
.CCA B(
Status500InternalServerErrorCCB ^
)CC^ _
]CC_ `
publicDD 
asyncDD 
TaskDD 
<DD 
ActionResultDD &
<DD& '
JsonResponseDD' 3
<DD3 4
GuidDD4 8
>DD8 9
>DD9 :
>DD: ;

CreateUserDD< F
(DDF G
[EE 
FromBodyEE 
]EE 
CreateUserCommandEE (
commandEE) 0
,EE0 1
CancellationTokenFF 
cancellationTokenFF /
=FF0 1
defaultFF2 9
)FF9 :
{GG 	
varHH 
resultHH 
=HH 
awaitHH 
	_mediatorHH (
.HH( )
SendHH) -
(HH- .
commandHH. 5
,HH5 6
cancellationTokenHH7 H
)HHH I
;HHI J
returnII 
CreatedAtActionII "
(II" #
nameofII# )
(II) *
GetUserByIdII* 5
)II5 6
,II6 7
newII8 ;
{II< =
idII> @
=IIA B
resultIIC I
}IIJ K
,IIK L
newIIM P
JsonResponseIIQ ]
<II] ^
GuidII^ b
>IIb c
(IIc d
resultIId j
)IIj k
)IIk l
;IIl m
}JJ 	
[QQ 	

HttpDeleteQQ	 
(QQ 
$strQQ 0
)QQ0 1
]QQ1 2
[RR 	 
ProducesResponseTypeRR	 
(RR 
StatusCodesRR )
.RR) *
Status200OKRR* 5
)RR5 6
]RR6 7
[SS 	 
ProducesResponseTypeSS	 
(SS 
StatusCodesSS )
.SS) *
Status400BadRequestSS* =
)SS= >
]SS> ?
[TT 	 
ProducesResponseTypeTT	 
(TT 
StatusCodesTT )
.TT) *
Status404NotFoundTT* ;
)TT; <
]TT< =
[UU 	 
ProducesResponseTypeUU	 
(UU 
typeofUU $
(UU$ %
ProblemDetailsUU% 3
)UU3 4
,UU4 5
StatusCodesUU6 A
.UUA B(
Status500InternalServerErrorUUB ^
)UU^ _
]UU_ `
publicVV 
asyncVV 
TaskVV 
<VV 
ActionResultVV &
>VV& '
DeleteUserAddressVV( 9
(VV9 :
[WW 
	FromQueryWW 
]WW 
GuidWW 
userIdWW #
,WW# $
[XX 
	FromRouteXX 
]XX 
GuidXX 
idXX 
,XX  
CancellationTokenYY 
cancellationTokenYY /
=YY0 1
defaultYY2 9
)YY9 :
{ZZ 	
await[[ 
	_mediator[[ 
.[[ 
Send[[  
([[  !
new[[! $$
DeleteUserAddressCommand[[% =
([[= >
userId[[> D
:[[D E
userId[[F L
,[[L M
id[[N P
:[[P Q
id[[R T
)[[T U
,[[U V
cancellationToken[[W h
)[[h i
;[[i j
return\\ 
Ok\\ 
(\\ 
)\\ 
;\\ 
}]] 	
[dd 	

HttpDeletedd	 
(dd 
$strdd #
)dd# $
]dd$ %
[ee 	 
ProducesResponseTypeee	 
(ee 
StatusCodesee )
.ee) *
Status200OKee* 5
)ee5 6
]ee6 7
[ff 	 
ProducesResponseTypeff	 
(ff 
StatusCodesff )
.ff) *
Status400BadRequestff* =
)ff= >
]ff> ?
[gg 	 
ProducesResponseTypegg	 
(gg 
StatusCodesgg )
.gg) *
Status404NotFoundgg* ;
)gg; <
]gg< =
[hh 	 
ProducesResponseTypehh	 
(hh 
typeofhh $
(hh$ %
ProblemDetailshh% 3
)hh3 4
,hh4 5
StatusCodeshh6 A
.hhA B(
Status500InternalServerErrorhhB ^
)hh^ _
]hh_ `
publicii 
asyncii 
Taskii 
<ii 
ActionResultii &
>ii& '

DeleteUserii( 2
(ii2 3
[ii3 4
	FromRouteii4 =
]ii= >
Guidii? C
idiiD F
,iiF G
CancellationTokeniiH Y
cancellationTokeniiZ k
=iil m
defaultiin u
)iiu v
{jj 	
awaitkk 
	_mediatorkk 
.kk 
Sendkk  
(kk  !
newkk! $
DeleteUserCommandkk% 6
(kk6 7
idkk7 9
:kk9 :
idkk; =
)kk= >
,kk> ?
cancellationTokenkk@ Q
)kkQ R
;kkR S
returnll 
Okll 
(ll 
)ll 
;ll 
}mm 	
[tt 	
HttpPuttt	 
(tt 
$strtt -
)tt- .
]tt. /
[uu 	 
ProducesResponseTypeuu	 
(uu 
StatusCodesuu )
.uu) *
Status204NoContentuu* <
)uu< =
]uu= >
[vv 	 
ProducesResponseTypevv	 
(vv 
StatusCodesvv )
.vv) *
Status400BadRequestvv* =
)vv= >
]vv> ?
[ww 	 
ProducesResponseTypeww	 
(ww 
StatusCodesww )
.ww) *
Status404NotFoundww* ;
)ww; <
]ww< =
[xx 	 
ProducesResponseTypexx	 
(xx 
typeofxx $
(xx$ %
ProblemDetailsxx% 3
)xx3 4
,xx4 5
StatusCodesxx6 A
.xxA B(
Status500InternalServerErrorxxB ^
)xx^ _
]xx_ `
publicyy 
asyncyy 
Taskyy 
<yy 
ActionResultyy &
>yy& '
UpdateUserAddressyy( 9
(yy9 :
[zz 
	FromRoutezz 
]zz 
Guidzz 
idzz 
,zz  
[{{ 
FromBody{{ 
]{{ $
UpdateUserAddressCommand{{ /
command{{0 7
,{{7 8
CancellationToken|| 
cancellationToken|| /
=||0 1
default||2 9
)||9 :
{}} 	
if~~ 
(~~ 
command~~ 
.~~ 
Id~~ 
==~~ 
Guid~~ "
.~~" #
Empty~~# (
)~~( )
{ 
command
�� 
.
�� 
Id
�� 
=
�� 
id
�� 
;
��  
}
�� 
if
�� 
(
�� 
id
�� 
!=
�� 
command
�� 
.
�� 
Id
��  
)
��  !
{
�� 
return
�� 

BadRequest
�� !
(
��! "
)
��" #
;
��# $
}
�� 
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpPut
��	 
(
�� 
$str
��  
)
��  !
]
��! "
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) * 
Status204NoContent
��* <
)
��< =
]
��= >
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '

UpdateUser
��( 2
(
��2 3
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
[
�� 
FromBody
�� 
]
�� 
UpdateUserCommand
�� (
command
��) 0
,
��0 1
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
if
�� 
(
�� 
command
�� 
.
�� 
Id
�� 
==
�� 
Guid
�� "
.
��" #
Empty
��# (
)
��( )
{
�� 
command
�� 
.
�� 
Id
�� 
=
�� 
id
�� 
;
��  
}
�� 
if
�� 
(
�� 
id
�� 
!=
�� 
command
�� 
.
�� 
Id
��  
)
��  !
{
�� 
return
�� 

BadRequest
�� !
(
��! "
)
��" #
;
��# $
}
�� 
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 6
)
��6 7
]
��7 8
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
UserAddressDto
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B
Status200OK
��B M
)
��M N
]
��N O
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
UserAddressDto
��' 5
>
��5 6
>
��6 7 
GetUserAddressById
��8 J
(
��J K
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
userId
�� #
,
��# $
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1%
GetUserAddressByIdQuery
��2 I
(
��I J
userId
��J P
:
��P Q
userId
��R X
,
��X Y
id
��Z \
:
��\ ]
id
��^ `
)
��` a
,
��a b
cancellationToken
��c t
)
��t u
;
��u v
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 1
)
��1 2
]
��2 3
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
List
��% )
<
��) *
UserAddressDto
��* 8
>
��8 9
)
��9 :
,
��: ;
StatusCodes
��< G
.
��G H
Status200OK
��H S
)
��S T
]
��T U
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
List
��' +
<
��+ ,
UserAddressDto
��, :
>
��: ;
>
��; <
>
��< =
GetUserAddresses
��> N
(
��N O
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
userId
�� #
,
��# $
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1#
GetUserAddressesQuery
��2 G
(
��G H
userId
��H N
:
��N O
userId
��P V
)
��V W
,
��W X
cancellationToken
��Y j
)
��j k
;
��k l
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
��  
)
��  !
]
��! "
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
UserDto
��% ,
)
��, -
,
��- .
StatusCodes
��/ :
.
��: ;
Status200OK
��; F
)
��F G
]
��G H
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
UserDto
��' .
>
��. /
>
��/ 0
GetUserById
��1 <
(
��< =
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1
GetUserByIdQuery
��2 B
(
��B C
id
��C E
:
��E F
id
��G I
)
��I J
,
��J K
cancellationToken
��L ]
)
��] ^
;
��^ _
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 
)
�� 
]
�� 
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
PagedResult
��% 0
<
��0 1
UserDto
��1 8
>
��8 9
)
��9 :
,
��: ;
StatusCodes
��< G
.
��G H
Status200OK
��H S
)
��S T
]
��T U
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
PagedResult
��' 2
<
��2 3
UserDto
��3 :
>
��: ;
>
��; <
>
��< =
GetUsers
��> F
(
��F G
[
�� 
	FromQuery
�� 
]
�� 
string
�� 
?
�� 
name
��  $
,
��$ %
[
�� 
	FromQuery
�� 
]
�� 
string
�� 
?
�� 
surname
��  '
,
��' (
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageNo
�� "
,
��" #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1
GetUsersQuery
��2 ?
(
��? @
name
��@ D
:
��D E
name
��F J
,
��J K
surname
��L S
:
��S T
surname
��U \
,
��\ ]
pageNo
��^ d
:
��d e
pageNo
��f l
,
��l m
pageSize
��n v
:
��v w
pageSize��x �
)��� �
,��� �!
cancellationToken��� �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
}
�� 
}�� �V
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\UploadDownloadController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
[ 
Route 

(
 
$str  
)  !
]! "
public 

class $
UploadDownloadController )
:* +
ControllerBase, :
{ 
private 
readonly "
IUploadDownloadService /
_appService0 ;
;; <
private 
readonly 
IUnitOfWork $
_unitOfWork% 0
;0 1
private   
readonly   
	IEventBus   "
	_eventBus  # ,
;  , -
public"" $
UploadDownloadController"" '
(""' ("
IUploadDownloadService""( >

appService""? I
,""I J
IUnitOfWork""K V

unitOfWork""W a
,""a b
	IEventBus""c l
eventBus""m u
)""u v
{## 	
_appService$$ 
=$$ 

appService$$ $
??$$% '
throw$$( -
new$$. 1!
ArgumentNullException$$2 G
($$G H
nameof$$H N
($$N O

appService$$O Y
)$$Y Z
)$$Z [
;$$[ \
_unitOfWork%% 
=%% 

unitOfWork%% $
??%%% '
throw%%( -
new%%. 1!
ArgumentNullException%%2 G
(%%G H
nameof%%H N
(%%N O

unitOfWork%%O Y
)%%Y Z
)%%Z [
;%%[ \
	_eventBus&& 
=&& 
eventBus&&  
??&&! #
throw&&$ )
new&&* -!
ArgumentNullException&&. C
(&&C D
nameof&&D J
(&&J K
eventBus&&K S
)&&S T
)&&T U
;&&U V
}'' 	
[-- 	
BinaryContent--	 
]-- 
[.. 	
HttpPost..	 
(.. 
$str.. 
).. 
].. 
[// 	 
ProducesResponseType//	 
(// 
typeof// $
(//$ %
Guid//% )
)//) *
,//* +
StatusCodes//, 7
.//7 8
Status201Created//8 H
)//H I
]//I J
[00 	 
ProducesResponseType00	 
(00 
StatusCodes00 )
.00) *
Status400BadRequest00* =
)00= >
]00> ?
[11 	 
ProducesResponseType11	 
(11 
typeof11 $
(11$ %
ProblemDetails11% 3
)113 4
,114 5
StatusCodes116 A
.11A B(
Status500InternalServerError11B ^
)11^ _
]11_ `
public22 
async22 
Task22 
<22 
ActionResult22 &
<22& '
Guid22' +
>22+ ,
>22, -
Upload22. 4
(224 5
[33 

FromHeader33 
(33 
Name33 
=33 
$str33 -
)33- .
]33. /
string330 6
?336 7
contentType338 C
,33C D
[44 

FromHeader44 
(44 
Name44 
=44 
$str44 /
)44/ 0
]440 1
long442 6
?446 7
contentLength448 E
,44E F
CancellationToken55 
cancellationToken55 /
=550 1
default552 9
)559 :
{66 	
Stream77 
stream77 
;77 
string88 
?88 
filename88 
=88 
null88 #
;88# $
if99 
(99 
Request99 
.99 
Headers99 
.99  
TryGetValue99  +
(99+ ,
$str99, A
,99A B
out99C F
var99G J
headerValues99K W
)99W X
)99X Y
{:: 
string;; 
?;; 
header;; 
=;;  
headerValues;;! -
;;;- .
if<< 
(<< 
header<< 
!=<< 
null<< "
)<<" #
{== 
var>> 
contentDisposition>> *
=>>+ ,)
ContentDispositionHeaderValue>>- J
.>>J K
Parse>>K P
(>>P Q
header>>Q W
)>>W X
;>>X Y
filename?? 
=?? 
contentDisposition?? 1
???1 2
.??2 3
FileName??3 ;
;??; <
}@@ 
}AA 
ifCC 
(CC 
RequestCC 
.CC 
ContentTypeCC #
!=CC$ &
nullCC' +
&&CC, .
(CC/ 0
RequestCC0 7
.CC7 8
ContentTypeCC8 C
==CCD F
$strCCG j
||CCk m
RequestCCn u
.CCu v
ContentType	CCv �
.
CC� �

StartsWith
CC� �
(
CC� �
$str
CC� �
)
CC� �
)
CC� �
&&
CC� �
Request
CC� �
.
CC� �
Form
CC� �
.
CC� �
Files
CC� �
.
CC� �
Any
CC� �
(
CC� �
)
CC� �
)
CC� �
{DD 
varEE 
fileEE 
=EE 
RequestEE "
.EE" #
FormEE# '
.EE' (
FilesEE( -
[EE- .
$numEE. /
]EE/ 0
;EE0 1
ifFF 
(FF 
fileFF 
==FF 
nullFF  
||FF! #
fileFF$ (
.FF( )
LengthFF) /
==FF0 2
$numFF3 4
)FF4 5
throwGG 
newGG 
ArgumentExceptionGG /
(GG/ 0
$strGG0 ?
)GG? @
;GG@ A
streamHH 
=HH 
fileHH 
.HH 
OpenReadStreamHH ,
(HH, -
)HH- .
;HH. /
filenameII 
??=II 
fileII !
.II! "
NameII" &
;II& '
}JJ 
elseKK 
{LL 
streamMM 
=MM 
RequestMM  
.MM  !
BodyMM! %
;MM% &
}NN 
varOO 
resultOO 
=OO 
GuidOO 
.OO 
EmptyOO #
;OO# $
usingQQ 
(QQ 
varQQ 
transactionQQ "
=QQ# $
newQQ% (
TransactionScopeQQ) 9
(QQ9 :"
TransactionScopeOptionQQ: P
.QQP Q
RequiredQQQ Y
,QQY Z
newRR 
TransactionOptionsRR &
{RR' (
IsolationLevelRR) 7
=RR8 9
IsolationLevelRR: H
.RRH I
ReadCommittedRRI V
}RRW X
,RRX Y+
TransactionScopeAsyncFlowOptionRRZ y
.RRy z
Enabled	RRz �
)
RR� �
)
RR� �
{SS 
resultTT 
=TT 
awaitTT 
_appServiceTT *
.TT* +
UploadTT+ 1
(TT1 2
streamTT2 8
,TT8 9
filenameTT: B
,TTB C
contentTypeTTD O
,TTO P
contentLengthTTQ ^
,TT^ _
cancellationTokenTT` q
)TTq r
;TTr s
awaitUU 
_unitOfWorkUU !
.UU! "
SaveChangesAsyncUU" 2
(UU2 3
cancellationTokenUU3 D
)UUD E
;UUE F
transactionVV 
.VV 
CompleteVV $
(VV$ %
)VV% &
;VV& '
}WW 
awaitXX 
	_eventBusXX 
.XX 
FlushAllAsyncXX )
(XX) *
cancellationTokenXX* ;
)XX; <
;XX< =
returnYY 
CreatedYY 
(YY 
stringYY !
.YY! "
EmptyYY" '
,YY' (
resultYY) /
)YY/ 0
;YY0 1
}ZZ 	
[aa 	
HttpGetaa	 
(aa 
$straa 
)aa 
]aa 
[bb 	 
ProducesResponseTypebb	 
(bb 
typeofbb $
(bb$ %
bytebb% )
[bb) *
]bb* +
)bb+ ,
,bb, -
StatusCodesbb. 9
.bb9 :
Status200OKbb: E
)bbE F
]bbF G
[cc 	 
ProducesResponseTypecc	 
(cc 
StatusCodescc )
.cc) *
Status400BadRequestcc* =
)cc= >
]cc> ?
[dd 	 
ProducesResponseTypedd	 
(dd 
StatusCodesdd )
.dd) *
Status404NotFounddd* ;
)dd; <
]dd< =
[ee 	 
ProducesResponseTypeee	 
(ee 
typeofee $
(ee$ %
ProblemDetailsee% 3
)ee3 4
,ee4 5
StatusCodesee6 A
.eeA B(
Status500InternalServerErroreeB ^
)ee^ _
]ee_ `
publicff 
asyncff 
Taskff 
<ff 
ActionResultff &
<ff& '
byteff' +
[ff+ ,
]ff, -
>ff- .
>ff. /
Downloadff0 8
(ff8 9
[gg 
	FromQuerygg 
]gg 
Guidgg 
idgg 
,gg  
CancellationTokenhh 
cancellationTokenhh /
=hh0 1
defaulthh2 9
)hh9 :
{ii 	
varjj 
resultjj 
=jj 
defaultjj  
(jj  !
FileDownloadDtojj! 0
)jj0 1
;jj1 2
resultkk 
=kk 
awaitkk 
_appServicekk &
.kk& '
Downloadkk' /
(kk/ 0
idkk0 2
,kk2 3
cancellationTokenkk4 E
)kkE F
;kkF G
ifll 
(ll 
resultll 
==ll 
nullll 
)ll 
{mm 
returnnn 
NotFoundnn 
(nn  
)nn  !
;nn! "
}oo 
returnpp 
Filepp 
(pp 
resultpp 
.pp 
Contentpp &
,pp& '
resultpp( .
.pp. /
ContentTypepp/ :
??pp; =
$strpp> X
,ppX Y
resultppZ `
.pp` a
Filenameppa i
)ppi j
;ppj k
}qq 	
}rr 
}ss �

�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\ResponseTypes\JsonResponse.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str F
,F G
VersionH O
=P Q
$strR W
)W X
]X Y
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
.@ A
ResponseTypesA N
{ 
public 

class 
JsonResponse 
< 
T 
>  
{ 
public 
JsonResponse 
( 
T 
value #
)# $
{ 	
Value 
= 
value 
; 
} 	
public 
T 
Value 
{ 
get 
; 
set !
;! "
}# $
} 
} ٌ
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\ProductsController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
[ 
Route 

(
 
$str 
) 
] 
public 

class 
ProductsController #
:$ %
ControllerBase& 4
{ 
private 
readonly 
IProductsService )
_appService* 5
;5 6
private 
readonly 
IValidationService +
_validationService, >
;> ?
private 
readonly 
IUnitOfWork $
_unitOfWork% 0
;0 1
private 
readonly 
	IEventBus "
	_eventBus# ,
;, -
public   
ProductsController   !
(  ! "
IProductsService  " 2

appService  3 =
,  = >
IValidationService!! 
validationService!! 0
,!!0 1
IUnitOfWork"" 

unitOfWork"" "
,""" #
	IEventBus## 
eventBus## 
)## 
{$$ 	
_appService%% 
=%% 

appService%% $
??%%% '
throw%%( -
new%%. 1!
ArgumentNullException%%2 G
(%%G H
nameof%%H N
(%%N O

appService%%O Y
)%%Y Z
)%%Z [
;%%[ \
_validationService&& 
=&&  
validationService&&! 2
??&&3 5
throw&&6 ;
new&&< ?!
ArgumentNullException&&@ U
(&&U V
nameof&&V \
(&&\ ]
validationService&&] n
)&&n o
)&&o p
;&&p q
_unitOfWork'' 
='' 

unitOfWork'' $
??''% '
throw''( -
new''. 1!
ArgumentNullException''2 G
(''G H
nameof''H N
(''N O

unitOfWork''O Y
)''Y Z
)''Z [
;''[ \
	_eventBus(( 
=(( 
eventBus((  
??((! #
throw(($ )
new((* -!
ArgumentNullException((. C
(((C D
nameof((D J
(((J K
eventBus((K S
)((S T
)((T U
;((U V
})) 	
[// 	
HttpPost//	 
]// 
[00 	 
ProducesResponseType00	 
(00 
typeof00 $
(00$ %
Guid00% )
)00) *
,00* +
StatusCodes00, 7
.007 8
Status201Created008 H
)00H I
]00I J
[11 	 
ProducesResponseType11	 
(11 
StatusCodes11 )
.11) *
Status400BadRequest11* =
)11= >
]11> ?
[22 	 
ProducesResponseType22	 
(22 
typeof22 $
(22$ %
ProblemDetails22% 3
)223 4
,224 5
StatusCodes226 A
.22A B(
Status500InternalServerError22B ^
)22^ _
]22_ `
public33 
async33 
Task33 
<33 
ActionResult33 &
<33& '
Guid33' +
>33+ ,
>33, -
CreateProduct33. ;
(33; <
[44 
FromBody44 
]44 
ProductCreateDto44 '
dto44( +
,44+ ,
CancellationToken55 
cancellationToken55 /
=550 1
default552 9
)559 :
{66 	
await77 
_validationService77 $
.77$ %
Handle77% +
(77+ ,
dto77, /
,77/ 0
cancellationToken771 B
)77B C
;77C D
var88 
result88 
=88 
Guid88 
.88 
Empty88 #
;88# $
using99 
(99 
var99 
transaction99 "
=99# $
new99% (
TransactionScope99) 9
(999 :"
TransactionScopeOption99: P
.99P Q
Required99Q Y
,99Y Z
new:: 
TransactionOptions:: &
{::' (
IsolationLevel::) 7
=::8 9
IsolationLevel::: H
.::H I
ReadCommitted::I V
}::W X
,::X Y+
TransactionScopeAsyncFlowOption::Z y
.::y z
Enabled	::z �
)
::� �
)
::� �
{;; 
result<< 
=<< 
await<< 
_appService<< *
.<<* +
CreateProduct<<+ 8
(<<8 9
dto<<9 <
,<<< =
cancellationToken<<> O
)<<O P
;<<P Q
await== 
_unitOfWork== !
.==! "
SaveChangesAsync==" 2
(==2 3
cancellationToken==3 D
)==D E
;==E F
transaction>> 
.>> 
Complete>> $
(>>$ %
)>>% &
;>>& '
}?? 
await@@ 
	_eventBus@@ 
.@@ 
FlushAllAsync@@ )
(@@) *
cancellationToken@@* ;
)@@; <
;@@< =
returnAA 
CreatedAtActionAA "
(AA" #
nameofAA# )
(AA) *
FindProductByIdAA* 9
)AA9 :
,AA: ;
newAA< ?
{AA@ A
idAAB D
=AAE F
resultAAG M
}AAN O
,AAO P
resultAAQ W
)AAW X
;AAX Y
}BB 	
[II 	
HttpGetII	 
(II 
$strII 
)II 
]II 
[JJ 	 
ProducesResponseTypeJJ	 
(JJ 
typeofJJ $
(JJ$ %

ProductDtoJJ% /
)JJ/ 0
,JJ0 1
StatusCodesJJ2 =
.JJ= >
Status200OKJJ> I
)JJI J
]JJJ K
[KK 	 
ProducesResponseTypeKK	 
(KK 
StatusCodesKK )
.KK) *
Status400BadRequestKK* =
)KK= >
]KK> ?
[LL 	 
ProducesResponseTypeLL	 
(LL 
StatusCodesLL )
.LL) *
Status404NotFoundLL* ;
)LL; <
]LL< =
[MM 	 
ProducesResponseTypeMM	 
(MM 
typeofMM $
(MM$ %
ProblemDetailsMM% 3
)MM3 4
,MM4 5
StatusCodesMM6 A
.MMA B(
Status500InternalServerErrorMMB ^
)MM^ _
]MM_ `
publicNN 
asyncNN 
TaskNN 
<NN 
ActionResultNN &
<NN& '

ProductDtoNN' 1
>NN1 2
>NN2 3
FindProductByIdNN4 C
(NNC D
[OO 
	FromRouteOO 
]OO 
GuidOO 
idOO 
,OO  
CancellationTokenPP 
cancellationTokenPP /
=PP0 1
defaultPP2 9
)PP9 :
{QQ 	
varRR 
resultRR 
=RR 
defaultRR  
(RR  !

ProductDtoRR! +
)RR+ ,
;RR, -
resultSS 
=SS 
awaitSS 
_appServiceSS &
.SS& '
FindProductByIdSS' 6
(SS6 7
idSS7 9
,SS9 :
cancellationTokenSS; L
)SSL M
;SSM N
returnTT 
resultTT 
==TT 
nullTT !
?TT" #
NotFoundTT$ ,
(TT, -
)TT- .
:TT/ 0
OkTT1 3
(TT3 4
resultTT4 :
)TT: ;
;TT; <
}UU 	
[ZZ 	
HttpGetZZ	 
]ZZ 
[[[ 	 
ProducesResponseType[[	 
([[ 
typeof[[ $
([[$ %
List[[% )
<[[) *

ProductDto[[* 4
>[[4 5
)[[5 6
,[[6 7
StatusCodes[[8 C
.[[C D
Status200OK[[D O
)[[O P
][[P Q
[\\ 	 
ProducesResponseType\\	 
(\\ 
typeof\\ $
(\\$ %
ProblemDetails\\% 3
)\\3 4
,\\4 5
StatusCodes\\6 A
.\\A B(
Status500InternalServerError\\B ^
)\\^ _
]\\_ `
public]] 
async]] 
Task]] 
<]] 
ActionResult]] &
<]]& '
List]]' +
<]]+ ,

ProductDto]], 6
>]]6 7
>]]7 8
>]]8 9
FindProducts]]: F
(]]F G
CancellationToken]]G X
cancellationToken]]Y j
=]]k l
default]]m t
)]]t u
{^^ 	
var__ 
result__ 
=__ 
default__  
(__  !
List__! %
<__% &

ProductDto__& 0
>__0 1
)__1 2
;__2 3
result`` 
=`` 
await`` 
_appService`` &
.``& '
FindProducts``' 3
(``3 4
cancellationToken``4 E
)``E F
;``F G
returnaa 
Okaa 
(aa 
resultaa 
)aa 
;aa 
}bb 	
[ii 	
HttpPutii	 
(ii 
$strii 
)ii 
]ii 
[jj 	 
ProducesResponseTypejj	 
(jj 
StatusCodesjj )
.jj) *
Status204NoContentjj* <
)jj< =
]jj= >
[kk 	 
ProducesResponseTypekk	 
(kk 
StatusCodeskk )
.kk) *
Status400BadRequestkk* =
)kk= >
]kk> ?
[ll 	 
ProducesResponseTypell	 
(ll 
StatusCodesll )
.ll) *
Status404NotFoundll* ;
)ll; <
]ll< =
[mm 	 
ProducesResponseTypemm	 
(mm 
typeofmm $
(mm$ %
ProblemDetailsmm% 3
)mm3 4
,mm4 5
StatusCodesmm6 A
.mmA B(
Status500InternalServerErrormmB ^
)mm^ _
]mm_ `
publicnn 
asyncnn 
Tasknn 
<nn 
ActionResultnn &
>nn& '
UpdateProductnn( 5
(nn5 6
[oo 
	FromRouteoo 
]oo 
Guidoo 
idoo 
,oo  
[pp 
FromBodypp 
]pp 
ProductUpdateDtopp '
dtopp( +
,pp+ ,
CancellationTokenqq 
cancellationTokenqq /
=qq0 1
defaultqq2 9
)qq9 :
{rr 	
awaitss 
_validationServicess $
.ss$ %
Handless% +
(ss+ ,
dtoss, /
,ss/ 0
cancellationTokenss1 B
)ssB C
;ssC D
usingtt 
(tt 
vartt 
transactiontt "
=tt# $
newtt% (
TransactionScopett) 9
(tt9 :"
TransactionScopeOptiontt: P
.ttP Q
RequiredttQ Y
,ttY Z
newuu 
TransactionOptionsuu &
{uu' (
IsolationLeveluu) 7
=uu8 9
IsolationLeveluu: H
.uuH I
ReadCommitteduuI V
}uuW X
,uuX Y+
TransactionScopeAsyncFlowOptionuuZ y
.uuy z
Enabled	uuz �
)
uu� �
)
uu� �
{vv 
awaitww 
_appServiceww !
.ww! "
UpdateProductww" /
(ww/ 0
idww0 2
,ww2 3
dtoww4 7
,ww7 8
cancellationTokenww9 J
)wwJ K
;wwK L
awaitxx 
_unitOfWorkxx !
.xx! "
SaveChangesAsyncxx" 2
(xx2 3
cancellationTokenxx3 D
)xxD E
;xxE F
transactionyy 
.yy 
Completeyy $
(yy$ %
)yy% &
;yy& '
}zz 
await{{ 
	_eventBus{{ 
.{{ 
FlushAllAsync{{ )
({{) *
cancellationToken{{* ;
){{; <
;{{< =
return|| 
	NoContent|| 
(|| 
)|| 
;|| 
}}} 	
[
�� 	

HttpDelete
��	 
(
�� 
$str
�� 
)
�� 
]
�� 
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status200OK
��* 5
)
��5 6
]
��6 7
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '
DeleteProduct
��( 5
(
��5 6
[
��6 7
	FromRoute
��7 @
]
��@ A
Guid
��B F
id
��G I
,
��I J
CancellationToken
��K \
cancellationToken
��] n
=
��o p
default
��q x
)
��x y
{
�� 	
using
�� 
(
�� 
var
�� 
transaction
�� "
=
��# $
new
��% (
TransactionScope
��) 9
(
��9 :$
TransactionScopeOption
��: P
.
��P Q
Required
��Q Y
,
��Y Z
new
��  
TransactionOptions
�� &
{
��' (
IsolationLevel
��) 7
=
��8 9
IsolationLevel
��: H
.
��H I
ReadCommitted
��I V
}
��W X
,
��X Y-
TransactionScopeAsyncFlowOption
��Z y
.
��y z
Enabled��z �
)��� �
)��� �
{
�� 
await
�� 
_appService
�� !
.
��! "
DeleteProduct
��" /
(
��/ 0
id
��0 2
,
��2 3
cancellationToken
��4 E
)
��E F
;
��F G
await
�� 
_unitOfWork
�� !
.
��! "
SaveChangesAsync
��" 2
(
��2 3
cancellationToken
��3 D
)
��D E
;
��E F
transaction
�� 
.
�� 
Complete
�� $
(
��$ %
)
��% &
;
��& '
}
�� 
await
�� 
	_eventBus
�� 
.
�� 
FlushAllAsync
�� )
(
��) *
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
Ok
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 
)
�� 
]
�� 
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
PagedResult
��% 0
<
��0 1

ProductDto
��1 ;
>
��; <
)
��< =
,
��= >
StatusCodes
��? J
.
��J K
Status200OK
��K V
)
��V W
]
��W X
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
PagedResult
��' 2
<
��2 3

ProductDto
��3 =
>
��= >
>
��> ?
>
��? @
FindProductsPaged
��A R
(
��R S
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageNo
�� "
,
��" #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
[
�� 
	FromQuery
�� 
]
�� 
string
�� 
orderBy
�� &
,
��& '
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
default
��  
(
��  !
PagedResult
��! ,
<
��, -

ProductDto
��- 7
>
��7 8
)
��8 9
;
��9 :
result
�� 
=
�� 
await
�� 
_appService
�� &
.
��& '
FindProductsPaged
��' 8
(
��8 9
pageNo
��9 ?
,
��? @
pageSize
��A I
,
��I J
orderBy
��K R
,
��R S
cancellationToken
��T e
)
��e f
;
��f g
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
}
�� 
}�� �Z
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\ParentWithAnemicChildrenController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
public 

class .
"ParentWithAnemicChildrenController 3
:4 5
ControllerBase6 D
{ 
private 
readonly 
ISender  
	_mediator! *
;* +
public .
"ParentWithAnemicChildrenController 1
(1 2
ISender2 9
mediator: B
)B C
{   	
	_mediator!! 
=!! 
mediator!!  
??!!! #
throw!!$ )
new!!* -!
ArgumentNullException!!. C
(!!C D
nameof!!D J
(!!J K
mediator!!K S
)!!S T
)!!T U
;!!U V
}"" 	
[(( 	
HttpPost((	 
((( 
$str(( 3
)((3 4
]((4 5
[)) 	
Produces))	 
()) 
MediaTypeNames))  
.))  !
Application))! ,
.)), -
Json))- 1
)))1 2
]))2 3
[** 	 
ProducesResponseType**	 
(** 
typeof** $
(**$ %
JsonResponse**% 1
<**1 2
Guid**2 6
>**6 7
)**7 8
,**8 9
StatusCodes**: E
.**E F
Status201Created**F V
)**V W
]**W X
[++ 	 
ProducesResponseType++	 
(++ 
StatusCodes++ )
.++) *
Status400BadRequest++* =
)++= >
]++> ?
[,, 	 
ProducesResponseType,,	 
(,, 
typeof,, $
(,,$ %
ProblemDetails,,% 3
),,3 4
,,,4 5
StatusCodes,,6 A
.,,A B(
Status500InternalServerError,,B ^
),,^ _
],,_ `
public-- 
async-- 
Task-- 
<-- 
ActionResult-- &
<--& '
JsonResponse--' 3
<--3 4
Guid--4 8
>--8 9
>--9 :
>--: ;'
CreateParentWithAnemicChild--< W
(--W X
[.. 
FromBody.. 
].. .
"CreateParentWithAnemicChildCommand.. 9
command..: A
,..A B
CancellationToken// 
cancellationToken// /
=//0 1
default//2 9
)//9 :
{00 	
var11 
result11 
=11 
await11 
	_mediator11 (
.11( )
Send11) -
(11- .
command11. 5
,115 6
cancellationToken117 H
)11H I
;11I J
return22 
CreatedAtAction22 "
(22" #
nameof22# )
(22) *(
GetParentWithAnemicChildById22* F
)22F G
,22G H
new22I L
{22M N
id22O Q
=22R S
result22T Z
}22[ \
,22\ ]
new22^ a
JsonResponse22b n
<22n o
Guid22o s
>22s t
(22t u
result22u {
)22{ |
)22| }
;22} ~
}33 	
[:: 	

HttpDelete::	 
(:: 
$str:: :
)::: ;
]::; <
[;; 	 
ProducesResponseType;;	 
(;; 
StatusCodes;; )
.;;) *
Status200OK;;* 5
);;5 6
];;6 7
[<< 	 
ProducesResponseType<<	 
(<< 
StatusCodes<< )
.<<) *
Status400BadRequest<<* =
)<<= >
]<<> ?
[== 	 
ProducesResponseType==	 
(== 
StatusCodes== )
.==) *
Status404NotFound==* ;
)==; <
]==< =
[>> 	 
ProducesResponseType>>	 
(>> 
typeof>> $
(>>$ %
ProblemDetails>>% 3
)>>3 4
,>>4 5
StatusCodes>>6 A
.>>A B(
Status500InternalServerError>>B ^
)>>^ _
]>>_ `
public?? 
async?? 
Task?? 
<?? 
ActionResult?? &
>??& ''
DeleteParentWithAnemicChild??( C
(??C D
[@@ 
	FromRoute@@ 
]@@ 
Guid@@ 
id@@ 
,@@  
CancellationTokenAA 
cancellationTokenAA /
=AA0 1
defaultAA2 9
)AA9 :
{BB 	
awaitCC 
	_mediatorCC 
.CC 
SendCC  
(CC  !
newCC! $.
"DeleteParentWithAnemicChildCommandCC% G
(CCG H
idCCH J
:CCJ K
idCCL N
)CCN O
,CCO P
cancellationTokenCCQ b
)CCb c
;CCc d
returnDD 
OkDD 
(DD 
)DD 
;DD 
}EE 	
[LL 	
HttpPutLL	 
(LL 
$strLL 7
)LL7 8
]LL8 9
[MM 	 
ProducesResponseTypeMM	 
(MM 
StatusCodesMM )
.MM) *
Status204NoContentMM* <
)MM< =
]MM= >
[NN 	 
ProducesResponseTypeNN	 
(NN 
StatusCodesNN )
.NN) *
Status400BadRequestNN* =
)NN= >
]NN> ?
[OO 	 
ProducesResponseTypeOO	 
(OO 
StatusCodesOO )
.OO) *
Status404NotFoundOO* ;
)OO; <
]OO< =
[PP 	 
ProducesResponseTypePP	 
(PP 
typeofPP $
(PP$ %
ProblemDetailsPP% 3
)PP3 4
,PP4 5
StatusCodesPP6 A
.PPA B(
Status500InternalServerErrorPPB ^
)PP^ _
]PP_ `
publicQQ 
asyncQQ 
TaskQQ 
<QQ 
ActionResultQQ &
>QQ& ''
UpdateParentWithAnemicChildQQ( C
(QQC D
[RR 
	FromRouteRR 
]RR 
GuidRR 
idRR 
,RR  
[SS 
FromBodySS 
]SS .
"UpdateParentWithAnemicChildCommandSS 9
commandSS: A
,SSA B
CancellationTokenTT 
cancellationTokenTT /
=TT0 1
defaultTT2 9
)TT9 :
{UU 	
ifVV 
(VV 
commandVV 
.VV 
IdVV 
==VV 
GuidVV "
.VV" #
EmptyVV# (
)VV( )
{WW 
commandXX 
.XX 
IdXX 
=XX 
idXX 
;XX  
}YY 
if[[ 
([[ 
id[[ 
!=[[ 
command[[ 
.[[ 
Id[[  
)[[  !
{\\ 
return]] 

BadRequest]] !
(]]! "
)]]" #
;]]# $
}^^ 
await`` 
	_mediator`` 
.`` 
Send``  
(``  !
command``! (
,``( )
cancellationToken``* ;
)``; <
;``< =
returnaa 
	NoContentaa 
(aa 
)aa 
;aa 
}bb 	
[ii 	
HttpGetii	 
(ii 
$strii 7
)ii7 8
]ii8 9
[jj 	 
ProducesResponseTypejj	 
(jj 
typeofjj $
(jj$ %$
ParentWithAnemicChildDtojj% =
)jj= >
,jj> ?
StatusCodesjj@ K
.jjK L
Status200OKjjL W
)jjW X
]jjX Y
[kk 	 
ProducesResponseTypekk	 
(kk 
StatusCodeskk )
.kk) *
Status400BadRequestkk* =
)kk= >
]kk> ?
[ll 	 
ProducesResponseTypell	 
(ll 
StatusCodesll )
.ll) *
Status404NotFoundll* ;
)ll; <
]ll< =
[mm 	 
ProducesResponseTypemm	 
(mm 
typeofmm $
(mm$ %
ProblemDetailsmm% 3
)mm3 4
,mm4 5
StatusCodesmm6 A
.mmA B(
Status500InternalServerErrormmB ^
)mm^ _
]mm_ `
publicnn 
asyncnn 
Tasknn 
<nn 
ActionResultnn &
<nn& '$
ParentWithAnemicChildDtonn' ?
>nn? @
>nn@ A(
GetParentWithAnemicChildByIdnnB ^
(nn^ _
[oo 
	FromRouteoo 
]oo 
Guidoo 
idoo 
,oo  
CancellationTokenpp 
cancellationTokenpp /
=pp0 1
defaultpp2 9
)pp9 :
{qq 	
varrr 
resultrr 
=rr 
awaitrr 
	_mediatorrr (
.rr( )
Sendrr) -
(rr- .
newrr. 1-
!GetParentWithAnemicChildByIdQueryrr2 S
(rrS T
idrrT V
:rrV W
idrrX Z
)rrZ [
,rr[ \
cancellationTokenrr] n
)rrn o
;rro p
returnss 
resultss 
==ss 
nullss !
?ss" #
NotFoundss$ ,
(ss, -
)ss- .
:ss/ 0
Okss1 3
(ss3 4
resultss4 :
)ss: ;
;ss; <
}tt 	
[yy 	
HttpGetyy	 
(yy 
$stryy 2
)yy2 3
]yy3 4
[zz 	 
ProducesResponseTypezz	 
(zz 
typeofzz $
(zz$ %
Listzz% )
<zz) *$
ParentWithAnemicChildDtozz* B
>zzB C
)zzC D
,zzD E
StatusCodeszzF Q
.zzQ R
Status200OKzzR ]
)zz] ^
]zz^ _
[{{ 	 
ProducesResponseType{{	 
({{ 
typeof{{ $
({{$ %
ProblemDetails{{% 3
){{3 4
,{{4 5
StatusCodes{{6 A
.{{A B(
Status500InternalServerError{{B ^
){{^ _
]{{_ `
public|| 
async|| 
Task|| 
<|| 
ActionResult|| &
<||& '
List||' +
<||+ ,$
ParentWithAnemicChildDto||, D
>||D E
>||E F
>||F G'
GetParentWithAnemicChildren||H c
(||c d
CancellationToken||d u
cancellationToken	||v �
=
||� �
default
||� �
)
||� �
{}} 	
var~~ 
result~~ 
=~~ 
await~~ 
	_mediator~~ (
.~~( )
Send~~) -
(~~- .
new~~. 1,
 GetParentWithAnemicChildrenQuery~~2 R
(~~R S
)~~S T
,~~T U
cancellationToken~~V g
)~~g h
;~~h i
return 
Ok 
( 
result 
) 
; 
}
�� 	
}
�� 
}�� �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\PagingTSController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
[ 
Route 

(
 
$str 
) 
] 
public 

class 
PagingTSController #
:$ %
ControllerBase& 4
{ 
private 
readonly 
IPagingTSService )
_appService* 5
;5 6
private 
readonly 
IValidationService +
_validationService, >
;> ?
private 
readonly 
IUnitOfWork $
_unitOfWork% 0
;0 1
private 
readonly 
	IEventBus "
	_eventBus# ,
;, -
public   
PagingTSController   !
(  ! "
IPagingTSService  " 2

appService  3 =
,  = >
IValidationService!! 
validationService!! 0
,!!0 1
IUnitOfWork"" 

unitOfWork"" "
,""" #
	IEventBus## 
eventBus## 
)## 
{$$ 	
_appService%% 
=%% 

appService%% $
??%%% '
throw%%( -
new%%. 1!
ArgumentNullException%%2 G
(%%G H
nameof%%H N
(%%N O

appService%%O Y
)%%Y Z
)%%Z [
;%%[ \
_validationService&& 
=&&  
validationService&&! 2
??&&3 5
throw&&6 ;
new&&< ?!
ArgumentNullException&&@ U
(&&U V
nameof&&V \
(&&\ ]
validationService&&] n
)&&n o
)&&o p
;&&p q
_unitOfWork'' 
='' 

unitOfWork'' $
??''% '
throw''( -
new''. 1!
ArgumentNullException''2 G
(''G H
nameof''H N
(''N O

unitOfWork''O Y
)''Y Z
)''Z [
;''[ \
	_eventBus(( 
=(( 
eventBus((  
??((! #
throw(($ )
new((* -!
ArgumentNullException((. C
(((C D
nameof((D J
(((J K
eventBus((K S
)((S T
)((T U
;((U V
})) 	
[// 	
HttpPost//	 
]// 
[00 	 
ProducesResponseType00	 
(00 
typeof00 $
(00$ %
Guid00% )
)00) *
,00* +
StatusCodes00, 7
.007 8
Status201Created008 H
)00H I
]00I J
[11 	 
ProducesResponseType11	 
(11 
StatusCodes11 )
.11) *
Status400BadRequest11* =
)11= >
]11> ?
[22 	 
ProducesResponseType22	 
(22 
typeof22 $
(22$ %
ProblemDetails22% 3
)223 4
,224 5
StatusCodes226 A
.22A B(
Status500InternalServerError22B ^
)22^ _
]22_ `
public33 
async33 
Task33 
<33 
ActionResult33 &
<33& '
Guid33' +
>33+ ,
>33, -
CreatePagingTS33. <
(33< =
[44 
FromBody44 
]44 
PagingTSCreateDto44 (
dto44) ,
,44, -
CancellationToken55 
cancellationToken55 /
=550 1
default552 9
)559 :
{66 	
await77 
_validationService77 $
.77$ %
Handle77% +
(77+ ,
dto77, /
,77/ 0
cancellationToken771 B
)77B C
;77C D
var88 
result88 
=88 
Guid88 
.88 
Empty88 #
;88# $
using:: 
(:: 
var:: 
transaction:: "
=::# $
new::% (
TransactionScope::) 9
(::9 :"
TransactionScopeOption::: P
.::P Q
Required::Q Y
,::Y Z
new;; 
TransactionOptions;; &
{;;' (
IsolationLevel;;) 7
=;;8 9
IsolationLevel;;: H
.;;H I
ReadCommitted;;I V
};;W X
,;;X Y+
TransactionScopeAsyncFlowOption;;Z y
.;;y z
Enabled	;;z �
)
;;� �
)
;;� �
{<< 
result== 
=== 
await== 
_appService== *
.==* +
CreatePagingTS==+ 9
(==9 :
dto==: =
,=== >
cancellationToken==? P
)==P Q
;==Q R
await>> 
_unitOfWork>> !
.>>! "
SaveChangesAsync>>" 2
(>>2 3
cancellationToken>>3 D
)>>D E
;>>E F
transaction?? 
.?? 
Complete?? $
(??$ %
)??% &
;??& '
}@@ 
awaitAA 
	_eventBusAA 
.AA 
FlushAllAsyncAA )
(AA) *
cancellationTokenAA* ;
)AA; <
;AA< =
returnBB 
CreatedAtActionBB "
(BB" #
nameofBB# )
(BB) *
FindPagingTSByIdBB* :
)BB: ;
,BB; <
newBB= @
{BBA B
idBBC E
=BBF G
resultBBH N
}BBO P
,BBP Q
resultBBR X
)BBX Y
;BBY Z
}CC 	
[JJ 	
HttpGetJJ	 
(JJ 
$strJJ 
)JJ 
]JJ 
[KK 	 
ProducesResponseTypeKK	 
(KK 
typeofKK $
(KK$ %
PagingTSDtoKK% 0
)KK0 1
,KK1 2
StatusCodesKK3 >
.KK> ?
Status200OKKK? J
)KKJ K
]KKK L
[LL 	 
ProducesResponseTypeLL	 
(LL 
StatusCodesLL )
.LL) *
Status400BadRequestLL* =
)LL= >
]LL> ?
[MM 	 
ProducesResponseTypeMM	 
(MM 
StatusCodesMM )
.MM) *
Status404NotFoundMM* ;
)MM; <
]MM< =
[NN 	 
ProducesResponseTypeNN	 
(NN 
typeofNN $
(NN$ %
ProblemDetailsNN% 3
)NN3 4
,NN4 5
StatusCodesNN6 A
.NNA B(
Status500InternalServerErrorNNB ^
)NN^ _
]NN_ `
publicOO 
asyncOO 
TaskOO 
<OO 
ActionResultOO &
<OO& '
PagingTSDtoOO' 2
>OO2 3
>OO3 4
FindPagingTSByIdOO5 E
(OOE F
[PP 
	FromRoutePP 
]PP 
GuidPP 
idPP 
,PP  
CancellationTokenQQ 
cancellationTokenQQ /
=QQ0 1
defaultQQ2 9
)QQ9 :
{RR 	
varSS 
resultSS 
=SS 
defaultSS  
(SS  !
PagingTSDtoSS! ,
)SS, -
;SS- .
resultTT 
=TT 
awaitTT 
_appServiceTT &
.TT& '
FindPagingTSByIdTT' 7
(TT7 8
idTT8 :
,TT: ;
cancellationTokenTT< M
)TTM N
;TTN O
returnUU 
resultUU 
==UU 
nullUU !
?UU" #
NotFoundUU$ ,
(UU, -
)UU- .
:UU/ 0
OkUU1 3
(UU3 4
resultUU4 :
)UU: ;
;UU; <
}VV 	
[\\ 	
HttpGet\\	 
]\\ 
[]] 	 
ProducesResponseType]]	 
(]] 
typeof]] $
(]]$ %
PagedResult]]% 0
<]]0 1
PagingTSDto]]1 <
>]]< =
)]]= >
,]]> ?
StatusCodes]]@ K
.]]K L
Status200OK]]L W
)]]W X
]]]X Y
[^^ 	 
ProducesResponseType^^	 
(^^ 
StatusCodes^^ )
.^^) *
Status400BadRequest^^* =
)^^= >
]^^> ?
[__ 	 
ProducesResponseType__	 
(__ 
typeof__ $
(__$ %
ProblemDetails__% 3
)__3 4
,__4 5
StatusCodes__6 A
.__A B(
Status500InternalServerError__B ^
)__^ _
]___ `
public`` 
async`` 
Task`` 
<`` 
ActionResult`` &
<``& '
PagedResult``' 2
<``2 3
PagingTSDto``3 >
>``> ?
>``? @
>``@ A
FindPagingTS``B N
(``N O
[aa 
	FromQueryaa 
]aa 
intaa 
pageNoaa "
,aa" #
[bb 
	FromQuerybb 
]bb 
intbb 
pageSizebb $
,bb$ %
[cc 
	FromQuerycc 
]cc 
stringcc 
?cc 
orderBycc  '
,cc' (
CancellationTokendd 
cancellationTokendd /
=dd0 1
defaultdd2 9
)dd9 :
{ee 	
varff 
resultff 
=ff 
defaultff  
(ff  !
PagedResultff! ,
<ff, -
PagingTSDtoff- 8
>ff8 9
)ff9 :
;ff: ;
resultgg 
=gg 
awaitgg 
_appServicegg &
.gg& '
FindPagingTSgg' 3
(gg3 4
pageNogg4 :
,gg: ;
pageSizegg< D
,ggD E
orderByggF M
,ggM N
cancellationTokenggO `
)gg` a
;gga b
returnhh 
Okhh 
(hh 
resulthh 
)hh 
;hh 
}ii 	
[pp 	
HttpPutpp	 
(pp 
$strpp 
)pp 
]pp 
[qq 	 
ProducesResponseTypeqq	 
(qq 
StatusCodesqq )
.qq) *
Status204NoContentqq* <
)qq< =
]qq= >
[rr 	 
ProducesResponseTyperr	 
(rr 
StatusCodesrr )
.rr) *
Status400BadRequestrr* =
)rr= >
]rr> ?
[ss 	 
ProducesResponseTypess	 
(ss 
StatusCodesss )
.ss) *
Status404NotFoundss* ;
)ss; <
]ss< =
[tt 	 
ProducesResponseTypett	 
(tt 
typeoftt $
(tt$ %
ProblemDetailstt% 3
)tt3 4
,tt4 5
StatusCodestt6 A
.ttA B(
Status500InternalServerErrorttB ^
)tt^ _
]tt_ `
publicuu 
asyncuu 
Taskuu 
<uu 
ActionResultuu &
>uu& '
UpdatePagingTSuu( 6
(uu6 7
[vv 
	FromRoutevv 
]vv 
Guidvv 
idvv 
,vv  
[ww 
FromBodyww 
]ww 
PagingTSUpdateDtoww (
dtoww) ,
,ww, -
CancellationTokenxx 
cancellationTokenxx /
=xx0 1
defaultxx2 9
)xx9 :
{yy 	
awaitzz 
_validationServicezz $
.zz$ %
Handlezz% +
(zz+ ,
dtozz, /
,zz/ 0
cancellationTokenzz1 B
)zzB C
;zzC D
using|| 
(|| 
var|| 
transaction|| "
=||# $
new||% (
TransactionScope||) 9
(||9 :"
TransactionScopeOption||: P
.||P Q
Required||Q Y
,||Y Z
new}} 
TransactionOptions}} &
{}}' (
IsolationLevel}}) 7
=}}8 9
IsolationLevel}}: H
.}}H I
ReadCommitted}}I V
}}}W X
,}}X Y+
TransactionScopeAsyncFlowOption}}Z y
.}}y z
Enabled	}}z �
)
}}� �
)
}}� �
{~~ 
await 
_appService !
.! "
UpdatePagingTS" 0
(0 1
id1 3
,3 4
dto5 8
,8 9
cancellationToken: K
)K L
;L M
await
�� 
_unitOfWork
�� !
.
��! "
SaveChangesAsync
��" 2
(
��2 3
cancellationToken
��3 D
)
��D E
;
��E F
transaction
�� 
.
�� 
Complete
�� $
(
��$ %
)
��% &
;
��& '
}
�� 
await
�� 
	_eventBus
�� 
.
�� 
FlushAllAsync
�� )
(
��) *
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	

HttpDelete
��	 
(
�� 
$str
�� 
)
�� 
]
�� 
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status200OK
��* 5
)
��5 6
]
��6 7
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '
DeletePagingTS
��( 6
(
��6 7
[
��7 8
	FromRoute
��8 A
]
��A B
Guid
��C G
id
��H J
,
��J K
CancellationToken
��L ]
cancellationToken
��^ o
=
��p q
default
��r y
)
��y z
{
�� 	
using
�� 
(
�� 
var
�� 
transaction
�� "
=
��# $
new
��% (
TransactionScope
��) 9
(
��9 :$
TransactionScopeOption
��: P
.
��P Q
Required
��Q Y
,
��Y Z
new
��  
TransactionOptions
�� &
{
��' (
IsolationLevel
��) 7
=
��8 9
IsolationLevel
��: H
.
��H I
ReadCommitted
��I V
}
��W X
,
��X Y-
TransactionScopeAsyncFlowOption
��Z y
.
��y z
Enabled��z �
)��� �
)��� �
{
�� 
await
�� 
_appService
�� !
.
��! "
DeletePagingTS
��" 0
(
��0 1
id
��1 3
,
��3 4
cancellationToken
��5 F
)
��F G
;
��G H
await
�� 
_unitOfWork
�� !
.
��! "
SaveChangesAsync
��" 2
(
��2 3
cancellationToken
��3 D
)
��D E
;
��E F
transaction
�� 
.
�� 
Complete
�� $
(
��$ %
)
��% &
;
��& '
}
�� 
await
�� 
	_eventBus
�� 
.
�� 
FlushAllAsync
�� )
(
��) *
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
Ok
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
}
�� 
}�� ��
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\OrdersController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[   
ApiController   
]   
public!! 

class!! 
OrdersController!! !
:!!" #
ControllerBase!!$ 2
{"" 
private## 
readonly## 
ISender##  
	_mediator##! *
;##* +
public%% 
OrdersController%% 
(%%  
ISender%%  '
mediator%%( 0
)%%0 1
{&& 	
	_mediator'' 
='' 
mediator''  
??''! #
throw''$ )
new''* -!
ArgumentNullException''. C
(''C D
nameof''D J
(''J K
mediator''K S
)''S T
)''T U
;''U V
}(( 	
[.. 	
HttpPost..	 
(.. 
$str.. 
).. 
].. 
[// 	
Produces//	 
(// 
MediaTypeNames//  
.//  !
Application//! ,
.//, -
Json//- 1
)//1 2
]//2 3
[00 	 
ProducesResponseType00	 
(00 
typeof00 $
(00$ %
JsonResponse00% 1
<001 2
Guid002 6
>006 7
)007 8
,008 9
StatusCodes00: E
.00E F
Status201Created00F V
)00V W
]00W X
[11 	 
ProducesResponseType11	 
(11 
StatusCodes11 )
.11) *
Status400BadRequest11* =
)11= >
]11> ?
[22 	 
ProducesResponseType22	 
(22 
typeof22 $
(22$ %
ProblemDetails22% 3
)223 4
,224 5
StatusCodes226 A
.22A B(
Status500InternalServerError22B ^
)22^ _
]22_ `
public33 
async33 
Task33 
<33 
ActionResult33 &
<33& '
JsonResponse33' 3
<333 4
Guid334 8
>338 9
>339 :
>33: ;
CreateOrder33< G
(33G H
[44 
FromBody44 
]44 
CreateOrderCommand44 )
command44* 1
,441 2
CancellationToken55 
cancellationToken55 /
=550 1
default552 9
)559 :
{66 	
var77 
result77 
=77 
await77 
	_mediator77 (
.77( )
Send77) -
(77- .
command77. 5
,775 6
cancellationToken777 H
)77H I
;77I J
return88 
CreatedAtAction88 "
(88" #
nameof88# )
(88) *
GetOrderById88* 6
)886 7
,887 8
new889 <
{88= >
id88? A
=88B C
result88D J
}88K L
,88L M
new88N Q
JsonResponse88R ^
<88^ _
Guid88_ c
>88c d
(88d e
result88e k
)88k l
)88l m
;88m n
}99 	
[?? 	
HttpPost??	 
(?? 
$str?? (
)??( )
]??) *
[@@ 	
Produces@@	 
(@@ 
MediaTypeNames@@  
.@@  !
Application@@! ,
.@@, -
Json@@- 1
)@@1 2
]@@2 3
[AA 	 
ProducesResponseTypeAA	 
(AA 
typeofAA $
(AA$ %
JsonResponseAA% 1
<AA1 2
GuidAA2 6
>AA6 7
)AA7 8
,AA8 9
StatusCodesAA: E
.AAE F
Status201CreatedAAF V
)AAV W
]AAW X
[BB 	 
ProducesResponseTypeBB	 
(BB 
StatusCodesBB )
.BB) *
Status400BadRequestBB* =
)BB= >
]BB> ?
[CC 	 
ProducesResponseTypeCC	 
(CC 
typeofCC $
(CC$ %
ProblemDetailsCC% 3
)CC3 4
,CC4 5
StatusCodesCC6 A
.CCA B(
Status500InternalServerErrorCCB ^
)CC^ _
]CC_ `
publicDD 
asyncDD 
TaskDD 
<DD 
ActionResultDD &
<DD& '
JsonResponseDD' 3
<DD3 4
GuidDD4 8
>DD8 9
>DD9 :
>DD: ; 
CreateOrderOrderItemDD< P
(DDP Q
[EE 
FromBodyEE 
]EE '
CreateOrderOrderItemCommandEE 2
commandEE3 :
,EE: ;
CancellationTokenFF 
cancellationTokenFF /
=FF0 1
defaultFF2 9
)FF9 :
{GG 	
varHH 
resultHH 
=HH 
awaitHH 
	_mediatorHH (
.HH( )
SendHH) -
(HH- .
commandHH. 5
,HH5 6
cancellationTokenHH7 H
)HHH I
;HHI J
returnII 
CreatedAtActionII "
(II" #
nameofII# )
(II) *
GetOrderByIdII* 6
)II6 7
,II7 8
newII9 <
{II= >
idII? A
=IIB C
resultIID J
}IIK L
,IIL M
newIIN Q
JsonResponseIIR ^
<II^ _
GuidII_ c
>IIc d
(IId e
resultIIe k
)IIk l
)IIl m
;IIm n
}JJ 	
[QQ 	

HttpDeleteQQ	 
(QQ 
$strQQ $
)QQ$ %
]QQ% &
[RR 	 
ProducesResponseTypeRR	 
(RR 
StatusCodesRR )
.RR) *
Status200OKRR* 5
)RR5 6
]RR6 7
[SS 	 
ProducesResponseTypeSS	 
(SS 
StatusCodesSS )
.SS) *
Status400BadRequestSS* =
)SS= >
]SS> ?
[TT 	 
ProducesResponseTypeTT	 
(TT 
StatusCodesTT )
.TT) *
Status404NotFoundTT* ;
)TT; <
]TT< =
[UU 	 
ProducesResponseTypeUU	 
(UU 
typeofUU $
(UU$ %
ProblemDetailsUU% 3
)UU3 4
,UU4 5
StatusCodesUU6 A
.UUA B(
Status500InternalServerErrorUUB ^
)UU^ _
]UU_ `
publicVV 
asyncVV 
TaskVV 
<VV 
ActionResultVV &
>VV& '
DeleteOrderVV( 3
(VV3 4
[VV4 5
	FromRouteVV5 >
]VV> ?
GuidVV@ D
idVVE G
,VVG H
CancellationTokenVVI Z
cancellationTokenVV[ l
=VVm n
defaultVVo v
)VVv w
{WW 	
awaitXX 
	_mediatorXX 
.XX 
SendXX  
(XX  !
newXX! $
DeleteOrderCommandXX% 7
(XX7 8
idXX8 :
:XX: ;
idXX< >
)XX> ?
,XX? @
cancellationTokenXXA R
)XXR S
;XXS T
returnYY 
OkYY 
(YY 
)YY 
;YY 
}ZZ 	
[aa 	

HttpDeleteaa	 
(aa 
$straa 9
)aa9 :
]aa: ;
[bb 	 
ProducesResponseTypebb	 
(bb 
StatusCodesbb )
.bb) *
Status200OKbb* 5
)bb5 6
]bb6 7
[cc 	 
ProducesResponseTypecc	 
(cc 
StatusCodescc )
.cc) *
Status400BadRequestcc* =
)cc= >
]cc> ?
[dd 	 
ProducesResponseTypedd	 
(dd 
StatusCodesdd )
.dd) *
Status404NotFounddd* ;
)dd; <
]dd< =
[ee 	 
ProducesResponseTypeee	 
(ee 
typeofee $
(ee$ %
ProblemDetailsee% 3
)ee3 4
,ee4 5
StatusCodesee6 A
.eeA B(
Status500InternalServerErroreeB ^
)ee^ _
]ee_ `
publicff 
asyncff 
Taskff 
<ff 
ActionResultff &
>ff& ' 
DeleteOrderOrderItemff( <
(ff< =
[gg 
	FromRoutegg 
]gg 
Guidgg 
orderIdgg $
,gg$ %
[hh 
	FromRoutehh 
]hh 
Guidhh 
idhh 
,hh  
CancellationTokenii 
cancellationTokenii /
=ii0 1
defaultii2 9
)ii9 :
{jj 	
awaitkk 
	_mediatorkk 
.kk 
Sendkk  
(kk  !
newkk! $'
DeleteOrderOrderItemCommandkk% @
(kk@ A
orderIdkkA H
:kkH I
orderIdkkJ Q
,kkQ R
idkkS U
:kkU V
idkkW Y
)kkY Z
,kkZ [
cancellationTokenkk\ m
)kkm n
;kkn o
returnll 
Okll 
(ll 
)ll 
;ll 
}mm 	
[tt 	
HttpPuttt	 
(tt 
$strtt !
)tt! "
]tt" #
[uu 	 
ProducesResponseTypeuu	 
(uu 
StatusCodesuu )
.uu) *
Status204NoContentuu* <
)uu< =
]uu= >
[vv 	 
ProducesResponseTypevv	 
(vv 
StatusCodesvv )
.vv) *
Status400BadRequestvv* =
)vv= >
]vv> ?
[ww 	 
ProducesResponseTypeww	 
(ww 
StatusCodesww )
.ww) *
Status404NotFoundww* ;
)ww; <
]ww< =
[xx 	 
ProducesResponseTypexx	 
(xx 
typeofxx $
(xx$ %
ProblemDetailsxx% 3
)xx3 4
,xx4 5
StatusCodesxx6 A
.xxA B(
Status500InternalServerErrorxxB ^
)xx^ _
]xx_ `
publicyy 
asyncyy 
Taskyy 
<yy 
ActionResultyy &
>yy& '
UpdateOrderyy( 3
(yy3 4
[zz 
	FromRoutezz 
]zz 
Guidzz 
idzz 
,zz  
[{{ 
FromBody{{ 
]{{ 
UpdateOrderCommand{{ )
command{{* 1
,{{1 2
CancellationToken|| 
cancellationToken|| /
=||0 1
default||2 9
)||9 :
{}} 	
if~~ 
(~~ 
command~~ 
.~~ 
Id~~ 
==~~ 
Guid~~ "
.~~" #
Empty~~# (
)~~( )
{ 
command
�� 
.
�� 
Id
�� 
=
�� 
id
�� 
;
��  
}
�� 
if
�� 
(
�� 
id
�� 
!=
�� 
command
�� 
.
�� 
Id
��  
)
��  !
{
�� 
return
�� 

BadRequest
�� !
(
��! "
)
��" #
;
��# $
}
�� 
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpPut
��	 
(
�� 
$str
�� ,
)
��, -
]
��- .
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) * 
Status204NoContent
��* <
)
��< =
]
��= >
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '"
UpdateOrderOrderItem
��( <
(
��< =
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
[
�� 
FromBody
�� 
]
�� )
UpdateOrderOrderItemCommand
�� 2
command
��3 :
,
��: ;
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
if
�� 
(
�� 
command
�� 
.
�� 
Id
�� 
==
�� 
Guid
�� "
.
��" #
Empty
��# (
)
��( )
{
�� 
command
�� 
.
�� 
Id
�� 
=
�� 
id
�� 
;
��  
}
�� 
if
�� 
(
�� 
id
�� 
!=
�� 
command
�� 
.
�� 
Id
��  
)
��  !
{
�� 
return
�� 

BadRequest
�� !
(
��! "
)
��" #
;
��# $
}
�� 
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� !
)
��! "
]
��" #
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
OrderDto
��% -
)
��- .
,
��. /
StatusCodes
��0 ;
.
��; <
Status200OK
��< G
)
��G H
]
��H I
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
OrderDto
��' /
>
��/ 0
>
��0 1
GetOrderById
��2 >
(
��> ?
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1
GetOrderByIdQuery
��2 C
(
��C D
id
��D F
:
��F G
id
��H J
)
��J K
,
��K L
cancellationToken
��M ^
)
��^ _
;
��_ `
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 6
)
��6 7
]
��7 8
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
OrderOrderItemDto
��% 6
)
��6 7
,
��7 8
StatusCodes
��9 D
.
��D E
Status200OK
��E P
)
��P Q
]
��Q R
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
OrderOrderItemDto
��' 8
>
��8 9
>
��9 :#
GetOrderOrderItemById
��; P
(
��P Q
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
orderId
�� $
,
��$ %
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1(
GetOrderOrderItemByIdQuery
��2 L
(
��L M
orderId
��M T
:
��T U
orderId
��V ]
,
��] ^
id
��_ a
:
��a b
id
��c e
)
��e f
,
��f g
cancellationToken
��h y
)
��y z
;
��z {
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 1
)
��1 2
]
��2 3
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
List
��% )
<
��) *
OrderOrderItemDto
��* ;
>
��; <
)
��< =
,
��= >
StatusCodes
��? J
.
��J K
Status200OK
��K V
)
��V W
]
��W X
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
List
��' +
<
��+ ,
OrderOrderItemDto
��, =
>
��= >
>
��> ?
>
��? @ 
GetOrderOrderItems
��A S
(
��S T
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
orderId
�� $
,
��$ %
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1%
GetOrderOrderItemsQuery
��2 I
(
��I J
orderId
��J Q
:
��Q R
orderId
��S Z
)
��Z [
,
��[ \
cancellationToken
��] n
)
��n o
;
��o p
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 
)
�� 
]
�� 
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
PagedResult
��% 0
<
��0 1
OrderDto
��1 9
>
��9 :
)
��: ;
,
��; <
StatusCodes
��= H
.
��H I
Status200OK
��I T
)
��T U
]
��U V
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
PagedResult
��' 2
<
��2 3
OrderDto
��3 ;
>
��; <
>
��< =
>
��= > 
GetOrdersPaginated
��? Q
(
��Q R
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageNo
�� "
,
��" #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1%
GetOrdersPaginatedQuery
��2 I
(
��I J
pageNo
��J P
:
��P Q
pageNo
��R X
,
��X Y
pageSize
��Z b
:
��b c
pageSize
��d l
)
��l m
,
��m n 
cancellationToken��o �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
}
�� 
}�� �,
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\OptionalsController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
public 

class 
OptionalsController $
:% &
ControllerBase' 5
{ 
private 
readonly 
ISender  
	_mediator! *
;* +
public 
OptionalsController "
(" #
ISender# *
mediator+ 3
)3 4
{ 	
	_mediator 
= 
mediator  
??! #
throw$ )
new* -!
ArgumentNullException. C
(C D
nameofD J
(J K
mediatorK S
)S T
)T U
;U V
} 	
[%% 	
HttpPost%%	 
(%% 
$str%%  
)%%  !
]%%! "
[&& 	
Produces&&	 
(&& 
MediaTypeNames&&  
.&&  !
Application&&! ,
.&&, -
Json&&- 1
)&&1 2
]&&2 3
['' 	 
ProducesResponseType''	 
('' 
typeof'' $
(''$ %
JsonResponse''% 1
<''1 2
Guid''2 6
>''6 7
)''7 8
,''8 9
StatusCodes'': E
.''E F
Status201Created''F V
)''V W
]''W X
[(( 	 
ProducesResponseType((	 
((( 
StatusCodes(( )
.(() *
Status400BadRequest((* =
)((= >
]((> ?
[)) 	 
ProducesResponseType))	 
()) 
typeof)) $
())$ %
ProblemDetails))% 3
)))3 4
,))4 5
StatusCodes))6 A
.))A B(
Status500InternalServerError))B ^
)))^ _
]))_ `
public** 
async** 
Task** 
<** 
ActionResult** &
<**& '
JsonResponse**' 3
<**3 4
Guid**4 8
>**8 9
>**9 :
>**: ;
CreateOptional**< J
(**J K
[++ 
FromBody++ 
]++ !
CreateOptionalCommand++ ,
command++- 4
,++4 5
CancellationToken,, 
cancellationToken,, /
=,,0 1
default,,2 9
),,9 :
{-- 	
var.. 
result.. 
=.. 
await.. 
	_mediator.. (
...( )
Send..) -
(..- .
command... 5
,..5 6
cancellationToken..7 H
)..H I
;..I J
return// 
CreatedAtAction// "
(//" #
nameof//# )
(//) *
GetOptionalById//* 9
)//9 :
,//: ;
new//< ?
{//@ A
id//B D
=//E F
result//G M
}//N O
,//O P
new//Q T
JsonResponse//U a
<//a b
Guid//b f
>//f g
(//g h
result//h n
)//n o
)//o p
;//p q
}00 	
[66 	
HttpGet66	 
(66 
$str66 $
)66$ %
]66% &
[77 	 
ProducesResponseType77	 
(77 
typeof77 $
(77$ %
OptionalDto77% 0
)770 1
,771 2
StatusCodes773 >
.77> ?
Status200OK77? J
)77J K
]77K L
[88 	 
ProducesResponseType88	 
(88 
StatusCodes88 )
.88) *
Status400BadRequest88* =
)88= >
]88> ?
[99 	 
ProducesResponseType99	 
(99 
typeof99 $
(99$ %
ProblemDetails99% 3
)993 4
,994 5
StatusCodes996 A
.99A B(
Status500InternalServerError99B ^
)99^ _
]99_ `
public:: 
async:: 
Task:: 
<:: 
ActionResult:: &
<::& '
OptionalDto::' 2
?::2 3
>::3 4
>::4 5
GetOptionalById::6 E
(::E F
[;; 
	FromRoute;; 
];; 
Guid;; 
id;; 
,;;  
CancellationToken<< 
cancellationToken<< /
=<<0 1
default<<2 9
)<<9 :
{== 	
var>> 
result>> 
=>> 
await>> 
	_mediator>> (
.>>( )
Send>>) -
(>>- .
new>>. 1 
GetOptionalByIdQuery>>2 F
(>>F G
id>>G I
:>>I J
id>>K M
)>>M N
,>>N O
cancellationToken>>P a
)>>a b
;>>b c
return?? 
Ok?? 
(?? 
result?? 
)?? 
;?? 
}@@ 	
}AA 
}BB �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\FileUploadsController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
public 

class !
FileUploadsController &
:' (
ControllerBase) 7
{   
private!! 
readonly!! 
ISender!!  
	_mediator!!! *
;!!* +
public## !
FileUploadsController## $
(##$ %
ISender##% ,
mediator##- 5
)##5 6
{$$ 	
	_mediator%% 
=%% 
mediator%%  
??%%! #
throw%%$ )
new%%* -!
ArgumentNullException%%. C
(%%C D
nameof%%D J
(%%J K
mediator%%K S
)%%S T
)%%T U
;%%U V
}&& 	
[,, 	
BinaryContent,,	 
],, 
[-- 	
HttpPost--	 
(-- 
$str-- 6
)--6 7
]--7 8
[.. 	 
ProducesResponseType..	 
(.. 
StatusCodes.. )
...) *
Status201Created..* :
)..: ;
]..; <
[// 	 
ProducesResponseType//	 
(// 
StatusCodes// )
.//) *
Status400BadRequest//* =
)//= >
]//> ?
[00 	 
ProducesResponseType00	 
(00 
typeof00 $
(00$ %
ProblemDetails00% 3
)003 4
,004 5
StatusCodes006 A
.00A B(
Status500InternalServerError00B ^
)00^ _
]00_ `
public11 
async11 
Task11 
<11 
ActionResult11 &
>11& '
RestrictedUpload11( 8
(118 9
[22 

FromHeader22 
(22 
Name22 
=22 
$str22 -
)22- .
]22. /
string220 6
?226 7
contentType228 C
,22C D
[33 

FromHeader33 
(33 
Name33 
=33 
$str33 /
)33/ 0
]330 1
long332 6
?336 7
contentLength338 E
,33E F
CancellationToken44 
cancellationToken44 /
=440 1
default442 9
)449 :
{55 	
if66 
(66 
Request66 
.66 
ContentLength66 %
!=66& (
null66) -
&&66. 0
Request661 8
.668 9
ContentLength669 F
>66G H
$num66I P
)66P Q
{77 
return88 

BadRequest88 !
(88! "
new88" %
{88& '
error88( -
=88. /
$str880 @
}88A B
)88B C
;88C D
}99 
var:: 
mimeTypeFilter:: 
=::  
new::! $
HashSet::% ,
<::, -
string::- 3
>::3 4
(::4 5
$str::5 L
.::L M
Split::M R
(::R S
$char::S V
)::V W
)::W X
;::X Y
if<< 
(<< 
Request<< 
.<< 
ContentType<< #
==<<$ &
null<<' +
||<<, .
!<</ 0
mimeTypeFilter<<0 >
.<<> ?
Contains<<? G
(<<G H
Request<<H O
.<<O P
ContentType<<P [
.<<[ \
ToLower<<\ c
(<<c d
)<<d e
)<<e f
)<<f g
{== 
return>> 

BadRequest>> !
(>>! "
new>>" %
{>>& '
error>>( -
=>>. /
$">>0 2
$str>>2 E
{>>E F
Request>>F M
.>>M N
ContentType>>N Y
}>>Y Z
$str>>Z g
">>g h
}>>i j
)>>j k
;>>k l
}?? 
Stream@@ 
stream@@ 
;@@ 
stringAA 
?AA 
filenameAA 
=AA 
nullAA #
;AA# $
ifBB 
(BB 
RequestBB 
.BB 
HeadersBB 
.BB  
TryGetValueBB  +
(BB+ ,
$strBB, A
,BBA B
outBBC F
varBBG J
headerValuesBBK W
)BBW X
)BBX Y
{CC 
stringDD 
?DD 
headerDD 
=DD  
headerValuesDD! -
;DD- .
ifEE 
(EE 
headerEE 
!=EE 
nullEE "
)EE" #
{FF 
varGG 
contentDispositionGG *
=GG+ ,)
ContentDispositionHeaderValueGG- J
.GGJ K
ParseGGK P
(GGP Q
headerGGQ W
)GGW X
;GGX Y
filenameHH 
=HH 
contentDispositionHH 1
?HH1 2
.HH2 3
FileNameHH3 ;
;HH; <
}II 
}JJ 
ifLL 
(LL 
RequestLL 
.LL 
ContentTypeLL #
!=LL$ &
nullLL' +
&&LL, .
(LL/ 0
RequestLL0 7
.LL7 8
ContentTypeLL8 C
==LLD F
$strLLG j
||LLk m
RequestLLn u
.LLu v
ContentType	LLv �
.
LL� �

StartsWith
LL� �
(
LL� �
$str
LL� �
)
LL� �
)
LL� �
&&
LL� �
Request
LL� �
.
LL� �
Form
LL� �
.
LL� �
Files
LL� �
.
LL� �
Any
LL� �
(
LL� �
)
LL� �
)
LL� �
{MM 
varNN 
fileNN 
=NN 
RequestNN "
.NN" #
FormNN# '
.NN' (
FilesNN( -
[NN- .
$numNN. /
]NN/ 0
;NN0 1
ifOO 
(OO 
fileOO 
==OO 
nullOO  
||OO! #
fileOO$ (
.OO( )
LengthOO) /
==OO0 2
$numOO3 4
)OO4 5
throwPP 
newPP 
ArgumentExceptionPP /
(PP/ 0
$strPP0 ?
)PP? @
;PP@ A
streamQQ 
=QQ 
fileQQ 
.QQ 
OpenReadStreamQQ ,
(QQ, -
)QQ- .
;QQ. /
filenameRR 
??=RR 
fileRR !
.RR! "
NameRR" &
;RR& '
}SS 
elseTT 
{UU 
streamVV 
=VV 
RequestVV  
.VV  !
BodyVV! %
;VV% &
}WW 
varXX 
commandXX 
=XX 
newXX #
RestrictedUploadCommandXX 5
(XX5 6
contentXX6 =
:XX= >
streamXX? E
,XXE F
filenameXXG O
:XXO P
filenameXXQ Y
,XXY Z
contentTypeXX[ f
:XXf g
contentTypeXXh s
,XXs t
contentLength	XXu �
:
XX� �
contentLength
XX� �
)
XX� �
;
XX� �
awaitZZ 
	_mediatorZZ 
.ZZ 
SendZZ  
(ZZ  !
commandZZ! (
,ZZ( )
cancellationTokenZZ* ;
)ZZ; <
;ZZ< =
return[[ 
Created[[ 
([[ 
string[[ !
.[[! "
Empty[[" '
,[[' (
null[[) -
)[[- .
;[[. /
}\\ 	
[bb 	
BinaryContentbb	 
]bb 
[cc 	
HttpPostcc	 
(cc 
$strcc 2
)cc2 3
]cc3 4
[dd 	 
ProducesResponseTypedd	 
(dd 
typeofdd $
(dd$ %
Guiddd% )
)dd) *
,dd* +
StatusCodesdd, 7
.dd7 8
Status201Createddd8 H
)ddH I
]ddI J
[ee 	 
ProducesResponseTypeee	 
(ee 
StatusCodesee )
.ee) *
Status400BadRequestee* =
)ee= >
]ee> ?
[ff 	 
ProducesResponseTypeff	 
(ff 
typeofff $
(ff$ %
ProblemDetailsff% 3
)ff3 4
,ff4 5
StatusCodesff6 A
.ffA B(
Status500InternalServerErrorffB ^
)ff^ _
]ff_ `
publicgg 
asyncgg 
Taskgg 
<gg 
ActionResultgg &
<gg& '
Guidgg' +
>gg+ ,
>gg, -
SimpleUploadgg. :
(gg: ;
CancellationTokengg; L
cancellationTokenggM ^
=gg_ `
defaultgga h
)ggh i
{hh 	
Streamii 
streamii 
;ii 
ifkk 
(kk 
Requestkk 
.kk 
ContentTypekk #
!=kk$ &
nullkk' +
&&kk, .
(kk/ 0
Requestkk0 7
.kk7 8
ContentTypekk8 C
==kkD F
$strkkG j
||kkk m
Requestkkn u
.kku v
ContentType	kkv �
.
kk� �

StartsWith
kk� �
(
kk� �
$str
kk� �
)
kk� �
)
kk� �
&&
kk� �
Request
kk� �
.
kk� �
Form
kk� �
.
kk� �
Files
kk� �
.
kk� �
Any
kk� �
(
kk� �
)
kk� �
)
kk� �
{ll 
varmm 
filemm 
=mm 
Requestmm "
.mm" #
Formmm# '
.mm' (
Filesmm( -
[mm- .
$nummm. /
]mm/ 0
;mm0 1
ifnn 
(nn 
filenn 
==nn 
nullnn  
||nn! #
filenn$ (
.nn( )
Lengthnn) /
==nn0 2
$numnn3 4
)nn4 5
throwoo 
newoo 
ArgumentExceptionoo /
(oo/ 0
$stroo0 ?
)oo? @
;oo@ A
streampp 
=pp 
filepp 
.pp 
OpenReadStreampp ,
(pp, -
)pp- .
;pp. /
}qq 
elserr 
{ss 
streamtt 
=tt 
Requesttt  
.tt  !
Bodytt! %
;tt% &
}uu 
varvv 
commandvv 
=vv 
newvv 
SimpleUploadCommandvv 1
(vv1 2
contentvv2 9
:vv9 :
streamvv; A
)vvA B
;vvB C
varxx 
resultxx 
=xx 
awaitxx 
	_mediatorxx (
.xx( )
Sendxx) -
(xx- .
commandxx. 5
,xx5 6
cancellationTokenxx7 H
)xxH I
;xxI J
returnyy 
CreatedAtActionyy "
(yy" #
nameofyy# )
(yy) *
DownloadFileyy* 6
)yy6 7
,yy7 8
newyy9 <
{yy= >
idyy? A
=yyB C
resultyyD J
}yyK L
,yyL M
resultyyN T
)yyT U
;yyU V
}zz 	
[
�� 	
BinaryContent
��	 
]
�� 
[
�� 	
HttpPost
��	 
(
�� 
$str
�� 0
)
��0 1
]
��1 2
[
�� 	
Produces
��	 
(
�� 
MediaTypeNames
��  
.
��  !
Application
��! ,
.
��, -
Json
��- 1
)
��1 2
]
��2 3
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
JsonResponse
��% 1
<
��1 2
Guid
��2 6
>
��6 7
)
��7 8
,
��8 9
StatusCodes
��: E
.
��E F
Status201Created
��F V
)
��V W
]
��W X
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
JsonResponse
��' 3
<
��3 4
Guid
��4 8
>
��8 9
>
��9 :
>
��: ;

UploadFile
��< F
(
��F G
[
�� 

FromHeader
�� 
(
�� 
Name
�� 
=
�� 
$str
�� -
)
��- .
]
��. /
string
��0 6
?
��6 7
contentType
��8 C
,
��C D
[
�� 

FromHeader
�� 
(
�� 
Name
�� 
=
�� 
$str
�� /
)
��/ 0
]
��0 1
long
��2 6
?
��6 7
contentLength
��8 E
,
��E F
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
Stream
�� 
stream
�� 
;
�� 
string
�� 
?
�� 
filename
�� 
=
�� 
null
�� #
;
��# $
if
�� 
(
�� 
Request
�� 
.
�� 
Headers
�� 
.
��  
TryGetValue
��  +
(
��+ ,
$str
��, A
,
��A B
out
��C F
var
��G J
headerValues
��K W
)
��W X
)
��X Y
{
�� 
string
�� 
?
�� 
header
�� 
=
��  
headerValues
��! -
;
��- .
if
�� 
(
�� 
header
�� 
!=
�� 
null
�� "
)
��" #
{
�� 
var
��  
contentDisposition
�� *
=
��+ ,+
ContentDispositionHeaderValue
��- J
.
��J K
Parse
��K P
(
��P Q
header
��Q W
)
��W X
;
��X Y
filename
�� 
=
��  
contentDisposition
�� 1
?
��1 2
.
��2 3
FileName
��3 ;
;
��; <
}
�� 
}
�� 
if
�� 
(
�� 
Request
�� 
.
�� 
ContentType
�� #
!=
��$ &
null
��' +
&&
��, .
(
��/ 0
Request
��0 7
.
��7 8
ContentType
��8 C
==
��D F
$str
��G j
||
��k m
Request
��n u
.
��u v
ContentType��v �
.��� �

StartsWith��� �
(��� �
$str��� �
)��� �
)��� �
&&��� �
Request��� �
.��� �
Form��� �
.��� �
Files��� �
.��� �
Any��� �
(��� �
)��� �
)��� �
{
�� 
var
�� 
file
�� 
=
�� 
Request
�� "
.
��" #
Form
��# '
.
��' (
Files
��( -
[
��- .
$num
��. /
]
��/ 0
;
��0 1
if
�� 
(
�� 
file
�� 
==
�� 
null
��  
||
��! #
file
��$ (
.
��( )
Length
��) /
==
��0 2
$num
��3 4
)
��4 5
throw
�� 
new
�� 
ArgumentException
�� /
(
��/ 0
$str
��0 ?
)
��? @
;
��@ A
stream
�� 
=
�� 
file
�� 
.
�� 
OpenReadStream
�� ,
(
��, -
)
��- .
;
��. /
filename
�� 
??=
�� 
file
�� !
.
��! "
Name
��" &
;
��& '
}
�� 
else
�� 
{
�� 
stream
�� 
=
�� 
Request
��  
.
��  !
Body
��! %
;
��% &
}
�� 
var
�� 
command
�� 
=
�� 
new
�� 
UploadFileCommand
�� /
(
��/ 0
content
��0 7
:
��7 8
stream
��9 ?
,
��? @
filename
��A I
:
��I J
filename
��K S
,
��S T
contentType
��U `
:
��` a
contentType
��b m
,
��m n
contentLength
��o |
:
��| }
contentLength��~ �
)��� �
;��� �
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
command
��. 5
,
��5 6
cancellationToken
��7 H
)
��H I
;
��I J
return
�� 
Created
�� 
(
�� 
string
�� !
.
��! "
Empty
��" '
,
��' (
new
��) ,
JsonResponse
��- 9
<
��9 :
Guid
��: >
>
��> ?
(
��? @
result
��@ F
)
��F G
)
��G H
;
��H I
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� '
)
��' (
]
��( )
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
byte
��% )
[
��) *
]
��* +
)
��+ ,
,
��, -
StatusCodes
��. 9
.
��9 :
Status200OK
��: E
)
��E F
]
��F G
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
byte
��' +
[
��+ ,
]
��, -
>
��- .
>
��. /
DownloadFile
��0 <
(
��< =
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1
DownloadFileQuery
��2 C
(
��C D
id
��D F
:
��F G
id
��H J
)
��J K
,
��K L
cancellationToken
��M ^
)
��^ _
;
��_ `
if
�� 
(
�� 
result
�� 
==
�� 
null
�� 
)
�� 
{
�� 
return
�� 
NotFound
�� 
(
��  
)
��  !
;
��! "
}
�� 
return
�� 
File
�� 
(
�� 
result
�� 
.
�� 
Content
�� &
,
��& '
result
��( .
.
��. /
ContentType
��/ :
??
��; =
$str
��> X
,
��X Y
result
��Z `
.
��` a
Filename
��a i
)
��i j
;
��j k
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 3
)
��3 4
]
��4 5
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
byte
��% )
[
��) *
]
��* +
)
��+ ,
,
��, -
StatusCodes
��. 9
.
��9 :
Status200OK
��: E
)
��E F
]
��F G
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
byte
��' +
[
��+ ,
]
��, -
>
��- .
>
��. /
SimpleDownload
��0 >
(
��> ?
[
�� 
	FromQuery
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1!
SimpleDownloadQuery
��2 E
(
��E F
id
��F H
:
��H I
id
��J L
)
��L M
,
��M N
cancellationToken
��O `
)
��` a
;
��a b
if
�� 
(
�� 
result
�� 
==
�� 
null
�� 
)
�� 
{
�� 
return
�� 
NotFound
�� 
(
��  
)
��  !
;
��! "
}
�� 
return
�� 
File
�� 
(
�� 
result
�� 
.
�� 
Content
�� &
,
��& '
$str
��( B
)
��B C
;
��C D
}
�� 	
}
�� 
}�� �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\FileTransfer\BinaryContentAttribute.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
.@ A
FileTransferA M
{ 
[		 
AttributeUsage		 
(		 
AttributeTargets		 $
.		$ %
Method		% +
)		+ ,
]		, -
public

 

class

 "
BinaryContentAttribute

 '
:

( )
	Attribute

* 3
{ 
} 
} ��
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\CustomersController.cs
["" 
assembly"" 	
:""	 
 
DefaultIntentManaged"" 
(""  
Mode""  $
.""$ %
Fully""% *
)""* +
]""+ ,
[## 
assembly## 	
:##	 
 
DefaultIntentManaged## 
(##  
Mode##  $
.##$ %
Fully##% *
,##* +
Targets##, 3
=##4 5
Targets##6 =
.##= >
Usings##> D
)##D E
]##E F
[$$ 
assembly$$ 	
:$$	 

IntentTemplate$$ 
($$ 
$str$$ D
,$$D E
Version$$F M
=$$N O
$str$$P U
)$$U V
]$$V W
	namespace&& 	
AdvancedMappingCrud&&
 
.&& 
Repositories&& *
.&&* +
Tests&&+ 0
.&&0 1
Api&&1 4
.&&4 5
Controllers&&5 @
{'' 
[(( 
ApiController(( 
](( 
public)) 

class)) 
CustomersController)) $
:))% &
ControllerBase))' 5
{** 
private++ 
readonly++ 
ISender++  
	_mediator++! *
;++* +
public-- 
CustomersController-- "
(--" #
ISender--# *
mediator--+ 3
)--3 4
{.. 	
	_mediator// 
=// 
mediator//  
??//! #
throw//$ )
new//* -!
ArgumentNullException//. C
(//C D
nameof//D J
(//J K
mediator//K S
)//S T
)//T U
;//U V
}00 	
[77 	
HttpPost77	 
(77 
$str77 3
)773 4
]774 5
[88 	 
ProducesResponseType88	 
(88 
StatusCodes88 )
.88) *
Status201Created88* :
)88: ;
]88; <
[99 	 
ProducesResponseType99	 
(99 
StatusCodes99 )
.99) *
Status400BadRequest99* =
)99= >
]99> ?
[:: 	 
ProducesResponseType::	 
(:: 
StatusCodes:: )
.::) *
Status404NotFound::* ;
)::; <
]::< =
[;; 	 
ProducesResponseType;;	 
(;; 
typeof;; $
(;;$ %
ProblemDetails;;% 3
);;3 4
,;;4 5
StatusCodes;;6 A
.;;A B(
Status500InternalServerError;;B ^
);;^ _
];;_ `
public<< 
async<< 
Task<< 
<<< 
ActionResult<< &
><<& '
ApproveQuote<<( 4
(<<4 5
[== 
	FromRoute== 
]== 
Guid== 
quoteId== $
,==$ %
[>> 
FromBody>> 
]>> 
ApproveQuoteCommand>> *
command>>+ 2
,>>2 3
CancellationToken?? 
cancellationToken?? /
=??0 1
default??2 9
)??9 :
{@@ 	
ifAA 
(AA 
commandAA 
.AA 
QuoteIdAA 
==AA  "
GuidAA# '
.AA' (
EmptyAA( -
)AA- .
{BB 
commandCC 
.CC 
QuoteIdCC 
=CC  !
quoteIdCC" )
;CC) *
}DD 
ifFF 
(FF 
quoteIdFF 
!=FF 
commandFF "
.FF" #
QuoteIdFF# *
)FF* +
{GG 
returnHH 

BadRequestHH !
(HH! "
)HH" #
;HH# $
}II 
awaitKK 
	_mediatorKK 
.KK 
SendKK  
(KK  !
commandKK! (
,KK( )
cancellationTokenKK* ;
)KK; <
;KK< =
returnLL 
CreatedLL 
(LL 
stringLL !
.LL! "
EmptyLL" '
,LL' (
nullLL) -
)LL- .
;LL. /
}MM 	
[SS 	
HttpPostSS	 
(SS 
$strSS .
)SS. /
]SS/ 0
[TT 	 
ProducesResponseTypeTT	 
(TT 
StatusCodesTT )
.TT) *
Status201CreatedTT* :
)TT: ;
]TT; <
[UU 	 
ProducesResponseTypeUU	 
(UU 
StatusCodesUU )
.UU) *
Status400BadRequestUU* =
)UU= >
]UU> ?
[VV 	 
ProducesResponseTypeVV	 
(VV 
typeofVV $
(VV$ %
ProblemDetailsVV% 3
)VV3 4
,VV4 5
StatusCodesVV6 A
.VVA B(
Status500InternalServerErrorVVB ^
)VV^ _
]VV_ `
publicWW 
asyncWW 
TaskWW 
<WW 
ActionResultWW &
>WW& ',
 CreateCorporateFuneralCoverQuoteWW( H
(WWH I
[XX 
FromBodyXX 
]XX 3
'CreateCorporateFuneralCoverQuoteCommandXX >
commandXX? F
,XXF G
CancellationTokenYY 
cancellationTokenYY /
=YY0 1
defaultYY2 9
)YY9 :
{ZZ 	
await[[ 
	_mediator[[ 
.[[ 
Send[[  
([[  !
command[[! (
,[[( )
cancellationToken[[* ;
)[[; <
;[[< =
return\\ 
Created\\ 
(\\ 
string\\ !
.\\! "
Empty\\" '
,\\' (
null\\) -
)\\- .
;\\. /
}]] 	
[cc 	
HttpPostcc	 
(cc 
$strcc  
)cc  !
]cc! "
[dd 	
Producesdd	 
(dd 
MediaTypeNamesdd  
.dd  !
Applicationdd! ,
.dd, -
Jsondd- 1
)dd1 2
]dd2 3
[ee 	 
ProducesResponseTypeee	 
(ee 
typeofee $
(ee$ %
JsonResponseee% 1
<ee1 2
Guidee2 6
>ee6 7
)ee7 8
,ee8 9
StatusCodesee: E
.eeE F
Status201CreatedeeF V
)eeV W
]eeW X
[ff 	 
ProducesResponseTypeff	 
(ff 
StatusCodesff )
.ff) *
Status400BadRequestff* =
)ff= >
]ff> ?
[gg 	 
ProducesResponseTypegg	 
(gg 
typeofgg $
(gg$ %
ProblemDetailsgg% 3
)gg3 4
,gg4 5
StatusCodesgg6 A
.ggA B(
Status500InternalServerErrorggB ^
)gg^ _
]gg_ `
publichh 
asynchh 
Taskhh 
<hh 
ActionResulthh &
<hh& '
JsonResponsehh' 3
<hh3 4
Guidhh4 8
>hh8 9
>hh9 :
>hh: ;
CreateCustomerhh< J
(hhJ K
[ii 
FromBodyii 
]ii !
CreateCustomerCommandii ,
commandii- 4
,ii4 5
CancellationTokenjj 
cancellationTokenjj /
=jj0 1
defaultjj2 9
)jj9 :
{kk 	
varll 
resultll 
=ll 
awaitll 
	_mediatorll (
.ll( )
Sendll) -
(ll- .
commandll. 5
,ll5 6
cancellationTokenll7 H
)llH I
;llI J
returnmm 
CreatedAtActionmm "
(mm" #
nameofmm# )
(mm) *
GetCustomerByIdmm* 9
)mm9 :
,mm: ;
newmm< ?
{mm@ A
idmmB D
=mmE F
resultmmG M
}mmN O
,mmO P
newmmQ T
JsonResponsemmU a
<mma b
Guidmmb f
>mmf g
(mmg h
resultmmh n
)mmn o
)mmo p
;mmp q
}nn 	
[tt 	
HttpPosttt	 
(tt 
$strtt )
)tt) *
]tt* +
[uu 	 
ProducesResponseTypeuu	 
(uu 
StatusCodesuu )
.uu) *
Status201Createduu* :
)uu: ;
]uu; <
[vv 	 
ProducesResponseTypevv	 
(vv 
StatusCodesvv )
.vv) *
Status400BadRequestvv* =
)vv= >
]vv> ?
[ww 	 
ProducesResponseTypeww	 
(ww 
typeofww $
(ww$ %
ProblemDetailsww% 3
)ww3 4
,ww4 5
StatusCodesww6 A
.wwA B(
Status500InternalServerErrorwwB ^
)ww^ _
]ww_ `
publicxx 
asyncxx 
Taskxx 
<xx 
ActionResultxx &
>xx& '#
CreateFuneralCoverQuotexx( ?
(xx? @
[yy 
FromBodyyy 
]yy *
CreateFuneralCoverQuoteCommandyy 5
commandyy6 =
,yy= >
CancellationTokenzz 
cancellationTokenzz /
=zz0 1
defaultzz2 9
)zz9 :
{{{ 	
await|| 
	_mediator|| 
.|| 
Send||  
(||  !
command||! (
,||( )
cancellationToken||* ;
)||; <
;||< =
return}} 
Created}} 
(}} 
string}} !
.}}! "
Empty}}" '
,}}' (
null}}) -
)}}- .
;}}. /
}~~ 	
[
�� 	
HttpPost
��	 
(
�� 
$str
�� !
)
��! "
]
��" #
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status201Created
��* :
)
��: ;
]
��; <
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '
CreateQuote
��( 3
(
��3 4
[
�� 
FromBody
�� 
]
��  
CreateQuoteCommand
�� )
command
��* 1
,
��1 2
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
Created
�� 
(
�� 
string
�� !
.
��! "
Empty
��" '
,
��' (
null
��) -
)
��- .
;
��. /
}
�� 	
[
�� 	
HttpPut
��	 
(
�� 
$str
�� 4
)
��4 5
]
��5 6
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) * 
Status204NoContent
��* <
)
��< =
]
��= >
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& ' 
DeactivateCustomer
��( :
(
��: ;
[
�� 
FromBody
�� 
]
�� '
DeactivateCustomerCommand
�� 0
command
��1 8
,
��8 9
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	

HttpDelete
��	 
(
�� 
$str
�� '
)
��' (
]
��( )
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status200OK
��* 5
)
��5 6
]
��6 7
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '
DeleteCustomer
��( 6
(
��6 7
[
��7 8
	FromRoute
��8 A
]
��A B
Guid
��C G
id
��H J
,
��J K
CancellationToken
��L ]
cancellationToken
��^ o
=
��p q
default
��r y
)
��y z
{
�� 	
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
new
��! $#
DeleteCustomerCommand
��% :
(
��: ;
id
��; =
:
��= >
id
��? A
)
��A B
,
��B C
cancellationToken
��D U
)
��U V
;
��V W
return
�� 
Ok
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpPut
��	 
(
�� 
$str
�� %
)
��% &
]
��& '
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) * 
Status204NoContent
��* <
)
��< =
]
��= >
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '.
 UpdateCorporateFuneralCoverQuote
��( H
(
��H I
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
[
�� 
FromBody
�� 
]
�� 5
'UpdateCorporateFuneralCoverQuoteCommand
�� >
command
��? F
,
��F G
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
if
�� 
(
�� 
command
�� 
.
�� 
Id
�� 
==
�� 
Guid
�� "
.
��" #
Empty
��# (
)
��( )
{
�� 
command
�� 
.
�� 
Id
�� 
=
�� 
id
�� 
;
��  
}
�� 
if
�� 
(
�� 
id
�� 
!=
�� 
command
�� 
.
�� 
Id
��  
)
��  !
{
�� 
return
�� 

BadRequest
�� !
(
��! "
)
��" #
;
��# $
}
�� 
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpPut
��	 
(
�� 
$str
�� $
)
��$ %
]
��% &
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) * 
Status204NoContent
��* <
)
��< =
]
��= >
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
>
��& '
UpdateCustomer
��( 6
(
��6 7
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
[
�� 
FromBody
�� 
]
�� #
UpdateCustomerCommand
�� ,
command
��- 4
,
��4 5
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
if
�� 
(
�� 
command
�� 
.
�� 
Id
�� 
==
�� 
Guid
�� "
.
��" #
Empty
��# (
)
��( )
{
�� 
command
�� 
.
�� 
Id
�� 
=
�� 
id
�� 
;
��  
}
�� 
if
�� 
(
�� 
id
�� 
!=
�� 
command
�� 
.
�� 
Id
��  
)
��  !
{
�� 
return
�� 

BadRequest
�� !
(
��! "
)
��" #
;
��# $
}
�� 
await
�� 
	_mediator
�� 
.
�� 
Send
��  
(
��  !
command
��! (
,
��( )
cancellationToken
��* ;
)
��; <
;
��< =
return
�� 
	NoContent
�� 
(
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� $
)
��$ %
]
��% &
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
CustomerDto
��% 0
)
��0 1
,
��1 2
StatusCodes
��3 >
.
��> ?
Status200OK
��? J
)
��J K
]
��K L
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
CustomerDto
��' 2
>
��2 3
>
��3 4
GetCustomerById
��5 D
(
��D E
[
�� 
	FromRoute
�� 
]
�� 
Guid
�� 
id
�� 
,
��  
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1"
GetCustomerByIdQuery
��2 F
(
��F G
id
��G I
:
��I J
id
��K M
)
��M N
,
��N O
cancellationToken
��P a
)
��a b
;
��b c
return
�� 
result
�� 
==
�� 
null
�� !
?
��" #
NotFound
��$ ,
(
��, -
)
��- .
:
��/ 0
Ok
��1 3
(
��3 4
result
��4 :
)
��: ;
;
��; <
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 0
)
��0 1
]
��1 2
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
List
��% )
<
��) *
CustomerDto
��* 5
>
��5 6
)
��6 7
,
��7 8
StatusCodes
��9 D
.
��D E
Status200OK
��E P
)
��P Q
]
��Q R
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
List
��' +
<
��+ ,
CustomerDto
��, 7
>
��7 8
>
��8 9
>
��9 :*
GetCustomersByNameAndSurname
��; W
(
��W X
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
name
�� #
,
��# $
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
surname
�� &
,
��& '
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1/
!GetCustomersByNameAndSurnameQuery
��2 S
(
��S T
name
��T X
:
��X Y
name
��Z ^
,
��^ _
surname
��` g
:
��g h
surname
��i p
)
��p q
,
��q r 
cancellationToken��s �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� A
)
��A B
]
��B C
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
PagedResult
��% 0
<
��0 1
CustomerDto
��1 <
>
��< =
)
��= >
,
��> ?
StatusCodes
��@ K
.
��K L
Status200OK
��L W
)
��W X
]
��X Y
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
PagedResult
��' 2
<
��2 3
CustomerDto
��3 >
>
��> ?
>
��? @
>
��@ A#
GetCustomersPaginated
��B W
(
��W X
[
�� 
	FromRoute
�� 
]
�� 
bool
�� 
isActive
�� %
,
��% &
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
name
�� #
,
��# $
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
surname
�� &
,
��& '
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageNo
�� "
,
��" #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1(
GetCustomersPaginatedQuery
��2 L
(
��L M
isActive
��M U
:
��U V
isActive
��W _
,
��_ `
name
��a e
:
��e f
name
��g k
,
��k l
surname
��m t
:
��t u
surname
��v }
,
��} ~
pageNo�� �
:��� �
pageNo��� �
,��� �
pageSize��� �
:��� �
pageSize��� �
)��� �
,��� �!
cancellationToken��� �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� I
)
��I J
]
��J K
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
PagedResult
��% 0
<
��0 1
CustomerDto
��1 <
>
��< =
)
��= >
,
��> ?
StatusCodes
��@ K
.
��K L
Status200OK
��L W
)
��W X
]
��X Y
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
PagedResult
��' 2
<
��2 3
CustomerDto
��3 >
>
��> ?
>
��? @
>
��@ A,
GetCustomersPaginatedWithOrder
��B `
(
��` a
[
�� 
	FromRoute
�� 
]
�� 
bool
�� 
isActive
�� %
,
��% &
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
name
�� #
,
��# $
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
surname
�� &
,
��& '
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageNo
�� "
,
��" #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
[
�� 
	FromQuery
�� 
]
�� 
string
�� 
orderBy
�� &
,
��& '
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 11
#GetCustomersPaginatedWithOrderQuery
��2 U
(
��U V
isActive
��V ^
:
��^ _
isActive
��` h
,
��h i
name
��j n
:
��n o
name
��p t
,
��t u
surname
��v }
:
��} ~
surname�� �
,��� �
pageNo��� �
:��� �
pageNo��� �
,��� �
pageSize��� �
:��� �
pageSize��� �
,��� �
orderBy��� �
:��� �
orderBy��� �
)��� �
,��� �!
cancellationToken��� �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
��  
)
��  !
]
��! "
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
List
��% )
<
��) *
CustomerDto
��* 5
>
��5 6
)
��6 7
,
��7 8
StatusCodes
��9 D
.
��D E
Status200OK
��E P
)
��P Q
]
��Q R
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
List
��' +
<
��+ ,
CustomerDto
��, 7
>
��7 8
>
��8 9
>
��9 :
GetCustomers
��; G
(
��G H
ODataQueryOptions
�� 
<
�� 
CustomerDto
�� )
>
��) *
oDataOptions
��+ 7
,
��7 8
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	"
ValidateODataOptions
��  
(
��  !
oDataOptions
��! -
)
��- .
;
��. /
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1
GetCustomersQuery
��2 C
(
��C D
oDataOptions
��D P
.
��P Q
ApplyTo
��Q X
)
��X Y
,
��Y Z
cancellationToken
��[ l
)
��l m
;
��m n
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� +
)
��+ ,
]
��, -
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
int
��% (
)
��( )
,
��) *
StatusCodes
��+ 6
.
��6 7
Status200OK
��7 B
)
��B C
]
��C D
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *
Status404NotFound
��* ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
int
��' *
>
��* +
>
��+ ,#
GetCustomerStatistics
��- B
(
��B C
[
�� 
	FromQuery
�� 
]
�� 
Guid
�� 

customerId
�� '
,
��' (
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1(
GetCustomerStatisticsQuery
��2 L
(
��L M

customerId
��M W
:
��W X

customerId
��Y c
)
��c d
,
��d e
cancellationToken
��f w
)
��w x
;
��x y
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� ;
)
��; <
]
��< =
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
List
��% )
<
��) *
CustomerDto
��* 5
>
��5 6
)
��6 7
,
��7 8
StatusCodes
��9 D
.
��D E
Status200OK
��E P
)
��P Q
]
��Q R
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
List
��' +
<
��+ ,
CustomerDto
��, 7
>
��7 8
>
��8 9
>
��9 :$
GetCustomersWithParams
��; Q
(
��Q R
[
�� 
	FromRoute
�� 
]
�� 
bool
�� 
isActive
�� %
,
��% &
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
?
�� 
name
��  $
,
��$ %
[
�� 
	FromRoute
�� 
]
�� 
string
�� 
?
�� 
surname
��  '
,
��' (
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1)
GetCustomersWithParamsQuery
��2 M
(
��M N
isActive
��N V
:
��V W
isActive
��X `
,
��` a
name
��b f
:
��f g
name
��h l
,
��l m
surname
��n u
:
��u v
surname
��w ~
)
��~ 
,�� �!
cancellationToken��� �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
private
�� 
static
�� 
void
�� "
ValidateODataOptions
�� 0
<
��0 1
TDto
��1 5
>
��5 6
(
��6 7
ODataQueryOptions
��7 H
<
��H I
TDto
��I M
>
��M N
options
��O V
,
��V W
bool
��X \
enableSelect
��] i
=
��j k
false
��l q
)
��q r
{
�� 	
var
�� 
settings
�� 
=
�� 
new
�� %
ODataValidationSettings
�� 6
(
��6 7
)
��7 8
;
��8 9
if
�� 
(
�� 
!
�� 
enableSelect
�� 
)
�� 
{
�� 
settings
�� 
.
�� !
AllowedQueryOptions
�� ,
=
��- .!
AllowedQueryOptions
��/ B
.
��B C
All
��C F
&
��G H
~
��I J!
AllowedQueryOptions
��J ]
.
��] ^
Select
��^ d
;
��d e
}
�� 
options
�� 
.
�� 
Validate
�� 
(
�� 
settings
�� %
)
��% &
;
��& '
}
�� 	
}
�� 
}�� �q
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\BasicsController.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Controllers5 @
{ 
[ 
ApiController 
] 
public 

class 
BasicsController !
:" #
ControllerBase$ 2
{ 
private 
readonly 
ISender  
	_mediator! *
;* +
public!! 
BasicsController!! 
(!!  
ISender!!  '
mediator!!( 0
)!!0 1
{"" 	
	_mediator## 
=## 
mediator##  
??##! #
throw##$ )
new##* -!
ArgumentNullException##. C
(##C D
nameof##D J
(##J K
mediator##K S
)##S T
)##T U
;##U V
}$$ 	
[** 	
HttpPost**	 
(** 
$str** 
)** 
]**  
[++ 	
Produces++	 
(++ 
MediaTypeNames++  
.++  !
Application++! ,
.++, -
Json++- 1
)++1 2
]++2 3
[,, 	 
ProducesResponseType,,	 
(,, 
typeof,, $
(,,$ %
JsonResponse,,% 1
<,,1 2
Guid,,2 6
>,,6 7
),,7 8
,,,8 9
StatusCodes,,: E
.,,E F
Status201Created,,F V
),,V W
],,W X
[-- 	 
ProducesResponseType--	 
(-- 
StatusCodes-- )
.--) *
Status400BadRequest--* =
)--= >
]--> ?
[.. 	 
ProducesResponseType..	 
(.. 
typeof.. $
(..$ %
ProblemDetails..% 3
)..3 4
,..4 5
StatusCodes..6 A
...A B(
Status500InternalServerError..B ^
)..^ _
].._ `
public// 
async// 
Task// 
<// 
ActionResult// &
<//& '
JsonResponse//' 3
<//3 4
Guid//4 8
>//8 9
>//9 :
>//: ;
CreateBasic//< G
(//G H
[00 
FromBody00 
]00 
CreateBasicCommand00 )
command00* 1
,001 2
CancellationToken11 
cancellationToken11 /
=110 1
default112 9
)119 :
{22 	
var33 
result33 
=33 
await33 
	_mediator33 (
.33( )
Send33) -
(33- .
command33. 5
,335 6
cancellationToken337 H
)33H I
;33I J
return44 
CreatedAtAction44 "
(44" #
nameof44# )
(44) *
GetBasicById44* 6
)446 7
,447 8
new449 <
{44= >
id44? A
=44B C
result44D J
}44K L
,44L M
new44N Q
JsonResponse44R ^
<44^ _
Guid44_ c
>44c d
(44d e
result44e k
)44k l
)44l m
;44m n
}55 	
[<< 	

HttpDelete<<	 
(<< 
$str<< %
)<<% &
]<<& '
[== 	 
ProducesResponseType==	 
(== 
StatusCodes== )
.==) *
Status200OK==* 5
)==5 6
]==6 7
[>> 	 
ProducesResponseType>>	 
(>> 
StatusCodes>> )
.>>) *
Status400BadRequest>>* =
)>>= >
]>>> ?
[?? 	 
ProducesResponseType??	 
(?? 
StatusCodes?? )
.??) *
Status404NotFound??* ;
)??; <
]??< =
[@@ 	 
ProducesResponseType@@	 
(@@ 
typeof@@ $
(@@$ %
ProblemDetails@@% 3
)@@3 4
,@@4 5
StatusCodes@@6 A
.@@A B(
Status500InternalServerError@@B ^
)@@^ _
]@@_ `
publicAA 
asyncAA 
TaskAA 
<AA 
ActionResultAA &
>AA& '
DeleteBasicAA( 3
(AA3 4
[AA4 5
	FromRouteAA5 >
]AA> ?
GuidAA@ D
idAAE G
,AAG H
CancellationTokenAAI Z
cancellationTokenAA[ l
=AAm n
defaultAAo v
)AAv w
{BB 	
awaitCC 
	_mediatorCC 
.CC 
SendCC  
(CC  !
newCC! $
DeleteBasicCommandCC% 7
(CC7 8
idCC8 :
:CC: ;
idCC< >
)CC> ?
,CC? @
cancellationTokenCCA R
)CCR S
;CCS T
returnDD 
OkDD 
(DD 
)DD 
;DD 
}EE 	
[LL 	
HttpPutLL	 
(LL 
$strLL "
)LL" #
]LL# $
[MM 	 
ProducesResponseTypeMM	 
(MM 
StatusCodesMM )
.MM) *
Status204NoContentMM* <
)MM< =
]MM= >
[NN 	 
ProducesResponseTypeNN	 
(NN 
StatusCodesNN )
.NN) *
Status400BadRequestNN* =
)NN= >
]NN> ?
[OO 	 
ProducesResponseTypeOO	 
(OO 
StatusCodesOO )
.OO) *
Status404NotFoundOO* ;
)OO; <
]OO< =
[PP 	 
ProducesResponseTypePP	 
(PP 
typeofPP $
(PP$ %
ProblemDetailsPP% 3
)PP3 4
,PP4 5
StatusCodesPP6 A
.PPA B(
Status500InternalServerErrorPPB ^
)PP^ _
]PP_ `
publicQQ 
asyncQQ 
TaskQQ 
<QQ 
ActionResultQQ &
>QQ& '
UpdateBasicQQ( 3
(QQ3 4
[RR 
	FromRouteRR 
]RR 
GuidRR 
idRR 
,RR  
[SS 
FromBodySS 
]SS 
UpdateBasicCommandSS )
commandSS* 1
,SS1 2
CancellationTokenTT 
cancellationTokenTT /
=TT0 1
defaultTT2 9
)TT9 :
{UU 	
ifVV 
(VV 
commandVV 
.VV 
IdVV 
==VV 
GuidVV "
.VV" #
EmptyVV# (
)VV( )
{WW 
commandXX 
.XX 
IdXX 
=XX 
idXX 
;XX  
}YY 
if[[ 
([[ 
id[[ 
!=[[ 
command[[ 
.[[ 
Id[[  
)[[  !
{\\ 
return]] 

BadRequest]] !
(]]! "
)]]" #
;]]# $
}^^ 
await`` 
	_mediator`` 
.`` 
Send``  
(``  !
command``! (
,``( )
cancellationToken``* ;
)``; <
;``< =
returnaa 
	NoContentaa 
(aa 
)aa 
;aa 
}bb 	
[ii 	
HttpGetii	 
(ii 
$strii "
)ii" #
]ii# $
[jj 	 
ProducesResponseTypejj	 
(jj 
typeofjj $
(jj$ %
BasicDtojj% -
)jj- .
,jj. /
StatusCodesjj0 ;
.jj; <
Status200OKjj< G
)jjG H
]jjH I
[kk 	 
ProducesResponseTypekk	 
(kk 
StatusCodeskk )
.kk) *
Status400BadRequestkk* =
)kk= >
]kk> ?
[ll 	 
ProducesResponseTypell	 
(ll 
StatusCodesll )
.ll) *
Status404NotFoundll* ;
)ll; <
]ll< =
[mm 	 
ProducesResponseTypemm	 
(mm 
typeofmm $
(mm$ %
ProblemDetailsmm% 3
)mm3 4
,mm4 5
StatusCodesmm6 A
.mmA B(
Status500InternalServerErrormmB ^
)mm^ _
]mm_ `
publicnn 
asyncnn 
Tasknn 
<nn 
ActionResultnn &
<nn& '
BasicDtonn' /
>nn/ 0
>nn0 1
GetBasicByIdnn2 >
(nn> ?
[oo 
	FromRouteoo 
]oo 
Guidoo 
idoo 
,oo  
CancellationTokenpp 
cancellationTokenpp /
=pp0 1
defaultpp2 9
)pp9 :
{qq 	
varrr 
resultrr 
=rr 
awaitrr 
	_mediatorrr (
.rr( )
Sendrr) -
(rr- .
newrr. 1
GetBasicByIdQueryrr2 C
(rrC D
idrrD F
:rrF G
idrrH J
)rrJ K
,rrK L
cancellationTokenrrM ^
)rr^ _
;rr_ `
returnss 
resultss 
==ss 
nullss !
?ss" #
NotFoundss$ ,
(ss, -
)ss- .
:ss/ 0
Okss1 3
(ss3 4
resultss4 :
)ss: ;
;ss; <
}tt 	
[zz 	
HttpGetzz	 
(zz 
$strzz &
)zz& '
]zz' (
[{{ 	 
ProducesResponseType{{	 
({{ 
typeof{{ $
({{$ %
PagedResult{{% 0
<{{0 1
BasicDto{{1 9
>{{9 :
){{: ;
,{{; <
StatusCodes{{= H
.{{H I
Status200OK{{I T
){{T U
]{{U V
[|| 	 
ProducesResponseType||	 
(|| 
StatusCodes|| )
.||) *
Status400BadRequest||* =
)||= >
]||> ?
[}} 	 
ProducesResponseType}}	 
(}} 
typeof}} $
(}}$ %
ProblemDetails}}% 3
)}}3 4
,}}4 5
StatusCodes}}6 A
.}}A B(
Status500InternalServerError}}B ^
)}}^ _
]}}_ `
public~~ 
async~~ 
Task~~ 
<~~ 
ActionResult~~ &
<~~& '
PagedResult~~' 2
<~~2 3
BasicDto~~3 ;
>~~; <
>~~< =
>~~= >
GetBasicsNullable~~? P
(~~P Q
[ 
	FromQuery 
] 
int 
pageNo "
," #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
[
�� 
	FromQuery
�� 
]
�� 
string
�� 
?
�� 
orderBy
��  '
,
��' (
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1$
GetBasicsNullableQuery
��2 H
(
��H I
pageNo
��I O
:
��O P
pageNo
��Q W
,
��W X
pageSize
��Y a
:
��a b
pageSize
��c k
,
��k l
orderBy
��m t
:
��t u
orderBy
��v }
)
��} ~
,
��~ !
cancellationToken��� �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
[
�� 	
HttpGet
��	 
(
�� 
$str
�� 
)
�� 
]
�� 
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
PagedResult
��% 0
<
��0 1
BasicDto
��1 9
>
��9 :
)
��: ;
,
��; <
StatusCodes
��= H
.
��H I
Status200OK
��I T
)
��T U
]
��U V
[
�� 	"
ProducesResponseType
��	 
(
�� 
StatusCodes
�� )
.
��) *!
Status400BadRequest
��* =
)
��= >
]
��> ?
[
�� 	"
ProducesResponseType
��	 
(
�� 
typeof
�� $
(
��$ %
ProblemDetails
��% 3
)
��3 4
,
��4 5
StatusCodes
��6 A
.
��A B*
Status500InternalServerError
��B ^
)
��^ _
]
��_ `
public
�� 
async
�� 
Task
�� 
<
�� 
ActionResult
�� &
<
��& '
PagedResult
��' 2
<
��2 3
BasicDto
��3 ;
>
��; <
>
��< =
>
��= >
	GetBasics
��? H
(
��H I
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageNo
�� "
,
��" #
[
�� 
	FromQuery
�� 
]
�� 
int
�� 
pageSize
�� $
,
��$ %
[
�� 
	FromQuery
�� 
]
�� 
string
�� 
orderBy
�� &
,
��& '
CancellationToken
�� 
cancellationToken
�� /
=
��0 1
default
��2 9
)
��9 :
{
�� 	
var
�� 
result
�� 
=
�� 
await
�� 
	_mediator
�� (
.
��( )
Send
��) -
(
��- .
new
��. 1
GetBasicsQuery
��2 @
(
��@ A
pageNo
��A G
:
��G H
pageNo
��I O
,
��O P
pageSize
��Q Y
:
��Y Z
pageSize
��[ c
,
��c d
orderBy
��e l
:
��l m
orderBy
��n u
)
��u v
,
��v w 
cancellationToken��x �
)��� �
;��� �
return
�� 
Ok
�� 
(
�� 
result
�� 
)
�� 
;
�� 
}
�� 	
}
�� 
}�� �Z
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\SwashbuckleConfiguration.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str R
,R S
VersionT [
=\ ]
$str^ c
)c d
]d e
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Configuration5 B
{ 
public 

static 
class $
SwashbuckleConfiguration 0
{ 
public 
static 
IServiceCollection (
ConfigureSwagger) 9
(9 :
this: >
IServiceCollection? Q
servicesR Z
,Z [
IConfiguration\ j
configurationk x
)x y
{ 	
services 
. 
AddTransient !
<! "
IConfigureOptions" 3
<3 4
SwaggerGenOptions4 E
>E F
,F G'
ApiVersionSwaggerGenOptionsH c
>c d
(d e
)e f
;f g
services 
. 
AddSwaggerGen "
(" #
options 
=> 
{   
options!! 
.!! 
SchemaFilter!! (
<!!( )4
(RequireNonNullablePropertiesSchemaFilter!!) Q
>!!Q R
(!!R S
)!!S T
;!!T U
options"" 
."" ,
 SupportNonNullableReferenceTypes"" <
(""< =
)""= >
;""> ?
options## 
.## 
CustomSchemaIds## +
(##+ ,
x##, -
=>##. 0
x##1 2
.##2 3
FullName##3 ;
?##; <
.##< =
Replace##= D
(##D E
$str##E H
,##H I
$str##J M
,##M N
StringComparison##O _
.##_ `
OrdinalIgnoreCase##` q
)##q r
)##r s
;##s t
var%% 

apiXmlFile%% "
=%%# $
Path%%% )
.%%) *
Combine%%* 1
(%%1 2

AppContext%%2 <
.%%< =
BaseDirectory%%= J
,%%J K
$"%%L N
{%%N O
Assembly%%O W
.%%W X 
GetExecutingAssembly%%X l
(%%l m
)%%m n
.%%n o
GetName%%o v
(%%v w
)%%w x
.%%x y
Name%%y }
}%%} ~
$str	%%~ �
"
%%� �
)
%%� �
;
%%� �
if&& 
(&& 
File&& 
.&& 
Exists&& #
(&&# $

apiXmlFile&&$ .
)&&. /
)&&/ 0
{'' 
options(( 
.((  
IncludeXmlComments((  2
(((2 3

apiXmlFile((3 =
)((= >
;((> ?
})) 
var++ 
applicationXmlFile++ *
=+++ ,
Path++- 1
.++1 2
Combine++2 9
(++9 :

AppContext++: D
.++D E
BaseDirectory++E R
,++R S
$"++T V
{++V W
typeof++W ]
(++] ^
DependencyInjection++^ q
)++q r
.++r s
Assembly++s {
.++{ |
GetName	++| �
(
++� �
)
++� �
.
++� �
Name
++� �
}
++� �
$str
++� �
"
++� �
)
++� �
;
++� �
if,, 
(,, 
File,, 
.,, 
Exists,, #
(,,# $
applicationXmlFile,,$ 6
),,6 7
),,7 8
{-- 
options.. 
...  
IncludeXmlComments..  2
(..2 3
applicationXmlFile..3 E
)..E F
;..F G
}// 
options00 
.00 
OperationFilter00 +
<00+ ,
BinaryContentFilter00, ?
>00? @
(00@ A
)00A B
;00B C
options11 
.11 
OperationFilter11 +
<11+ ,
ODataQueryFilter11, <
>11< =
(11= >
)11> ?
;11? @
options22 
.22 
OperationFilter22 +
<22+ ,)
AuthorizeCheckOperationFilter22, I
>22I J
(22J K
)22K L
;22L M
var44 
securityScheme44 &
=44' (
new44) ,!
OpenApiSecurityScheme44- B
(44B C
)44C D
{55 
Name66 
=66 
$str66 .
,66. /
Description77 #
=77$ %
$str	77& �
,
77� �
In88 
=88 
ParameterLocation88 .
.88. /
Header88/ 5
,885 6
Type99 
=99 
SecuritySchemeType99 1
.991 2
Http992 6
,996 7
Scheme:: 
=::  
$str::! )
,::) *
BearerFormat;; $
=;;% &
$str;;' ,
,;;, -
	Reference<< !
=<<" #
new<<$ '
OpenApiReference<<( 8
{== 
Id>> 
=>>  
JwtBearerDefaults>>! 2
.>>2 3 
AuthenticationScheme>>3 G
,>>G H
Type??  
=??! "
ReferenceType??# 0
.??0 1
SecurityScheme??1 ?
}@@ 
}AA 
;AA 
optionsCC 
.CC !
AddSecurityDefinitionCC 1
(CC1 2
$strCC2 :
,CC: ;
securitySchemeCC< J
)CCJ K
;CCK L
optionsDD 
.DD "
AddSecurityRequirementDD 2
(DD2 3
newEE &
OpenApiSecurityRequirementEE 6
{FF 
{GG 
securitySchemeGG ,
,GG, -
ArrayGG. 3
.GG3 4
EmptyGG4 9
<GG9 :
stringGG: @
>GG@ A
(GGA B
)GGB C
}GGD E
}HH 
)HH 
;HH 
optionsII 
.II 
SchemaFilterII (
<II( )
TypeSchemaFilterII) 9
>II9 :
(II: ;
)II; <
;II< =
}JJ 
)JJ 
;JJ 
returnKK 
servicesKK 
;KK 
}LL 	
publicNN 
staticNN 
voidNN 
UseSwashbuckleNN )
(NN) *
thisNN* .
IApplicationBuilderNN/ B
appNNC F
,NNF G
IConfigurationNNH V
configurationNNW d
)NNd e
{OO 	
appPP 
.PP 

UseSwaggerPP 
(PP 
)PP 
;PP 
appQQ 
.QQ 
UseSwaggerUIQQ 
(QQ 
optionsRR 
=>RR 
{SS 
optionsTT 
.TT 
RoutePrefixTT '
=TT( )
$strTT* 3
;TT3 4
optionsUU 
.UU 
OAuthAppNameUU (
(UU( )
$strUU) U
)UUU V
;UUV W
optionsVV 
.VV 
EnableDeepLinkingVV -
(VV- .
)VV. /
;VV/ 0
optionsWW 
.WW 
DisplayOperationIdWW .
(WW. /
)WW/ 0
;WW0 1
optionsXX 
.XX $
DefaultModelsExpandDepthXX 4
(XX4 5
$numXX5 6
)XX6 7
;XX7 8
optionsYY 
.YY !
DefaultModelRenderingYY 1
(YY1 2
ModelRenderingYY2 @
.YY@ A
ExampleYYA H
)YYH I
;YYI J
optionsZZ 
.ZZ 
DocExpansionZZ (
(ZZ( )
DocExpansionZZ) 5
.ZZ5 6
ListZZ6 :
)ZZ: ;
;ZZ; <
options[[ 
.[[ 
ShowExtensions[[ *
([[* +
)[[+ ,
;[[, -
options\\ 
.\\ 
EnableFilter\\ (
(\\( )
string\\) /
.\\/ 0
Empty\\0 5
)\\5 6
;\\6 7
AddSwaggerEndpoints]] '
(]]' (
app]]( +
,]]+ ,
options]]- 4
)]]4 5
;]]5 6
options^^ 
.^^ 
OAuthScopeSeparator^^ /
(^^/ 0
$str^^0 3
)^^3 4
;^^4 5
}__ 
)__ 
;__ 
}`` 	
privatebb 
staticbb 
voidbb 
AddSwaggerEndpointsbb /
(bb/ 0
IApplicationBuilderbb0 C
appbbD G
,bbG H
SwaggerUIOptionsbbI Y
optionsbbZ a
)bba b
{cc 	
vardd 
providerdd 
=dd 
appdd 
.dd 
ApplicationServicesdd 2
.dd2 3
GetRequiredServicedd3 E
<ddE F*
IApiVersionDescriptionProviderddF d
>ddd e
(dde f
)ddf g
;ddg h
foreachff 
(ff 
varff 
	groupNameff "
inff# %
providerff& .
.ff. /"
ApiVersionDescriptionsff/ E
.gg 
OrderByDescendinggg "
(gg" #
ogg# $
=>gg% '
ogg( )
.gg) *

ApiVersiongg* 4
)gg4 5
.hh 
Selecthh 
(hh 
ahh 
=>hh 
ahh 
.hh 
	GroupNamehh (
)hh( )
)hh) *
{ii 
optionsjj 
.jj 
SwaggerEndpointjj '
(jj' (
$"jj( *
$strjj* 3
{jj3 4
	groupNamejj4 =
}jj= >
$strjj> K
"jjK L
,jjL M
$"jjN P
{jjP Q
optionsjjQ X
.jjX Y
OAuthConfigObjectjjY j
.jjj k
AppNamejjk r
}jjr s
$strjjs t
{jjt u
	groupNamejju ~
}jj~ 
"	jj �
)
jj� �
;
jj� �
}kk 
}ll 	
}mm 
internaloo 
classoo 4
(RequireNonNullablePropertiesSchemaFilteroo ;
:oo< =
ISchemaFilteroo> K
{pp 
publicqq 
voidqq 
Applyqq 
(qq 
OpenApiSchemaqq '
schemaqq( .
,qq. /
SchemaFilterContextqq0 C
contextqqD K
)qqK L
{rr 	
varss #
additionalRequiredPropsss '
=ss( )
schemass* 0
.ss0 1

Propertiesss1 ;
.tt 
Wherett 
(tt 
xtt 
=>tt 
!tt 
xtt 
.tt 
Valuett $
.tt$ %
Nullablett% -
&&tt. 0
!tt1 2
schematt2 8
.tt8 9
Requiredtt9 A
.ttA B
ContainsttB J
(ttJ K
xttK L
.ttL M
KeyttM P
)ttP Q
)ttQ R
.uu 
Selectuu 
(uu 
xuu 
=>uu 
xuu 
.uu 
Keyuu "
)uu" #
;uu# $
foreachww 
(ww 
varww 
propKeyww  
inww! ##
additionalRequiredPropsww$ ;
)ww; <
{xx 
schemayy 
.yy 
Requiredyy 
.yy  
Addyy  #
(yy# $
propKeyyy$ +
)yy+ ,
;yy, -
}zz 
}{{ 	
}|| 
}}} �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ProblemDetailsConfiguration.cs
[		 
assembly		 	
:			 
 
DefaultIntentManaged		 
(		  
Mode		  $
.		$ %
Fully		% *
)		* +
]		+ ,
[

 
assembly

 	
:

	 

IntentTemplate

 
(

 
$str

 I
,

I J
Version

K R
=

S T
$str

U Z
)

Z [
]

[ \
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Configuration5 B
{ 
public 

static 
class '
ProblemDetailsConfiguration 3
{ 
public 
static 
IServiceCollection (#
ConfigureProblemDetails) @
(@ A
thisA E
IServiceCollectionF X
servicesY a
)a b
{ 	
services 
. 
AddProblemDetails &
(& '
conf' +
=>, .
conf/ 3
.3 4#
CustomizeProblemDetails4 K
=L M
contextN U
=>V X
{ 
context 
. 
ProblemDetails &
.& '
Type' +
=, -
$". 0
$str0 H
{H I
contextI P
.P Q
ProblemDetailsQ _
._ `
Status` f
}f g
"g h
;h i
if 
( 
context 
. 
ProblemDetails *
.* +
Status+ 1
!=2 4
$num5 8
)8 9
return: @
;@ A
context 
. 
ProblemDetails &
.& '
Title' ,
=- .
$str/ F
;F G
context 
. 
ProblemDetails &
.& '

Extensions' 1
.1 2
TryAdd2 8
(8 9
$str9 B
,B C
ActivityD L
.L M
CurrentM T
?T U
.U V
IdV X
??Y [
context\ c
.c d
HttpContextd o
.o p
TraceIdentifierp 
)	 �
;
� �
var 
env 
= 
context !
.! "
HttpContext" -
.- .
RequestServices. =
.= >

GetService> H
<H I
IWebHostEnvironmentI \
>\ ]
(] ^
)^ _
!_ `
;` a
if 
( 
! 
env 
. 
IsDevelopment &
(& '
)' (
)( )
return* 0
;0 1
var 
exceptionFeature $
=% &
context' .
.. /
HttpContext/ :
.: ;
Features; C
.C D
GetD G
<G H$
IExceptionHandlerFeatureH `
>` a
(a b
)b c
;c d
if 
( 
exceptionFeature $
is% '
null( ,
), -
return. 4
;4 5
context 
. 
ProblemDetails &
.& '
Detail' -
=. /
exceptionFeature0 @
.@ A
ErrorA F
.F G
ToStringG O
(O P
)P Q
;Q R
}   
)   
;   
return!! 
services!! 
;!! 
}"" 	
}## 
}$$ �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\HealthChecksConfiguration.cs
[		 
assembly		 	
:			 
 
DefaultIntentManaged		 
(		  
Mode		  $
.		$ %
Fully		% *
)		* +
]		+ ,
[

 
assembly

 	
:

	 

IntentTemplate

 
(

 
$str

 T
,

T U
Version

V ]
=

^ _
$str

` e
)

e f
]

f g
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Configuration5 B
{ 
public 

static 
class %
HealthChecksConfiguration 1
{ 
public 
static 
IServiceCollection (!
ConfigureHealthChecks) >
(> ?
this 
IServiceCollection #
services$ ,
,, -
IConfiguration 
configuration (
)( )
{ 	
var 
	hcBuilder 
= 
services $
.$ %
AddHealthChecks% 4
(4 5
)5 6
;6 7
return 
services 
; 
} 	
public 
static !
IEndpointRouteBuilder +"
MapDefaultHealthChecks, B
(B C
thisC G!
IEndpointRouteBuilderH ]
	endpoints^ g
)g h
{ 	
	endpoints 
. 
MapHealthChecks %
(% &
$str& +
,+ ,
new- 0
HealthCheckOptions1 C
{ 
	Predicate 
= 
_ 
=>  
true! %
,% &
ResponseWriter 
=  
UIResponseWriter! 1
.1 2&
WriteHealthCheckUIResponse2 L
} 
) 
; 
return   
	endpoints   
;   
}!! 	
}"" 
}## �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ApplicationSecurityConfiguration.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Configuration5 B
{ 
public 

static 
class ,
 ApplicationSecurityConfiguration 8
{ 
public 
static 
IServiceCollection ((
ConfigureApplicationSecurity) E
(E F
this 
IServiceCollection #
services$ ,
,, -
IConfiguration 
configuration (
)( )
{ 	
services 
. 
AddTransient !
<! "
ICurrentUserService" 5
,5 6
CurrentUserService7 I
>I J
(J K
)K L
;L M#
JwtSecurityTokenHandler #
.# $#
DefaultMapInboundClaims$ ;
=< =
false> C
;C D
services 
. "
AddHttpContextAccessor +
(+ ,
), -
;- .
services 
. 
AddAuthentication &
(& '
JwtBearerDefaults' 8
.8 9 
AuthenticationScheme9 M
)M N
. 
AddJwtBearer 
( 
JwtBearerDefaults %
.% & 
AuthenticationScheme& :
,: ;
options 
=> 
{ 
options   
.    
	Authority    )
=  * +
configuration  , 9
.  9 :

GetSection  : D
(  D E
$str  E `
)  ` a
.  a b
Get  b e
<  e f
string  f l
>  l m
(  m n
)  n o
;  o p
options!! 
.!!  
Audience!!  (
=!!) *
configuration!!+ 8
.!!8 9

GetSection!!9 C
(!!C D
$str!!D ^
)!!^ _
.!!_ `
Get!!` c
<!!c d
string!!d j
>!!j k
(!!k l
)!!l m
;!!m n
options## 
.##  %
TokenValidationParameters##  9
.##9 :
RoleClaimType##: G
=##H I
$str##J P
;##P Q
options$$ 
.$$  
	SaveToken$$  )
=$$* +
true$$, 0
;$$0 1
}%% 
)%% 
;%% 
services'' 
.'' 
AddAuthorization'' %
(''% &"
ConfigureAuthorization''& <
)''< =
;''= >
return)) 
services)) 
;)) 
}** 	
[,, 	
IntentManaged,,	 
(,, 
Mode,, 
.,, 
Ignore,, "
),," #
],,# $
private-- 
static-- 
void-- "
ConfigureAuthorization-- 2
(--2 3 
AuthorizationOptions--3 G
options--H O
)--O P
{.. 	
}22 	
}33 
}44 �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ApiVersionSwaggerGenOptions.cs
[

 
assembly

 	
:

	 
 
DefaultIntentManaged

 
(

  
Mode

  $
.

$ %
Fully

% *
)

* +
]

+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str T
,T U
VersionV ]
=^ _
$str` e
)e f
]f g
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Api1 4
.4 5
Configuration5 B
{ 
public 

class '
ApiVersionSwaggerGenOptions ,
:- .
IConfigureOptions/ @
<@ A
SwaggerGenOptionsA R
>R S
{ 
private 
readonly *
IApiVersionDescriptionProvider 7
	_provider8 A
;A B
public '
ApiVersionSwaggerGenOptions *
(* +*
IApiVersionDescriptionProvider+ I
providerJ R
)R S
{ 	
	_provider 
= 
provider  
;  !
} 	
public 
void 
	Configure 
( 
SwaggerGenOptions /
options0 7
)7 8
{ 	
foreach 
( 
var 
description $
in% '
	_provider( 1
.1 2"
ApiVersionDescriptions2 H
.H I
OrderByDescendingI Z
(Z [
o[ \
=>] _
o` a
.a b

ApiVersionb l
)l m
)m n
{ 
options 
. 

SwaggerDoc "
(" #
description# .
.. /
	GroupName/ 8
,8 9#
CreateInfoForApiVersion: Q
(Q R
descriptionR ]
)] ^
)^ _
;_ `
} 
} 	
private   
static   
OpenApiInfo   "#
CreateInfoForApiVersion  # :
(  : ;!
ApiVersionDescription  ; P
description  Q \
)  \ ]
{!! 	
var"" 
info"" 
="" 
new"" 
OpenApiInfo"" &
(""& '
)""' (
{## 
Title$$ 
=$$ 
$str$$ D
,$$D E
Version%% 
=%% 
description%% %
.%%% &

ApiVersion%%& 0
.%%0 1
ToString%%1 9
(%%9 :
)%%: ;
}&& 
;&& 
if(( 
((( 
description(( 
.(( 
IsDeprecated(( (
)((( )
{)) 
info** 
.** 
Description**  
=**! "
$str**# J
;**J K
}++ 
return,, 
info,, 
;,, 
}-- 	
}.. 
}// �
�D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ApiVersioningConfiguration.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str S
,S T
VersionU \
=] ^
$str_ d
)d e
]e f
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Api		1 4
.		4 5
Configuration		5 B
{

 
public 

static 
class &
ApiVersioningConfiguration 2
{ 
public 
static 
IServiceCollection ("
ConfigureApiVersioning) ?
(? @
this@ D
IServiceCollectionE W
servicesX `
)` a
{ 	
services 
. 
AddApiVersioning %
(% &
options& -
=>. 0
{ 
options 
. /
#AssumeDefaultVersionWhenUnspecified ;
=< =
true> B
;B C
options 
. 
ReportApiVersions )
=* +
true, 0
;0 1
options 
. 
ApiVersionReader (
=) *
ApiVersionReader+ ;
.; <
Combine< C
(C D
newD G&
UrlSegmentApiVersionReaderH b
(b c
)c d
)d e
;e f
} 
) 
. 
AddMvc 
( 
) 
. 
AddApiExplorer 
( 
options #
=>$ &
{ 
options 
. 
GroupNameFormat '
=( )
$str* 2
;2 3
options 
. %
SubstituteApiVersionInUrl 1
=2 3
true4 8
;8 9
} 
) 
; 
return 
services 
; 
} 	
} 
} 