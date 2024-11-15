›&
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
}LL ¿
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Services\CurrentUserService.cs
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
}++ »
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
}00 £
çD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\TypeSchemaFilter.cs
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
} ˛,
çD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\ODataQueryFilter.cs
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
$str	**? ®
)
**® ©
)
**© ™
;
**™ ´
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
$str	++@ ∑
)
++∑ ∏
)
++∏ π
;
++π ∫
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
}@@ ÿ*
åD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\ExceptionFilter.cs
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
}BB Œ
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\BinaryContentFilter.cs
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
}11 Ω
öD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Filters\AuthorizeCheckOperationFilter.cs
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
}.. ¸∫
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\UsersController.cs
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
ÄÄ 
.
ÄÄ 
Id
ÄÄ 
=
ÄÄ 
id
ÄÄ 
;
ÄÄ  
}
ÅÅ 
if
ÉÉ 
(
ÉÉ 
id
ÉÉ 
!=
ÉÉ 
command
ÉÉ 
.
ÉÉ 
Id
ÉÉ  
)
ÉÉ  !
{
ÑÑ 
return
ÖÖ 

BadRequest
ÖÖ !
(
ÖÖ! "
)
ÖÖ" #
;
ÖÖ# $
}
ÜÜ 
await
àà 
	_mediator
àà 
.
àà 
Send
àà  
(
àà  !
command
àà! (
,
àà( )
cancellationToken
àà* ;
)
àà; <
;
àà< =
return
ââ 
	NoContent
ââ 
(
ââ 
)
ââ 
;
ââ 
}
ää 	
[
ëë 	
HttpPut
ëë	 
(
ëë 
$str
ëë  
)
ëë  !
]
ëë! "
[
íí 	"
ProducesResponseType
íí	 
(
íí 
StatusCodes
íí )
.
íí) * 
Status204NoContent
íí* <
)
íí< =
]
íí= >
[
ìì 	"
ProducesResponseType
ìì	 
(
ìì 
StatusCodes
ìì )
.
ìì) *!
Status400BadRequest
ìì* =
)
ìì= >
]
ìì> ?
[
îî 	"
ProducesResponseType
îî	 
(
îî 
StatusCodes
îî )
.
îî) *
Status404NotFound
îî* ;
)
îî; <
]
îî< =
[
ïï 	"
ProducesResponseType
ïï	 
(
ïï 
typeof
ïï $
(
ïï$ %
ProblemDetails
ïï% 3
)
ïï3 4
,
ïï4 5
StatusCodes
ïï6 A
.
ïïA B*
Status500InternalServerError
ïïB ^
)
ïï^ _
]
ïï_ `
public
ññ 
async
ññ 
Task
ññ 
<
ññ 
ActionResult
ññ &
>
ññ& '

UpdateUser
ññ( 2
(
ññ2 3
[
óó 
	FromRoute
óó 
]
óó 
Guid
óó 
id
óó 
,
óó  
[
òò 
FromBody
òò 
]
òò 
UpdateUserCommand
òò (
command
òò) 0
,
òò0 1
CancellationToken
ôô 
cancellationToken
ôô /
=
ôô0 1
default
ôô2 9
)
ôô9 :
{
öö 	
if
õõ 
(
õõ 
command
õõ 
.
õõ 
Id
õõ 
==
õõ 
Guid
õõ "
.
õõ" #
Empty
õõ# (
)
õõ( )
{
úú 
command
ùù 
.
ùù 
Id
ùù 
=
ùù 
id
ùù 
;
ùù  
}
ûû 
if
†† 
(
†† 
id
†† 
!=
†† 
command
†† 
.
†† 
Id
††  
)
††  !
{
°° 
return
¢¢ 

BadRequest
¢¢ !
(
¢¢! "
)
¢¢" #
;
¢¢# $
}
££ 
await
•• 
	_mediator
•• 
.
•• 
Send
••  
(
••  !
command
••! (
,
••( )
cancellationToken
••* ;
)
••; <
;
••< =
return
¶¶ 
	NoContent
¶¶ 
(
¶¶ 
)
¶¶ 
;
¶¶ 
}
ßß 	
[
ÆÆ 	
HttpGet
ÆÆ	 
(
ÆÆ 
$str
ÆÆ 6
)
ÆÆ6 7
]
ÆÆ7 8
[
ØØ 	"
ProducesResponseType
ØØ	 
(
ØØ 
typeof
ØØ $
(
ØØ$ %
UserAddressDto
ØØ% 3
)
ØØ3 4
,
ØØ4 5
StatusCodes
ØØ6 A
.
ØØA B
Status200OK
ØØB M
)
ØØM N
]
ØØN O
[
∞∞ 	"
ProducesResponseType
∞∞	 
(
∞∞ 
StatusCodes
∞∞ )
.
∞∞) *!
Status400BadRequest
∞∞* =
)
∞∞= >
]
∞∞> ?
[
±± 	"
ProducesResponseType
±±	 
(
±± 
StatusCodes
±± )
.
±±) *
Status404NotFound
±±* ;
)
±±; <
]
±±< =
[
≤≤ 	"
ProducesResponseType
≤≤	 
(
≤≤ 
typeof
≤≤ $
(
≤≤$ %
ProblemDetails
≤≤% 3
)
≤≤3 4
,
≤≤4 5
StatusCodes
≤≤6 A
.
≤≤A B*
Status500InternalServerError
≤≤B ^
)
≤≤^ _
]
≤≤_ `
public
≥≥ 
async
≥≥ 
Task
≥≥ 
<
≥≥ 
ActionResult
≥≥ &
<
≥≥& '
UserAddressDto
≥≥' 5
>
≥≥5 6
>
≥≥6 7 
GetUserAddressById
≥≥8 J
(
≥≥J K
[
¥¥ 
	FromRoute
¥¥ 
]
¥¥ 
Guid
¥¥ 
userId
¥¥ #
,
¥¥# $
[
µµ 
	FromRoute
µµ 
]
µµ 
Guid
µµ 
id
µµ 
,
µµ  
CancellationToken
∂∂ 
cancellationToken
∂∂ /
=
∂∂0 1
default
∂∂2 9
)
∂∂9 :
{
∑∑ 	
var
∏∏ 
result
∏∏ 
=
∏∏ 
await
∏∏ 
	_mediator
∏∏ (
.
∏∏( )
Send
∏∏) -
(
∏∏- .
new
∏∏. 1%
GetUserAddressByIdQuery
∏∏2 I
(
∏∏I J
userId
∏∏J P
:
∏∏P Q
userId
∏∏R X
,
∏∏X Y
id
∏∏Z \
:
∏∏\ ]
id
∏∏^ `
)
∏∏` a
,
∏∏a b
cancellationToken
∏∏c t
)
∏∏t u
;
∏∏u v
return
ππ 
result
ππ 
==
ππ 
null
ππ !
?
ππ" #
NotFound
ππ$ ,
(
ππ, -
)
ππ- .
:
ππ/ 0
Ok
ππ1 3
(
ππ3 4
result
ππ4 :
)
ππ: ;
;
ππ; <
}
∫∫ 	
[
¡¡ 	
HttpGet
¡¡	 
(
¡¡ 
$str
¡¡ 1
)
¡¡1 2
]
¡¡2 3
[
¬¬ 	"
ProducesResponseType
¬¬	 
(
¬¬ 
typeof
¬¬ $
(
¬¬$ %
List
¬¬% )
<
¬¬) *
UserAddressDto
¬¬* 8
>
¬¬8 9
)
¬¬9 :
,
¬¬: ;
StatusCodes
¬¬< G
.
¬¬G H
Status200OK
¬¬H S
)
¬¬S T
]
¬¬T U
[
√√ 	"
ProducesResponseType
√√	 
(
√√ 
StatusCodes
√√ )
.
√√) *!
Status400BadRequest
√√* =
)
√√= >
]
√√> ?
[
ƒƒ 	"
ProducesResponseType
ƒƒ	 
(
ƒƒ 
StatusCodes
ƒƒ )
.
ƒƒ) *
Status404NotFound
ƒƒ* ;
)
ƒƒ; <
]
ƒƒ< =
[
≈≈ 	"
ProducesResponseType
≈≈	 
(
≈≈ 
typeof
≈≈ $
(
≈≈$ %
ProblemDetails
≈≈% 3
)
≈≈3 4
,
≈≈4 5
StatusCodes
≈≈6 A
.
≈≈A B*
Status500InternalServerError
≈≈B ^
)
≈≈^ _
]
≈≈_ `
public
∆∆ 
async
∆∆ 
Task
∆∆ 
<
∆∆ 
ActionResult
∆∆ &
<
∆∆& '
List
∆∆' +
<
∆∆+ ,
UserAddressDto
∆∆, :
>
∆∆: ;
>
∆∆; <
>
∆∆< =
GetUserAddresses
∆∆> N
(
∆∆N O
[
«« 
	FromRoute
«« 
]
«« 
Guid
«« 
userId
«« #
,
««# $
CancellationToken
»» 
cancellationToken
»» /
=
»»0 1
default
»»2 9
)
»»9 :
{
…… 	
var
   
result
   
=
   
await
   
	_mediator
   (
.
  ( )
Send
  ) -
(
  - .
new
  . 1#
GetUserAddressesQuery
  2 G
(
  G H
userId
  H N
:
  N O
userId
  P V
)
  V W
,
  W X
cancellationToken
  Y j
)
  j k
;
  k l
return
ÀÀ 
result
ÀÀ 
==
ÀÀ 
null
ÀÀ !
?
ÀÀ" #
NotFound
ÀÀ$ ,
(
ÀÀ, -
)
ÀÀ- .
:
ÀÀ/ 0
Ok
ÀÀ1 3
(
ÀÀ3 4
result
ÀÀ4 :
)
ÀÀ: ;
;
ÀÀ; <
}
ÃÃ 	
[
”” 	
HttpGet
””	 
(
”” 
$str
””  
)
””  !
]
””! "
[
‘‘ 	"
ProducesResponseType
‘‘	 
(
‘‘ 
typeof
‘‘ $
(
‘‘$ %
UserDto
‘‘% ,
)
‘‘, -
,
‘‘- .
StatusCodes
‘‘/ :
.
‘‘: ;
Status200OK
‘‘; F
)
‘‘F G
]
‘‘G H
[
’’ 	"
ProducesResponseType
’’	 
(
’’ 
StatusCodes
’’ )
.
’’) *!
Status400BadRequest
’’* =
)
’’= >
]
’’> ?
[
÷÷ 	"
ProducesResponseType
÷÷	 
(
÷÷ 
StatusCodes
÷÷ )
.
÷÷) *
Status404NotFound
÷÷* ;
)
÷÷; <
]
÷÷< =
[
◊◊ 	"
ProducesResponseType
◊◊	 
(
◊◊ 
typeof
◊◊ $
(
◊◊$ %
ProblemDetails
◊◊% 3
)
◊◊3 4
,
◊◊4 5
StatusCodes
◊◊6 A
.
◊◊A B*
Status500InternalServerError
◊◊B ^
)
◊◊^ _
]
◊◊_ `
public
ÿÿ 
async
ÿÿ 
Task
ÿÿ 
<
ÿÿ 
ActionResult
ÿÿ &
<
ÿÿ& '
UserDto
ÿÿ' .
>
ÿÿ. /
>
ÿÿ/ 0
GetUserById
ÿÿ1 <
(
ÿÿ< =
[
ŸŸ 
	FromRoute
ŸŸ 
]
ŸŸ 
Guid
ŸŸ 
id
ŸŸ 
,
ŸŸ  
CancellationToken
⁄⁄ 
cancellationToken
⁄⁄ /
=
⁄⁄0 1
default
⁄⁄2 9
)
⁄⁄9 :
{
€€ 	
var
‹‹ 
result
‹‹ 
=
‹‹ 
await
‹‹ 
	_mediator
‹‹ (
.
‹‹( )
Send
‹‹) -
(
‹‹- .
new
‹‹. 1
GetUserByIdQuery
‹‹2 B
(
‹‹B C
id
‹‹C E
:
‹‹E F
id
‹‹G I
)
‹‹I J
,
‹‹J K
cancellationToken
‹‹L ]
)
‹‹] ^
;
‹‹^ _
return
›› 
result
›› 
==
›› 
null
›› !
?
››" #
NotFound
››$ ,
(
››, -
)
››- .
:
››/ 0
Ok
››1 3
(
››3 4
result
››4 :
)
››: ;
;
››; <
}
ﬁﬁ 	
[
‰‰ 	
HttpGet
‰‰	 
(
‰‰ 
$str
‰‰ 
)
‰‰ 
]
‰‰ 
[
ÂÂ 	"
ProducesResponseType
ÂÂ	 
(
ÂÂ 
typeof
ÂÂ $
(
ÂÂ$ %
PagedResult
ÂÂ% 0
<
ÂÂ0 1
UserDto
ÂÂ1 8
>
ÂÂ8 9
)
ÂÂ9 :
,
ÂÂ: ;
StatusCodes
ÂÂ< G
.
ÂÂG H
Status200OK
ÂÂH S
)
ÂÂS T
]
ÂÂT U
[
ÊÊ 	"
ProducesResponseType
ÊÊ	 
(
ÊÊ 
StatusCodes
ÊÊ )
.
ÊÊ) *!
Status400BadRequest
ÊÊ* =
)
ÊÊ= >
]
ÊÊ> ?
[
ÁÁ 	"
ProducesResponseType
ÁÁ	 
(
ÁÁ 
typeof
ÁÁ $
(
ÁÁ$ %
ProblemDetails
ÁÁ% 3
)
ÁÁ3 4
,
ÁÁ4 5
StatusCodes
ÁÁ6 A
.
ÁÁA B*
Status500InternalServerError
ÁÁB ^
)
ÁÁ^ _
]
ÁÁ_ `
public
ËË 
async
ËË 
Task
ËË 
<
ËË 
ActionResult
ËË &
<
ËË& '
PagedResult
ËË' 2
<
ËË2 3
UserDto
ËË3 :
>
ËË: ;
>
ËË; <
>
ËË< =
GetUsers
ËË> F
(
ËËF G
[
ÈÈ 
	FromQuery
ÈÈ 
]
ÈÈ 
string
ÈÈ 
?
ÈÈ 
name
ÈÈ  $
,
ÈÈ$ %
[
ÍÍ 
	FromQuery
ÍÍ 
]
ÍÍ 
string
ÍÍ 
?
ÍÍ 
surname
ÍÍ  '
,
ÍÍ' (
[
ÎÎ 
	FromQuery
ÎÎ 
]
ÎÎ 
int
ÎÎ 
pageNo
ÎÎ "
,
ÎÎ" #
[
ÏÏ 
	FromQuery
ÏÏ 
]
ÏÏ 
int
ÏÏ 
pageSize
ÏÏ $
,
ÏÏ$ %
CancellationToken
ÌÌ 
cancellationToken
ÌÌ /
=
ÌÌ0 1
default
ÌÌ2 9
)
ÌÌ9 :
{
ÓÓ 	
var
ÔÔ 
result
ÔÔ 
=
ÔÔ 
await
ÔÔ 
	_mediator
ÔÔ (
.
ÔÔ( )
Send
ÔÔ) -
(
ÔÔ- .
new
ÔÔ. 1
GetUsersQuery
ÔÔ2 ?
(
ÔÔ? @
name
ÔÔ@ D
:
ÔÔD E
name
ÔÔF J
,
ÔÔJ K
surname
ÔÔL S
:
ÔÔS T
surname
ÔÔU \
,
ÔÔ\ ]
pageNo
ÔÔ^ d
:
ÔÔd e
pageNo
ÔÔf l
,
ÔÔl m
pageSize
ÔÔn v
:
ÔÔv w
pageSizeÔÔx Ä
)ÔÔÄ Å
,ÔÔÅ Ç!
cancellationTokenÔÔÉ î
)ÔÔî ï
;ÔÔï ñ
return
 
Ok
 
(
 
result
 
)
 
;
 
}
ÒÒ 	
}
ÚÚ 
}ÛÛ ∏V
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\UploadDownloadController.cs
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
ContentType	CCv Å
.
CCÅ Ç

StartsWith
CCÇ å
(
CCå ç
$str
CCç ¢
)
CC¢ £
)
CC£ §
&&
CC• ß
Request
CC® Ø
.
CCØ ∞
Form
CC∞ ¥
.
CC¥ µ
Files
CCµ ∫
.
CC∫ ª
Any
CCª æ
(
CCæ ø
)
CCø ¿
)
CC¿ ¡
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
Enabled	RRz Å
)
RRÅ Ç
)
RRÇ É
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
}ss ¢

õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\ResponseTypes\JsonResponse.cs
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
} Ÿå
ìD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\ProductsController.cs
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
Enabled	::z Å
)
::Å Ç
)
::Ç É
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
Enabled	uuz Å
)
uuÅ Ç
)
uuÇ É
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
ÑÑ 	

HttpDelete
ÑÑ	 
(
ÑÑ 
$str
ÑÑ 
)
ÑÑ 
]
ÑÑ 
[
ÖÖ 	"
ProducesResponseType
ÖÖ	 
(
ÖÖ 
StatusCodes
ÖÖ )
.
ÖÖ) *
Status200OK
ÖÖ* 5
)
ÖÖ5 6
]
ÖÖ6 7
[
ÜÜ 	"
ProducesResponseType
ÜÜ	 
(
ÜÜ 
StatusCodes
ÜÜ )
.
ÜÜ) *!
Status400BadRequest
ÜÜ* =
)
ÜÜ= >
]
ÜÜ> ?
[
áá 	"
ProducesResponseType
áá	 
(
áá 
StatusCodes
áá )
.
áá) *
Status404NotFound
áá* ;
)
áá; <
]
áá< =
[
àà 	"
ProducesResponseType
àà	 
(
àà 
typeof
àà $
(
àà$ %
ProblemDetails
àà% 3
)
àà3 4
,
àà4 5
StatusCodes
àà6 A
.
ààA B*
Status500InternalServerError
ààB ^
)
àà^ _
]
àà_ `
public
ââ 
async
ââ 
Task
ââ 
<
ââ 
ActionResult
ââ &
>
ââ& '
DeleteProduct
ââ( 5
(
ââ5 6
[
ââ6 7
	FromRoute
ââ7 @
]
ââ@ A
Guid
ââB F
id
ââG I
,
ââI J
CancellationToken
ââK \
cancellationToken
ââ] n
=
ââo p
default
ââq x
)
ââx y
{
ää 	
using
ãã 
(
ãã 
var
ãã 
transaction
ãã "
=
ãã# $
new
ãã% (
TransactionScope
ãã) 9
(
ãã9 :$
TransactionScopeOption
ãã: P
.
ããP Q
Required
ããQ Y
,
ããY Z
new
åå  
TransactionOptions
åå &
{
åå' (
IsolationLevel
åå) 7
=
åå8 9
IsolationLevel
åå: H
.
ååH I
ReadCommitted
ååI V
}
ååW X
,
ååX Y-
TransactionScopeAsyncFlowOption
ååZ y
.
ååy z
Enabledååz Å
)ååÅ Ç
)ååÇ É
{
çç 
await
éé 
_appService
éé !
.
éé! "
DeleteProduct
éé" /
(
éé/ 0
id
éé0 2
,
éé2 3
cancellationToken
éé4 E
)
ééE F
;
ééF G
await
èè 
_unitOfWork
èè !
.
èè! "
SaveChangesAsync
èè" 2
(
èè2 3
cancellationToken
èè3 D
)
èèD E
;
èèE F
transaction
êê 
.
êê 
Complete
êê $
(
êê$ %
)
êê% &
;
êê& '
}
ëë 
await
íí 
	_eventBus
íí 
.
íí 
FlushAllAsync
íí )
(
íí) *
cancellationToken
íí* ;
)
íí; <
;
íí< =
return
ìì 
Ok
ìì 
(
ìì 
)
ìì 
;
ìì 
}
îî 	
[
öö 	
HttpGet
öö	 
(
öö 
$str
öö 
)
öö 
]
öö 
[
õõ 	"
ProducesResponseType
õõ	 
(
õõ 
typeof
õõ $
(
õõ$ %
PagedResult
õõ% 0
<
õõ0 1

ProductDto
õõ1 ;
>
õõ; <
)
õõ< =
,
õõ= >
StatusCodes
õõ? J
.
õõJ K
Status200OK
õõK V
)
õõV W
]
õõW X
[
úú 	"
ProducesResponseType
úú	 
(
úú 
StatusCodes
úú )
.
úú) *!
Status400BadRequest
úú* =
)
úú= >
]
úú> ?
[
ùù 	"
ProducesResponseType
ùù	 
(
ùù 
typeof
ùù $
(
ùù$ %
ProblemDetails
ùù% 3
)
ùù3 4
,
ùù4 5
StatusCodes
ùù6 A
.
ùùA B*
Status500InternalServerError
ùùB ^
)
ùù^ _
]
ùù_ `
public
ûû 
async
ûû 
Task
ûû 
<
ûû 
ActionResult
ûû &
<
ûû& '
PagedResult
ûû' 2
<
ûû2 3

ProductDto
ûû3 =
>
ûû= >
>
ûû> ?
>
ûû? @
FindProductsPaged
ûûA R
(
ûûR S
[
üü 
	FromQuery
üü 
]
üü 
int
üü 
pageNo
üü "
,
üü" #
[
†† 
	FromQuery
†† 
]
†† 
int
†† 
pageSize
†† $
,
††$ %
[
°° 
	FromQuery
°° 
]
°° 
string
°° 
orderBy
°° &
,
°°& '
CancellationToken
¢¢ 
cancellationToken
¢¢ /
=
¢¢0 1
default
¢¢2 9
)
¢¢9 :
{
££ 	
var
§§ 
result
§§ 
=
§§ 
default
§§  
(
§§  !
PagedResult
§§! ,
<
§§, -

ProductDto
§§- 7
>
§§7 8
)
§§8 9
;
§§9 :
result
•• 
=
•• 
await
•• 
_appService
•• &
.
••& '
FindProductsPaged
••' 8
(
••8 9
pageNo
••9 ?
,
••? @
pageSize
••A I
,
••I J
orderBy
••K R
,
••R S
cancellationToken
••T e
)
••e f
;
••f g
return
¶¶ 
Ok
¶¶ 
(
¶¶ 
result
¶¶ 
)
¶¶ 
;
¶¶ 
}
ßß 	
}
®® 
}©© ≠Z
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\ParentWithAnemicChildrenController.cs
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
cancellationToken	||v á
=
||à â
default
||ä ë
)
||ë í
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
ÄÄ 	
}
ÅÅ 
}ÇÇ µ
ìD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\PagingTSController.cs
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
Enabled	;;z Å
)
;;Å Ç
)
;;Ç É
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
Enabled	}}z Å
)
}}Å Ç
)
}}Ç É
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
ÄÄ 
_unitOfWork
ÄÄ !
.
ÄÄ! "
SaveChangesAsync
ÄÄ" 2
(
ÄÄ2 3
cancellationToken
ÄÄ3 D
)
ÄÄD E
;
ÄÄE F
transaction
ÅÅ 
.
ÅÅ 
Complete
ÅÅ $
(
ÅÅ$ %
)
ÅÅ% &
;
ÅÅ& '
}
ÇÇ 
await
ÉÉ 
	_eventBus
ÉÉ 
.
ÉÉ 
FlushAllAsync
ÉÉ )
(
ÉÉ) *
cancellationToken
ÉÉ* ;
)
ÉÉ; <
;
ÉÉ< =
return
ÑÑ 
	NoContent
ÑÑ 
(
ÑÑ 
)
ÑÑ 
;
ÑÑ 
}
ÖÖ 	
[
åå 	

HttpDelete
åå	 
(
åå 
$str
åå 
)
åå 
]
åå 
[
çç 	"
ProducesResponseType
çç	 
(
çç 
StatusCodes
çç )
.
çç) *
Status200OK
çç* 5
)
çç5 6
]
çç6 7
[
éé 	"
ProducesResponseType
éé	 
(
éé 
StatusCodes
éé )
.
éé) *!
Status400BadRequest
éé* =
)
éé= >
]
éé> ?
[
èè 	"
ProducesResponseType
èè	 
(
èè 
StatusCodes
èè )
.
èè) *
Status404NotFound
èè* ;
)
èè; <
]
èè< =
[
êê 	"
ProducesResponseType
êê	 
(
êê 
typeof
êê $
(
êê$ %
ProblemDetails
êê% 3
)
êê3 4
,
êê4 5
StatusCodes
êê6 A
.
êêA B*
Status500InternalServerError
êêB ^
)
êê^ _
]
êê_ `
public
ëë 
async
ëë 
Task
ëë 
<
ëë 
ActionResult
ëë &
>
ëë& '
DeletePagingTS
ëë( 6
(
ëë6 7
[
ëë7 8
	FromRoute
ëë8 A
]
ëëA B
Guid
ëëC G
id
ëëH J
,
ëëJ K
CancellationToken
ëëL ]
cancellationToken
ëë^ o
=
ëëp q
default
ëër y
)
ëëy z
{
íí 	
using
ìì 
(
ìì 
var
ìì 
transaction
ìì "
=
ìì# $
new
ìì% (
TransactionScope
ìì) 9
(
ìì9 :$
TransactionScopeOption
ìì: P
.
ììP Q
Required
ììQ Y
,
ììY Z
new
îî  
TransactionOptions
îî &
{
îî' (
IsolationLevel
îî) 7
=
îî8 9
IsolationLevel
îî: H
.
îîH I
ReadCommitted
îîI V
}
îîW X
,
îîX Y-
TransactionScopeAsyncFlowOption
îîZ y
.
îîy z
Enabledîîz Å
)îîÅ Ç
)îîÇ É
{
ïï 
await
ññ 
_appService
ññ !
.
ññ! "
DeletePagingTS
ññ" 0
(
ññ0 1
id
ññ1 3
,
ññ3 4
cancellationToken
ññ5 F
)
ññF G
;
ññG H
await
óó 
_unitOfWork
óó !
.
óó! "
SaveChangesAsync
óó" 2
(
óó2 3
cancellationToken
óó3 D
)
óóD E
;
óóE F
transaction
òò 
.
òò 
Complete
òò $
(
òò$ %
)
òò% &
;
òò& '
}
ôô 
await
öö 
	_eventBus
öö 
.
öö 
FlushAllAsync
öö )
(
öö) *
cancellationToken
öö* ;
)
öö; <
;
öö< =
return
õõ 
Ok
õõ 
(
õõ 
)
õõ 
;
õõ 
}
úú 	
}
ùù 
}ûû °∏
ëD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\OrdersController.cs
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
ÄÄ 
.
ÄÄ 
Id
ÄÄ 
=
ÄÄ 
id
ÄÄ 
;
ÄÄ  
}
ÅÅ 
if
ÉÉ 
(
ÉÉ 
id
ÉÉ 
!=
ÉÉ 
command
ÉÉ 
.
ÉÉ 
Id
ÉÉ  
)
ÉÉ  !
{
ÑÑ 
return
ÖÖ 

BadRequest
ÖÖ !
(
ÖÖ! "
)
ÖÖ" #
;
ÖÖ# $
}
ÜÜ 
await
àà 
	_mediator
àà 
.
àà 
Send
àà  
(
àà  !
command
àà! (
,
àà( )
cancellationToken
àà* ;
)
àà; <
;
àà< =
return
ââ 
	NoContent
ââ 
(
ââ 
)
ââ 
;
ââ 
}
ää 	
[
ëë 	
HttpPut
ëë	 
(
ëë 
$str
ëë ,
)
ëë, -
]
ëë- .
[
íí 	"
ProducesResponseType
íí	 
(
íí 
StatusCodes
íí )
.
íí) * 
Status204NoContent
íí* <
)
íí< =
]
íí= >
[
ìì 	"
ProducesResponseType
ìì	 
(
ìì 
StatusCodes
ìì )
.
ìì) *!
Status400BadRequest
ìì* =
)
ìì= >
]
ìì> ?
[
îî 	"
ProducesResponseType
îî	 
(
îî 
StatusCodes
îî )
.
îî) *
Status404NotFound
îî* ;
)
îî; <
]
îî< =
[
ïï 	"
ProducesResponseType
ïï	 
(
ïï 
typeof
ïï $
(
ïï$ %
ProblemDetails
ïï% 3
)
ïï3 4
,
ïï4 5
StatusCodes
ïï6 A
.
ïïA B*
Status500InternalServerError
ïïB ^
)
ïï^ _
]
ïï_ `
public
ññ 
async
ññ 
Task
ññ 
<
ññ 
ActionResult
ññ &
>
ññ& '"
UpdateOrderOrderItem
ññ( <
(
ññ< =
[
óó 
	FromRoute
óó 
]
óó 
Guid
óó 
id
óó 
,
óó  
[
òò 
FromBody
òò 
]
òò )
UpdateOrderOrderItemCommand
òò 2
command
òò3 :
,
òò: ;
CancellationToken
ôô 
cancellationToken
ôô /
=
ôô0 1
default
ôô2 9
)
ôô9 :
{
öö 	
if
õõ 
(
õõ 
command
õõ 
.
õõ 
Id
õõ 
==
õõ 
Guid
õõ "
.
õõ" #
Empty
õõ# (
)
õõ( )
{
úú 
command
ùù 
.
ùù 
Id
ùù 
=
ùù 
id
ùù 
;
ùù  
}
ûû 
if
†† 
(
†† 
id
†† 
!=
†† 
command
†† 
.
†† 
Id
††  
)
††  !
{
°° 
return
¢¢ 

BadRequest
¢¢ !
(
¢¢! "
)
¢¢" #
;
¢¢# $
}
££ 
await
•• 
	_mediator
•• 
.
•• 
Send
••  
(
••  !
command
••! (
,
••( )
cancellationToken
••* ;
)
••; <
;
••< =
return
¶¶ 
	NoContent
¶¶ 
(
¶¶ 
)
¶¶ 
;
¶¶ 
}
ßß 	
[
ÆÆ 	
HttpGet
ÆÆ	 
(
ÆÆ 
$str
ÆÆ !
)
ÆÆ! "
]
ÆÆ" #
[
ØØ 	"
ProducesResponseType
ØØ	 
(
ØØ 
typeof
ØØ $
(
ØØ$ %
OrderDto
ØØ% -
)
ØØ- .
,
ØØ. /
StatusCodes
ØØ0 ;
.
ØØ; <
Status200OK
ØØ< G
)
ØØG H
]
ØØH I
[
∞∞ 	"
ProducesResponseType
∞∞	 
(
∞∞ 
StatusCodes
∞∞ )
.
∞∞) *!
Status400BadRequest
∞∞* =
)
∞∞= >
]
∞∞> ?
[
±± 	"
ProducesResponseType
±±	 
(
±± 
StatusCodes
±± )
.
±±) *
Status404NotFound
±±* ;
)
±±; <
]
±±< =
[
≤≤ 	"
ProducesResponseType
≤≤	 
(
≤≤ 
typeof
≤≤ $
(
≤≤$ %
ProblemDetails
≤≤% 3
)
≤≤3 4
,
≤≤4 5
StatusCodes
≤≤6 A
.
≤≤A B*
Status500InternalServerError
≤≤B ^
)
≤≤^ _
]
≤≤_ `
public
≥≥ 
async
≥≥ 
Task
≥≥ 
<
≥≥ 
ActionResult
≥≥ &
<
≥≥& '
OrderDto
≥≥' /
>
≥≥/ 0
>
≥≥0 1
GetOrderById
≥≥2 >
(
≥≥> ?
[
¥¥ 
	FromRoute
¥¥ 
]
¥¥ 
Guid
¥¥ 
id
¥¥ 
,
¥¥  
CancellationToken
µµ 
cancellationToken
µµ /
=
µµ0 1
default
µµ2 9
)
µµ9 :
{
∂∂ 	
var
∑∑ 
result
∑∑ 
=
∑∑ 
await
∑∑ 
	_mediator
∑∑ (
.
∑∑( )
Send
∑∑) -
(
∑∑- .
new
∑∑. 1
GetOrderByIdQuery
∑∑2 C
(
∑∑C D
id
∑∑D F
:
∑∑F G
id
∑∑H J
)
∑∑J K
,
∑∑K L
cancellationToken
∑∑M ^
)
∑∑^ _
;
∑∑_ `
return
∏∏ 
result
∏∏ 
==
∏∏ 
null
∏∏ !
?
∏∏" #
NotFound
∏∏$ ,
(
∏∏, -
)
∏∏- .
:
∏∏/ 0
Ok
∏∏1 3
(
∏∏3 4
result
∏∏4 :
)
∏∏: ;
;
∏∏; <
}
ππ 	
[
¿¿ 	
HttpGet
¿¿	 
(
¿¿ 
$str
¿¿ 6
)
¿¿6 7
]
¿¿7 8
[
¡¡ 	"
ProducesResponseType
¡¡	 
(
¡¡ 
typeof
¡¡ $
(
¡¡$ %
OrderOrderItemDto
¡¡% 6
)
¡¡6 7
,
¡¡7 8
StatusCodes
¡¡9 D
.
¡¡D E
Status200OK
¡¡E P
)
¡¡P Q
]
¡¡Q R
[
¬¬ 	"
ProducesResponseType
¬¬	 
(
¬¬ 
StatusCodes
¬¬ )
.
¬¬) *!
Status400BadRequest
¬¬* =
)
¬¬= >
]
¬¬> ?
[
√√ 	"
ProducesResponseType
√√	 
(
√√ 
StatusCodes
√√ )
.
√√) *
Status404NotFound
√√* ;
)
√√; <
]
√√< =
[
ƒƒ 	"
ProducesResponseType
ƒƒ	 
(
ƒƒ 
typeof
ƒƒ $
(
ƒƒ$ %
ProblemDetails
ƒƒ% 3
)
ƒƒ3 4
,
ƒƒ4 5
StatusCodes
ƒƒ6 A
.
ƒƒA B*
Status500InternalServerError
ƒƒB ^
)
ƒƒ^ _
]
ƒƒ_ `
public
≈≈ 
async
≈≈ 
Task
≈≈ 
<
≈≈ 
ActionResult
≈≈ &
<
≈≈& '
OrderOrderItemDto
≈≈' 8
>
≈≈8 9
>
≈≈9 :#
GetOrderOrderItemById
≈≈; P
(
≈≈P Q
[
∆∆ 
	FromRoute
∆∆ 
]
∆∆ 
Guid
∆∆ 
orderId
∆∆ $
,
∆∆$ %
[
«« 
	FromRoute
«« 
]
«« 
Guid
«« 
id
«« 
,
««  
CancellationToken
»» 
cancellationToken
»» /
=
»»0 1
default
»»2 9
)
»»9 :
{
…… 	
var
   
result
   
=
   
await
   
	_mediator
   (
.
  ( )
Send
  ) -
(
  - .
new
  . 1(
GetOrderOrderItemByIdQuery
  2 L
(
  L M
orderId
  M T
:
  T U
orderId
  V ]
,
  ] ^
id
  _ a
:
  a b
id
  c e
)
  e f
,
  f g
cancellationToken
  h y
)
  y z
;
  z {
return
ÀÀ 
result
ÀÀ 
==
ÀÀ 
null
ÀÀ !
?
ÀÀ" #
NotFound
ÀÀ$ ,
(
ÀÀ, -
)
ÀÀ- .
:
ÀÀ/ 0
Ok
ÀÀ1 3
(
ÀÀ3 4
result
ÀÀ4 :
)
ÀÀ: ;
;
ÀÀ; <
}
ÃÃ 	
[
”” 	
HttpGet
””	 
(
”” 
$str
”” 1
)
””1 2
]
””2 3
[
‘‘ 	"
ProducesResponseType
‘‘	 
(
‘‘ 
typeof
‘‘ $
(
‘‘$ %
List
‘‘% )
<
‘‘) *
OrderOrderItemDto
‘‘* ;
>
‘‘; <
)
‘‘< =
,
‘‘= >
StatusCodes
‘‘? J
.
‘‘J K
Status200OK
‘‘K V
)
‘‘V W
]
‘‘W X
[
’’ 	"
ProducesResponseType
’’	 
(
’’ 
StatusCodes
’’ )
.
’’) *!
Status400BadRequest
’’* =
)
’’= >
]
’’> ?
[
÷÷ 	"
ProducesResponseType
÷÷	 
(
÷÷ 
StatusCodes
÷÷ )
.
÷÷) *
Status404NotFound
÷÷* ;
)
÷÷; <
]
÷÷< =
[
◊◊ 	"
ProducesResponseType
◊◊	 
(
◊◊ 
typeof
◊◊ $
(
◊◊$ %
ProblemDetails
◊◊% 3
)
◊◊3 4
,
◊◊4 5
StatusCodes
◊◊6 A
.
◊◊A B*
Status500InternalServerError
◊◊B ^
)
◊◊^ _
]
◊◊_ `
public
ÿÿ 
async
ÿÿ 
Task
ÿÿ 
<
ÿÿ 
ActionResult
ÿÿ &
<
ÿÿ& '
List
ÿÿ' +
<
ÿÿ+ ,
OrderOrderItemDto
ÿÿ, =
>
ÿÿ= >
>
ÿÿ> ?
>
ÿÿ? @ 
GetOrderOrderItems
ÿÿA S
(
ÿÿS T
[
ŸŸ 
	FromRoute
ŸŸ 
]
ŸŸ 
Guid
ŸŸ 
orderId
ŸŸ $
,
ŸŸ$ %
CancellationToken
⁄⁄ 
cancellationToken
⁄⁄ /
=
⁄⁄0 1
default
⁄⁄2 9
)
⁄⁄9 :
{
€€ 	
var
‹‹ 
result
‹‹ 
=
‹‹ 
await
‹‹ 
	_mediator
‹‹ (
.
‹‹( )
Send
‹‹) -
(
‹‹- .
new
‹‹. 1%
GetOrderOrderItemsQuery
‹‹2 I
(
‹‹I J
orderId
‹‹J Q
:
‹‹Q R
orderId
‹‹S Z
)
‹‹Z [
,
‹‹[ \
cancellationToken
‹‹] n
)
‹‹n o
;
‹‹o p
return
›› 
result
›› 
==
›› 
null
›› !
?
››" #
NotFound
››$ ,
(
››, -
)
››- .
:
››/ 0
Ok
››1 3
(
››3 4
result
››4 :
)
››: ;
;
››; <
}
ﬁﬁ 	
[
‰‰ 	
HttpGet
‰‰	 
(
‰‰ 
$str
‰‰ 
)
‰‰ 
]
‰‰ 
[
ÂÂ 	"
ProducesResponseType
ÂÂ	 
(
ÂÂ 
typeof
ÂÂ $
(
ÂÂ$ %
PagedResult
ÂÂ% 0
<
ÂÂ0 1
OrderDto
ÂÂ1 9
>
ÂÂ9 :
)
ÂÂ: ;
,
ÂÂ; <
StatusCodes
ÂÂ= H
.
ÂÂH I
Status200OK
ÂÂI T
)
ÂÂT U
]
ÂÂU V
[
ÊÊ 	"
ProducesResponseType
ÊÊ	 
(
ÊÊ 
StatusCodes
ÊÊ )
.
ÊÊ) *!
Status400BadRequest
ÊÊ* =
)
ÊÊ= >
]
ÊÊ> ?
[
ÁÁ 	"
ProducesResponseType
ÁÁ	 
(
ÁÁ 
typeof
ÁÁ $
(
ÁÁ$ %
ProblemDetails
ÁÁ% 3
)
ÁÁ3 4
,
ÁÁ4 5
StatusCodes
ÁÁ6 A
.
ÁÁA B*
Status500InternalServerError
ÁÁB ^
)
ÁÁ^ _
]
ÁÁ_ `
public
ËË 
async
ËË 
Task
ËË 
<
ËË 
ActionResult
ËË &
<
ËË& '
PagedResult
ËË' 2
<
ËË2 3
OrderDto
ËË3 ;
>
ËË; <
>
ËË< =
>
ËË= > 
GetOrdersPaginated
ËË? Q
(
ËËQ R
[
ÈÈ 
	FromQuery
ÈÈ 
]
ÈÈ 
int
ÈÈ 
pageNo
ÈÈ "
,
ÈÈ" #
[
ÍÍ 
	FromQuery
ÍÍ 
]
ÍÍ 
int
ÍÍ 
pageSize
ÍÍ $
,
ÍÍ$ %
CancellationToken
ÎÎ 
cancellationToken
ÎÎ /
=
ÎÎ0 1
default
ÎÎ2 9
)
ÎÎ9 :
{
ÏÏ 	
var
ÌÌ 
result
ÌÌ 
=
ÌÌ 
await
ÌÌ 
	_mediator
ÌÌ (
.
ÌÌ( )
Send
ÌÌ) -
(
ÌÌ- .
new
ÌÌ. 1%
GetOrdersPaginatedQuery
ÌÌ2 I
(
ÌÌI J
pageNo
ÌÌJ P
:
ÌÌP Q
pageNo
ÌÌR X
,
ÌÌX Y
pageSize
ÌÌZ b
:
ÌÌb c
pageSize
ÌÌd l
)
ÌÌl m
,
ÌÌm n 
cancellationTokenÌÌo Ä
)ÌÌÄ Å
;ÌÌÅ Ç
return
ÓÓ 
Ok
ÓÓ 
(
ÓÓ 
result
ÓÓ 
)
ÓÓ 
;
ÓÓ 
}
ÔÔ 	
}
 
}ÒÒ —,
îD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\OptionalsController.cs
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
}BB ‚≤
ñD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\FileUploadsController.cs
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
ContentType	LLv Å
.
LLÅ Ç

StartsWith
LLÇ å
(
LLå ç
$str
LLç ¢
)
LL¢ £
)
LL£ §
&&
LL• ß
Request
LL® Ø
.
LLØ ∞
Form
LL∞ ¥
.
LL¥ µ
Files
LLµ ∫
.
LL∫ ª
Any
LLª æ
(
LLæ ø
)
LLø ¿
)
LL¿ ¡
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
contentLength	XXu Ç
:
XXÇ É
contentLength
XXÑ ë
)
XXë í
;
XXí ì
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
ContentType	kkv Å
.
kkÅ Ç

StartsWith
kkÇ å
(
kkå ç
$str
kkç ¢
)
kk¢ £
)
kk£ §
&&
kk• ß
Request
kk® Ø
.
kkØ ∞
Form
kk∞ ¥
.
kk¥ µ
Files
kkµ ∫
.
kk∫ ª
Any
kkª æ
(
kkæ ø
)
kkø ¿
)
kk¿ ¡
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
ÄÄ 	
BinaryContent
ÄÄ	 
]
ÄÄ 
[
ÅÅ 	
HttpPost
ÅÅ	 
(
ÅÅ 
$str
ÅÅ 0
)
ÅÅ0 1
]
ÅÅ1 2
[
ÇÇ 	
Produces
ÇÇ	 
(
ÇÇ 
MediaTypeNames
ÇÇ  
.
ÇÇ  !
Application
ÇÇ! ,
.
ÇÇ, -
Json
ÇÇ- 1
)
ÇÇ1 2
]
ÇÇ2 3
[
ÉÉ 	"
ProducesResponseType
ÉÉ	 
(
ÉÉ 
typeof
ÉÉ $
(
ÉÉ$ %
JsonResponse
ÉÉ% 1
<
ÉÉ1 2
Guid
ÉÉ2 6
>
ÉÉ6 7
)
ÉÉ7 8
,
ÉÉ8 9
StatusCodes
ÉÉ: E
.
ÉÉE F
Status201Created
ÉÉF V
)
ÉÉV W
]
ÉÉW X
[
ÑÑ 	"
ProducesResponseType
ÑÑ	 
(
ÑÑ 
StatusCodes
ÑÑ )
.
ÑÑ) *!
Status400BadRequest
ÑÑ* =
)
ÑÑ= >
]
ÑÑ> ?
[
ÖÖ 	"
ProducesResponseType
ÖÖ	 
(
ÖÖ 
typeof
ÖÖ $
(
ÖÖ$ %
ProblemDetails
ÖÖ% 3
)
ÖÖ3 4
,
ÖÖ4 5
StatusCodes
ÖÖ6 A
.
ÖÖA B*
Status500InternalServerError
ÖÖB ^
)
ÖÖ^ _
]
ÖÖ_ `
public
ÜÜ 
async
ÜÜ 
Task
ÜÜ 
<
ÜÜ 
ActionResult
ÜÜ &
<
ÜÜ& '
JsonResponse
ÜÜ' 3
<
ÜÜ3 4
Guid
ÜÜ4 8
>
ÜÜ8 9
>
ÜÜ9 :
>
ÜÜ: ;

UploadFile
ÜÜ< F
(
ÜÜF G
[
áá 

FromHeader
áá 
(
áá 
Name
áá 
=
áá 
$str
áá -
)
áá- .
]
áá. /
string
áá0 6
?
áá6 7
contentType
áá8 C
,
ááC D
[
àà 

FromHeader
àà 
(
àà 
Name
àà 
=
àà 
$str
àà /
)
àà/ 0
]
àà0 1
long
àà2 6
?
àà6 7
contentLength
àà8 E
,
ààE F
CancellationToken
ââ 
cancellationToken
ââ /
=
ââ0 1
default
ââ2 9
)
ââ9 :
{
ää 	
Stream
ãã 
stream
ãã 
;
ãã 
string
åå 
?
åå 
filename
åå 
=
åå 
null
åå #
;
åå# $
if
çç 
(
çç 
Request
çç 
.
çç 
Headers
çç 
.
çç  
TryGetValue
çç  +
(
çç+ ,
$str
çç, A
,
ççA B
out
ççC F
var
ççG J
headerValues
ççK W
)
ççW X
)
ççX Y
{
éé 
string
èè 
?
èè 
header
èè 
=
èè  
headerValues
èè! -
;
èè- .
if
êê 
(
êê 
header
êê 
!=
êê 
null
êê "
)
êê" #
{
ëë 
var
íí  
contentDisposition
íí *
=
íí+ ,+
ContentDispositionHeaderValue
íí- J
.
ííJ K
Parse
ííK P
(
ííP Q
header
ííQ W
)
ííW X
;
ííX Y
filename
ìì 
=
ìì  
contentDisposition
ìì 1
?
ìì1 2
.
ìì2 3
FileName
ìì3 ;
;
ìì; <
}
îî 
}
ïï 
if
óó 
(
óó 
Request
óó 
.
óó 
ContentType
óó #
!=
óó$ &
null
óó' +
&&
óó, .
(
óó/ 0
Request
óó0 7
.
óó7 8
ContentType
óó8 C
==
óóD F
$str
óóG j
||
óók m
Request
óón u
.
óóu v
ContentTypeóóv Å
.óóÅ Ç

StartsWithóóÇ å
(óóå ç
$stróóç ¢
)óó¢ £
)óó£ §
&&óó• ß
Requestóó® Ø
.óóØ ∞
Formóó∞ ¥
.óó¥ µ
Filesóóµ ∫
.óó∫ ª
Anyóóª æ
(óóæ ø
)óóø ¿
)óó¿ ¡
{
òò 
var
ôô 
file
ôô 
=
ôô 
Request
ôô "
.
ôô" #
Form
ôô# '
.
ôô' (
Files
ôô( -
[
ôô- .
$num
ôô. /
]
ôô/ 0
;
ôô0 1
if
öö 
(
öö 
file
öö 
==
öö 
null
öö  
||
öö! #
file
öö$ (
.
öö( )
Length
öö) /
==
öö0 2
$num
öö3 4
)
öö4 5
throw
õõ 
new
õõ 
ArgumentException
õõ /
(
õõ/ 0
$str
õõ0 ?
)
õõ? @
;
õõ@ A
stream
úú 
=
úú 
file
úú 
.
úú 
OpenReadStream
úú ,
(
úú, -
)
úú- .
;
úú. /
filename
ùù 
??=
ùù 
file
ùù !
.
ùù! "
Name
ùù" &
;
ùù& '
}
ûû 
else
üü 
{
†† 
stream
°° 
=
°° 
Request
°°  
.
°°  !
Body
°°! %
;
°°% &
}
¢¢ 
var
££ 
command
££ 
=
££ 
new
££ 
UploadFileCommand
££ /
(
££/ 0
content
££0 7
:
££7 8
stream
££9 ?
,
££? @
filename
££A I
:
££I J
filename
££K S
,
££S T
contentType
££U `
:
££` a
contentType
££b m
,
££m n
contentLength
££o |
:
££| }
contentLength££~ ã
)££ã å
;££å ç
var
•• 
result
•• 
=
•• 
await
•• 
	_mediator
•• (
.
••( )
Send
••) -
(
••- .
command
••. 5
,
••5 6
cancellationToken
••7 H
)
••H I
;
••I J
return
¶¶ 
Created
¶¶ 
(
¶¶ 
string
¶¶ !
.
¶¶! "
Empty
¶¶" '
,
¶¶' (
new
¶¶) ,
JsonResponse
¶¶- 9
<
¶¶9 :
Guid
¶¶: >
>
¶¶> ?
(
¶¶? @
result
¶¶@ F
)
¶¶F G
)
¶¶G H
;
¶¶H I
}
ßß 	
[
ÆÆ 	
HttpGet
ÆÆ	 
(
ÆÆ 
$str
ÆÆ '
)
ÆÆ' (
]
ÆÆ( )
[
ØØ 	"
ProducesResponseType
ØØ	 
(
ØØ 
typeof
ØØ $
(
ØØ$ %
byte
ØØ% )
[
ØØ) *
]
ØØ* +
)
ØØ+ ,
,
ØØ, -
StatusCodes
ØØ. 9
.
ØØ9 :
Status200OK
ØØ: E
)
ØØE F
]
ØØF G
[
∞∞ 	"
ProducesResponseType
∞∞	 
(
∞∞ 
StatusCodes
∞∞ )
.
∞∞) *!
Status400BadRequest
∞∞* =
)
∞∞= >
]
∞∞> ?
[
±± 	"
ProducesResponseType
±±	 
(
±± 
StatusCodes
±± )
.
±±) *
Status404NotFound
±±* ;
)
±±; <
]
±±< =
[
≤≤ 	"
ProducesResponseType
≤≤	 
(
≤≤ 
typeof
≤≤ $
(
≤≤$ %
ProblemDetails
≤≤% 3
)
≤≤3 4
,
≤≤4 5
StatusCodes
≤≤6 A
.
≤≤A B*
Status500InternalServerError
≤≤B ^
)
≤≤^ _
]
≤≤_ `
public
≥≥ 
async
≥≥ 
Task
≥≥ 
<
≥≥ 
ActionResult
≥≥ &
<
≥≥& '
byte
≥≥' +
[
≥≥+ ,
]
≥≥, -
>
≥≥- .
>
≥≥. /
DownloadFile
≥≥0 <
(
≥≥< =
[
¥¥ 
	FromRoute
¥¥ 
]
¥¥ 
Guid
¥¥ 
id
¥¥ 
,
¥¥  
CancellationToken
µµ 
cancellationToken
µµ /
=
µµ0 1
default
µµ2 9
)
µµ9 :
{
∂∂ 	
var
∑∑ 
result
∑∑ 
=
∑∑ 
await
∑∑ 
	_mediator
∑∑ (
.
∑∑( )
Send
∑∑) -
(
∑∑- .
new
∑∑. 1
DownloadFileQuery
∑∑2 C
(
∑∑C D
id
∑∑D F
:
∑∑F G
id
∑∑H J
)
∑∑J K
,
∑∑K L
cancellationToken
∑∑M ^
)
∑∑^ _
;
∑∑_ `
if
∏∏ 
(
∏∏ 
result
∏∏ 
==
∏∏ 
null
∏∏ 
)
∏∏ 
{
ππ 
return
∫∫ 
NotFound
∫∫ 
(
∫∫  
)
∫∫  !
;
∫∫! "
}
ªª 
return
ºº 
File
ºº 
(
ºº 
result
ºº 
.
ºº 
Content
ºº &
,
ºº& '
result
ºº( .
.
ºº. /
ContentType
ºº/ :
??
ºº; =
$str
ºº> X
,
ººX Y
result
ººZ `
.
ºº` a
Filename
ººa i
)
ººi j
;
ººj k
}
ΩΩ 	
[
ƒƒ 	
HttpGet
ƒƒ	 
(
ƒƒ 
$str
ƒƒ 3
)
ƒƒ3 4
]
ƒƒ4 5
[
≈≈ 	"
ProducesResponseType
≈≈	 
(
≈≈ 
typeof
≈≈ $
(
≈≈$ %
byte
≈≈% )
[
≈≈) *
]
≈≈* +
)
≈≈+ ,
,
≈≈, -
StatusCodes
≈≈. 9
.
≈≈9 :
Status200OK
≈≈: E
)
≈≈E F
]
≈≈F G
[
∆∆ 	"
ProducesResponseType
∆∆	 
(
∆∆ 
StatusCodes
∆∆ )
.
∆∆) *!
Status400BadRequest
∆∆* =
)
∆∆= >
]
∆∆> ?
[
«« 	"
ProducesResponseType
««	 
(
«« 
StatusCodes
«« )
.
««) *
Status404NotFound
««* ;
)
««; <
]
««< =
[
»» 	"
ProducesResponseType
»»	 
(
»» 
typeof
»» $
(
»»$ %
ProblemDetails
»»% 3
)
»»3 4
,
»»4 5
StatusCodes
»»6 A
.
»»A B*
Status500InternalServerError
»»B ^
)
»»^ _
]
»»_ `
public
…… 
async
…… 
Task
…… 
<
…… 
ActionResult
…… &
<
……& '
byte
……' +
[
……+ ,
]
……, -
>
……- .
>
……. /
SimpleDownload
……0 >
(
……> ?
[
   
	FromQuery
   
]
   
Guid
   
id
   
,
    
CancellationToken
ÀÀ 
cancellationToken
ÀÀ /
=
ÀÀ0 1
default
ÀÀ2 9
)
ÀÀ9 :
{
ÃÃ 	
var
ÕÕ 
result
ÕÕ 
=
ÕÕ 
await
ÕÕ 
	_mediator
ÕÕ (
.
ÕÕ( )
Send
ÕÕ) -
(
ÕÕ- .
new
ÕÕ. 1!
SimpleDownloadQuery
ÕÕ2 E
(
ÕÕE F
id
ÕÕF H
:
ÕÕH I
id
ÕÕJ L
)
ÕÕL M
,
ÕÕM N
cancellationToken
ÕÕO `
)
ÕÕ` a
;
ÕÕa b
if
ŒŒ 
(
ŒŒ 
result
ŒŒ 
==
ŒŒ 
null
ŒŒ 
)
ŒŒ 
{
œœ 
return
–– 
NotFound
–– 
(
––  
)
––  !
;
––! "
}
—— 
return
““ 
File
““ 
(
““ 
result
““ 
.
““ 
Content
““ &
,
““& '
$str
““( B
)
““B C
;
““C D
}
”” 	
}
‘‘ 
}’’ ‚
§D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\FileTransfer\BinaryContentAttribute.cs
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
} ∂¢
îD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\CustomersController.cs
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
ÑÑ 	
HttpPost
ÑÑ	 
(
ÑÑ 
$str
ÑÑ !
)
ÑÑ! "
]
ÑÑ" #
[
ÖÖ 	"
ProducesResponseType
ÖÖ	 
(
ÖÖ 
StatusCodes
ÖÖ )
.
ÖÖ) *
Status201Created
ÖÖ* :
)
ÖÖ: ;
]
ÖÖ; <
[
ÜÜ 	"
ProducesResponseType
ÜÜ	 
(
ÜÜ 
StatusCodes
ÜÜ )
.
ÜÜ) *!
Status400BadRequest
ÜÜ* =
)
ÜÜ= >
]
ÜÜ> ?
[
áá 	"
ProducesResponseType
áá	 
(
áá 
typeof
áá $
(
áá$ %
ProblemDetails
áá% 3
)
áá3 4
,
áá4 5
StatusCodes
áá6 A
.
ááA B*
Status500InternalServerError
ááB ^
)
áá^ _
]
áá_ `
public
àà 
async
àà 
Task
àà 
<
àà 
ActionResult
àà &
>
àà& '
CreateQuote
àà( 3
(
àà3 4
[
ââ 
FromBody
ââ 
]
ââ  
CreateQuoteCommand
ââ )
command
ââ* 1
,
ââ1 2
CancellationToken
ää 
cancellationToken
ää /
=
ää0 1
default
ää2 9
)
ää9 :
{
ãã 	
await
åå 
	_mediator
åå 
.
åå 
Send
åå  
(
åå  !
command
åå! (
,
åå( )
cancellationToken
åå* ;
)
åå; <
;
åå< =
return
çç 
Created
çç 
(
çç 
string
çç !
.
çç! "
Empty
çç" '
,
çç' (
null
çç) -
)
çç- .
;
çç. /
}
éé 	
[
îî 	
HttpPut
îî	 
(
îî 
$str
îî 4
)
îî4 5
]
îî5 6
[
ïï 	"
ProducesResponseType
ïï	 
(
ïï 
StatusCodes
ïï )
.
ïï) * 
Status204NoContent
ïï* <
)
ïï< =
]
ïï= >
[
ññ 	"
ProducesResponseType
ññ	 
(
ññ 
StatusCodes
ññ )
.
ññ) *!
Status400BadRequest
ññ* =
)
ññ= >
]
ññ> ?
[
óó 	"
ProducesResponseType
óó	 
(
óó 
typeof
óó $
(
óó$ %
ProblemDetails
óó% 3
)
óó3 4
,
óó4 5
StatusCodes
óó6 A
.
óóA B*
Status500InternalServerError
óóB ^
)
óó^ _
]
óó_ `
public
òò 
async
òò 
Task
òò 
<
òò 
ActionResult
òò &
>
òò& ' 
DeactivateCustomer
òò( :
(
òò: ;
[
ôô 
FromBody
ôô 
]
ôô '
DeactivateCustomerCommand
ôô 0
command
ôô1 8
,
ôô8 9
CancellationToken
öö 
cancellationToken
öö /
=
öö0 1
default
öö2 9
)
öö9 :
{
õõ 	
await
úú 
	_mediator
úú 
.
úú 
Send
úú  
(
úú  !
command
úú! (
,
úú( )
cancellationToken
úú* ;
)
úú; <
;
úú< =
return
ùù 
	NoContent
ùù 
(
ùù 
)
ùù 
;
ùù 
}
ûû 	
[
•• 	

HttpDelete
••	 
(
•• 
$str
•• '
)
••' (
]
••( )
[
¶¶ 	"
ProducesResponseType
¶¶	 
(
¶¶ 
StatusCodes
¶¶ )
.
¶¶) *
Status200OK
¶¶* 5
)
¶¶5 6
]
¶¶6 7
[
ßß 	"
ProducesResponseType
ßß	 
(
ßß 
StatusCodes
ßß )
.
ßß) *!
Status400BadRequest
ßß* =
)
ßß= >
]
ßß> ?
[
®® 	"
ProducesResponseType
®®	 
(
®® 
StatusCodes
®® )
.
®®) *
Status404NotFound
®®* ;
)
®®; <
]
®®< =
[
©© 	"
ProducesResponseType
©©	 
(
©© 
typeof
©© $
(
©©$ %
ProblemDetails
©©% 3
)
©©3 4
,
©©4 5
StatusCodes
©©6 A
.
©©A B*
Status500InternalServerError
©©B ^
)
©©^ _
]
©©_ `
public
™™ 
async
™™ 
Task
™™ 
<
™™ 
ActionResult
™™ &
>
™™& '
DeleteCustomer
™™( 6
(
™™6 7
[
™™7 8
	FromRoute
™™8 A
]
™™A B
Guid
™™C G
id
™™H J
,
™™J K
CancellationToken
™™L ]
cancellationToken
™™^ o
=
™™p q
default
™™r y
)
™™y z
{
´´ 	
await
¨¨ 
	_mediator
¨¨ 
.
¨¨ 
Send
¨¨  
(
¨¨  !
new
¨¨! $#
DeleteCustomerCommand
¨¨% :
(
¨¨: ;
id
¨¨; =
:
¨¨= >
id
¨¨? A
)
¨¨A B
,
¨¨B C
cancellationToken
¨¨D U
)
¨¨U V
;
¨¨V W
return
≠≠ 
Ok
≠≠ 
(
≠≠ 
)
≠≠ 
;
≠≠ 
}
ÆÆ 	
[
µµ 	
HttpPut
µµ	 
(
µµ 
$str
µµ %
)
µµ% &
]
µµ& '
[
∂∂ 	"
ProducesResponseType
∂∂	 
(
∂∂ 
StatusCodes
∂∂ )
.
∂∂) * 
Status204NoContent
∂∂* <
)
∂∂< =
]
∂∂= >
[
∑∑ 	"
ProducesResponseType
∑∑	 
(
∑∑ 
StatusCodes
∑∑ )
.
∑∑) *!
Status400BadRequest
∑∑* =
)
∑∑= >
]
∑∑> ?
[
∏∏ 	"
ProducesResponseType
∏∏	 
(
∏∏ 
StatusCodes
∏∏ )
.
∏∏) *
Status404NotFound
∏∏* ;
)
∏∏; <
]
∏∏< =
[
ππ 	"
ProducesResponseType
ππ	 
(
ππ 
typeof
ππ $
(
ππ$ %
ProblemDetails
ππ% 3
)
ππ3 4
,
ππ4 5
StatusCodes
ππ6 A
.
ππA B*
Status500InternalServerError
ππB ^
)
ππ^ _
]
ππ_ `
public
∫∫ 
async
∫∫ 
Task
∫∫ 
<
∫∫ 
ActionResult
∫∫ &
>
∫∫& '.
 UpdateCorporateFuneralCoverQuote
∫∫( H
(
∫∫H I
[
ªª 
	FromRoute
ªª 
]
ªª 
Guid
ªª 
id
ªª 
,
ªª  
[
ºº 
FromBody
ºº 
]
ºº 5
'UpdateCorporateFuneralCoverQuoteCommand
ºº >
command
ºº? F
,
ººF G
CancellationToken
ΩΩ 
cancellationToken
ΩΩ /
=
ΩΩ0 1
default
ΩΩ2 9
)
ΩΩ9 :
{
ææ 	
if
øø 
(
øø 
command
øø 
.
øø 
Id
øø 
==
øø 
Guid
øø "
.
øø" #
Empty
øø# (
)
øø( )
{
¿¿ 
command
¡¡ 
.
¡¡ 
Id
¡¡ 
=
¡¡ 
id
¡¡ 
;
¡¡  
}
¬¬ 
if
ƒƒ 
(
ƒƒ 
id
ƒƒ 
!=
ƒƒ 
command
ƒƒ 
.
ƒƒ 
Id
ƒƒ  
)
ƒƒ  !
{
≈≈ 
return
∆∆ 

BadRequest
∆∆ !
(
∆∆! "
)
∆∆" #
;
∆∆# $
}
«« 
await
…… 
	_mediator
…… 
.
…… 
Send
……  
(
……  !
command
……! (
,
……( )
cancellationToken
……* ;
)
……; <
;
……< =
return
   
	NoContent
   
(
   
)
   
;
   
}
ÀÀ 	
[
““ 	
HttpPut
““	 
(
““ 
$str
““ $
)
““$ %
]
““% &
[
”” 	"
ProducesResponseType
””	 
(
”” 
StatusCodes
”” )
.
””) * 
Status204NoContent
””* <
)
””< =
]
””= >
[
‘‘ 	"
ProducesResponseType
‘‘	 
(
‘‘ 
StatusCodes
‘‘ )
.
‘‘) *!
Status400BadRequest
‘‘* =
)
‘‘= >
]
‘‘> ?
[
’’ 	"
ProducesResponseType
’’	 
(
’’ 
StatusCodes
’’ )
.
’’) *
Status404NotFound
’’* ;
)
’’; <
]
’’< =
[
÷÷ 	"
ProducesResponseType
÷÷	 
(
÷÷ 
typeof
÷÷ $
(
÷÷$ %
ProblemDetails
÷÷% 3
)
÷÷3 4
,
÷÷4 5
StatusCodes
÷÷6 A
.
÷÷A B*
Status500InternalServerError
÷÷B ^
)
÷÷^ _
]
÷÷_ `
public
◊◊ 
async
◊◊ 
Task
◊◊ 
<
◊◊ 
ActionResult
◊◊ &
>
◊◊& '
UpdateCustomer
◊◊( 6
(
◊◊6 7
[
ÿÿ 
	FromRoute
ÿÿ 
]
ÿÿ 
Guid
ÿÿ 
id
ÿÿ 
,
ÿÿ  
[
ŸŸ 
FromBody
ŸŸ 
]
ŸŸ #
UpdateCustomerCommand
ŸŸ ,
command
ŸŸ- 4
,
ŸŸ4 5
CancellationToken
⁄⁄ 
cancellationToken
⁄⁄ /
=
⁄⁄0 1
default
⁄⁄2 9
)
⁄⁄9 :
{
€€ 	
if
‹‹ 
(
‹‹ 
command
‹‹ 
.
‹‹ 
Id
‹‹ 
==
‹‹ 
Guid
‹‹ "
.
‹‹" #
Empty
‹‹# (
)
‹‹( )
{
›› 
command
ﬁﬁ 
.
ﬁﬁ 
Id
ﬁﬁ 
=
ﬁﬁ 
id
ﬁﬁ 
;
ﬁﬁ  
}
ﬂﬂ 
if
·· 
(
·· 
id
·· 
!=
·· 
command
·· 
.
·· 
Id
··  
)
··  !
{
‚‚ 
return
„„ 

BadRequest
„„ !
(
„„! "
)
„„" #
;
„„# $
}
‰‰ 
await
ÊÊ 
	_mediator
ÊÊ 
.
ÊÊ 
Send
ÊÊ  
(
ÊÊ  !
command
ÊÊ! (
,
ÊÊ( )
cancellationToken
ÊÊ* ;
)
ÊÊ; <
;
ÊÊ< =
return
ÁÁ 
	NoContent
ÁÁ 
(
ÁÁ 
)
ÁÁ 
;
ÁÁ 
}
ËË 	
[
ÔÔ 	
HttpGet
ÔÔ	 
(
ÔÔ 
$str
ÔÔ $
)
ÔÔ$ %
]
ÔÔ% &
[
 	"
ProducesResponseType
	 
(
 
typeof
 $
(
$ %
CustomerDto
% 0
)
0 1
,
1 2
StatusCodes
3 >
.
> ?
Status200OK
? J
)
J K
]
K L
[
ÒÒ 	"
ProducesResponseType
ÒÒ	 
(
ÒÒ 
StatusCodes
ÒÒ )
.
ÒÒ) *!
Status400BadRequest
ÒÒ* =
)
ÒÒ= >
]
ÒÒ> ?
[
ÚÚ 	"
ProducesResponseType
ÚÚ	 
(
ÚÚ 
StatusCodes
ÚÚ )
.
ÚÚ) *
Status404NotFound
ÚÚ* ;
)
ÚÚ; <
]
ÚÚ< =
[
ÛÛ 	"
ProducesResponseType
ÛÛ	 
(
ÛÛ 
typeof
ÛÛ $
(
ÛÛ$ %
ProblemDetails
ÛÛ% 3
)
ÛÛ3 4
,
ÛÛ4 5
StatusCodes
ÛÛ6 A
.
ÛÛA B*
Status500InternalServerError
ÛÛB ^
)
ÛÛ^ _
]
ÛÛ_ `
public
ÙÙ 
async
ÙÙ 
Task
ÙÙ 
<
ÙÙ 
ActionResult
ÙÙ &
<
ÙÙ& '
CustomerDto
ÙÙ' 2
>
ÙÙ2 3
>
ÙÙ3 4
GetCustomerById
ÙÙ5 D
(
ÙÙD E
[
ıı 
	FromRoute
ıı 
]
ıı 
Guid
ıı 
id
ıı 
,
ıı  
CancellationToken
ˆˆ 
cancellationToken
ˆˆ /
=
ˆˆ0 1
default
ˆˆ2 9
)
ˆˆ9 :
{
˜˜ 	
var
¯¯ 
result
¯¯ 
=
¯¯ 
await
¯¯ 
	_mediator
¯¯ (
.
¯¯( )
Send
¯¯) -
(
¯¯- .
new
¯¯. 1"
GetCustomerByIdQuery
¯¯2 F
(
¯¯F G
id
¯¯G I
:
¯¯I J
id
¯¯K M
)
¯¯M N
,
¯¯N O
cancellationToken
¯¯P a
)
¯¯a b
;
¯¯b c
return
˘˘ 
result
˘˘ 
==
˘˘ 
null
˘˘ !
?
˘˘" #
NotFound
˘˘$ ,
(
˘˘, -
)
˘˘- .
:
˘˘/ 0
Ok
˘˘1 3
(
˘˘3 4
result
˘˘4 :
)
˘˘: ;
;
˘˘; <
}
˙˙ 	
[
ÄÄ 	
HttpGet
ÄÄ	 
(
ÄÄ 
$str
ÄÄ 0
)
ÄÄ0 1
]
ÄÄ1 2
[
ÅÅ 	"
ProducesResponseType
ÅÅ	 
(
ÅÅ 
typeof
ÅÅ $
(
ÅÅ$ %
List
ÅÅ% )
<
ÅÅ) *
CustomerDto
ÅÅ* 5
>
ÅÅ5 6
)
ÅÅ6 7
,
ÅÅ7 8
StatusCodes
ÅÅ9 D
.
ÅÅD E
Status200OK
ÅÅE P
)
ÅÅP Q
]
ÅÅQ R
[
ÇÇ 	"
ProducesResponseType
ÇÇ	 
(
ÇÇ 
StatusCodes
ÇÇ )
.
ÇÇ) *!
Status400BadRequest
ÇÇ* =
)
ÇÇ= >
]
ÇÇ> ?
[
ÉÉ 	"
ProducesResponseType
ÉÉ	 
(
ÉÉ 
typeof
ÉÉ $
(
ÉÉ$ %
ProblemDetails
ÉÉ% 3
)
ÉÉ3 4
,
ÉÉ4 5
StatusCodes
ÉÉ6 A
.
ÉÉA B*
Status500InternalServerError
ÉÉB ^
)
ÉÉ^ _
]
ÉÉ_ `
public
ÑÑ 
async
ÑÑ 
Task
ÑÑ 
<
ÑÑ 
ActionResult
ÑÑ &
<
ÑÑ& '
List
ÑÑ' +
<
ÑÑ+ ,
CustomerDto
ÑÑ, 7
>
ÑÑ7 8
>
ÑÑ8 9
>
ÑÑ9 :*
GetCustomersByNameAndSurname
ÑÑ; W
(
ÑÑW X
[
ÖÖ 
	FromRoute
ÖÖ 
]
ÖÖ 
string
ÖÖ 
name
ÖÖ #
,
ÖÖ# $
[
ÜÜ 
	FromRoute
ÜÜ 
]
ÜÜ 
string
ÜÜ 
surname
ÜÜ &
,
ÜÜ& '
CancellationToken
áá 
cancellationToken
áá /
=
áá0 1
default
áá2 9
)
áá9 :
{
àà 	
var
ââ 
result
ââ 
=
ââ 
await
ââ 
	_mediator
ââ (
.
ââ( )
Send
ââ) -
(
ââ- .
new
ââ. 1/
!GetCustomersByNameAndSurnameQuery
ââ2 S
(
ââS T
name
ââT X
:
ââX Y
name
ââZ ^
,
ââ^ _
surname
ââ` g
:
ââg h
surname
ââi p
)
ââp q
,
ââq r 
cancellationTokenââs Ñ
)ââÑ Ö
;ââÖ Ü
return
ää 
Ok
ää 
(
ää 
result
ää 
)
ää 
;
ää 
}
ãã 	
[
ëë 	
HttpGet
ëë	 
(
ëë 
$str
ëë A
)
ëëA B
]
ëëB C
[
íí 	"
ProducesResponseType
íí	 
(
íí 
typeof
íí $
(
íí$ %
PagedResult
íí% 0
<
íí0 1
CustomerDto
íí1 <
>
íí< =
)
íí= >
,
íí> ?
StatusCodes
íí@ K
.
ííK L
Status200OK
ííL W
)
ííW X
]
ííX Y
[
ìì 	"
ProducesResponseType
ìì	 
(
ìì 
StatusCodes
ìì )
.
ìì) *!
Status400BadRequest
ìì* =
)
ìì= >
]
ìì> ?
[
îî 	"
ProducesResponseType
îî	 
(
îî 
typeof
îî $
(
îî$ %
ProblemDetails
îî% 3
)
îî3 4
,
îî4 5
StatusCodes
îî6 A
.
îîA B*
Status500InternalServerError
îîB ^
)
îî^ _
]
îî_ `
public
ïï 
async
ïï 
Task
ïï 
<
ïï 
ActionResult
ïï &
<
ïï& '
PagedResult
ïï' 2
<
ïï2 3
CustomerDto
ïï3 >
>
ïï> ?
>
ïï? @
>
ïï@ A#
GetCustomersPaginated
ïïB W
(
ïïW X
[
ññ 
	FromRoute
ññ 
]
ññ 
bool
ññ 
isActive
ññ %
,
ññ% &
[
óó 
	FromRoute
óó 
]
óó 
string
óó 
name
óó #
,
óó# $
[
òò 
	FromRoute
òò 
]
òò 
string
òò 
surname
òò &
,
òò& '
[
ôô 
	FromQuery
ôô 
]
ôô 
int
ôô 
pageNo
ôô "
,
ôô" #
[
öö 
	FromQuery
öö 
]
öö 
int
öö 
pageSize
öö $
,
öö$ %
CancellationToken
õõ 
cancellationToken
õõ /
=
õõ0 1
default
õõ2 9
)
õõ9 :
{
úú 	
var
ùù 
result
ùù 
=
ùù 
await
ùù 
	_mediator
ùù (
.
ùù( )
Send
ùù) -
(
ùù- .
new
ùù. 1(
GetCustomersPaginatedQuery
ùù2 L
(
ùùL M
isActive
ùùM U
:
ùùU V
isActive
ùùW _
,
ùù_ `
name
ùùa e
:
ùùe f
name
ùùg k
,
ùùk l
surname
ùùm t
:
ùùt u
surname
ùùv }
,
ùù} ~
pageNoùù Ö
:ùùÖ Ü
pageNoùùá ç
,ùùç é
pageSizeùùè ó
:ùùó ò
pageSizeùùô °
)ùù° ¢
,ùù¢ £!
cancellationTokenùù§ µ
)ùùµ ∂
;ùù∂ ∑
return
ûû 
Ok
ûû 
(
ûû 
result
ûû 
)
ûû 
;
ûû 
}
üü 	
[
•• 	
HttpGet
••	 
(
•• 
$str
•• I
)
••I J
]
••J K
[
¶¶ 	"
ProducesResponseType
¶¶	 
(
¶¶ 
typeof
¶¶ $
(
¶¶$ %
PagedResult
¶¶% 0
<
¶¶0 1
CustomerDto
¶¶1 <
>
¶¶< =
)
¶¶= >
,
¶¶> ?
StatusCodes
¶¶@ K
.
¶¶K L
Status200OK
¶¶L W
)
¶¶W X
]
¶¶X Y
[
ßß 	"
ProducesResponseType
ßß	 
(
ßß 
StatusCodes
ßß )
.
ßß) *!
Status400BadRequest
ßß* =
)
ßß= >
]
ßß> ?
[
®® 	"
ProducesResponseType
®®	 
(
®® 
typeof
®® $
(
®®$ %
ProblemDetails
®®% 3
)
®®3 4
,
®®4 5
StatusCodes
®®6 A
.
®®A B*
Status500InternalServerError
®®B ^
)
®®^ _
]
®®_ `
public
©© 
async
©© 
Task
©© 
<
©© 
ActionResult
©© &
<
©©& '
PagedResult
©©' 2
<
©©2 3
CustomerDto
©©3 >
>
©©> ?
>
©©? @
>
©©@ A,
GetCustomersPaginatedWithOrder
©©B `
(
©©` a
[
™™ 
	FromRoute
™™ 
]
™™ 
bool
™™ 
isActive
™™ %
,
™™% &
[
´´ 
	FromRoute
´´ 
]
´´ 
string
´´ 
name
´´ #
,
´´# $
[
¨¨ 
	FromRoute
¨¨ 
]
¨¨ 
string
¨¨ 
surname
¨¨ &
,
¨¨& '
[
≠≠ 
	FromQuery
≠≠ 
]
≠≠ 
int
≠≠ 
pageNo
≠≠ "
,
≠≠" #
[
ÆÆ 
	FromQuery
ÆÆ 
]
ÆÆ 
int
ÆÆ 
pageSize
ÆÆ $
,
ÆÆ$ %
[
ØØ 
	FromQuery
ØØ 
]
ØØ 
string
ØØ 
orderBy
ØØ &
,
ØØ& '
CancellationToken
∞∞ 
cancellationToken
∞∞ /
=
∞∞0 1
default
∞∞2 9
)
∞∞9 :
{
±± 	
var
≤≤ 
result
≤≤ 
=
≤≤ 
await
≤≤ 
	_mediator
≤≤ (
.
≤≤( )
Send
≤≤) -
(
≤≤- .
new
≤≤. 11
#GetCustomersPaginatedWithOrderQuery
≤≤2 U
(
≤≤U V
isActive
≤≤V ^
:
≤≤^ _
isActive
≤≤` h
,
≤≤h i
name
≤≤j n
:
≤≤n o
name
≤≤p t
,
≤≤t u
surname
≤≤v }
:
≤≤} ~
surname≤≤ Ü
,≤≤Ü á
pageNo≤≤à é
:≤≤é è
pageNo≤≤ê ñ
,≤≤ñ ó
pageSize≤≤ò †
:≤≤† °
pageSize≤≤¢ ™
,≤≤™ ´
orderBy≤≤¨ ≥
:≤≤≥ ¥
orderBy≤≤µ º
)≤≤º Ω
,≤≤Ω æ!
cancellationToken≤≤ø –
)≤≤– —
;≤≤— “
return
≥≥ 
Ok
≥≥ 
(
≥≥ 
result
≥≥ 
)
≥≥ 
;
≥≥ 
}
¥¥ 	
[
ππ 	
HttpGet
ππ	 
(
ππ 
$str
ππ  
)
ππ  !
]
ππ! "
[
∫∫ 	"
ProducesResponseType
∫∫	 
(
∫∫ 
typeof
∫∫ $
(
∫∫$ %
List
∫∫% )
<
∫∫) *
CustomerDto
∫∫* 5
>
∫∫5 6
)
∫∫6 7
,
∫∫7 8
StatusCodes
∫∫9 D
.
∫∫D E
Status200OK
∫∫E P
)
∫∫P Q
]
∫∫Q R
[
ªª 	"
ProducesResponseType
ªª	 
(
ªª 
typeof
ªª $
(
ªª$ %
ProblemDetails
ªª% 3
)
ªª3 4
,
ªª4 5
StatusCodes
ªª6 A
.
ªªA B*
Status500InternalServerError
ªªB ^
)
ªª^ _
]
ªª_ `
public
ºº 
async
ºº 
Task
ºº 
<
ºº 
ActionResult
ºº &
<
ºº& '
List
ºº' +
<
ºº+ ,
CustomerDto
ºº, 7
>
ºº7 8
>
ºº8 9
>
ºº9 :
GetCustomers
ºº; G
(
ººG H
ODataQueryOptions
ΩΩ 
<
ΩΩ 
CustomerDto
ΩΩ )
>
ΩΩ) *
oDataOptions
ΩΩ+ 7
,
ΩΩ7 8
CancellationToken
ææ 
cancellationToken
ææ /
=
ææ0 1
default
ææ2 9
)
ææ9 :
{
øø 	"
ValidateODataOptions
¿¿  
(
¿¿  !
oDataOptions
¿¿! -
)
¿¿- .
;
¿¿. /
var
¬¬ 
result
¬¬ 
=
¬¬ 
await
¬¬ 
	_mediator
¬¬ (
.
¬¬( )
Send
¬¬) -
(
¬¬- .
new
¬¬. 1
GetCustomersQuery
¬¬2 C
(
¬¬C D
oDataOptions
¬¬D P
.
¬¬P Q
ApplyTo
¬¬Q X
)
¬¬X Y
,
¬¬Y Z
cancellationToken
¬¬[ l
)
¬¬l m
;
¬¬m n
return
ƒƒ 
Ok
ƒƒ 
(
ƒƒ 
result
ƒƒ 
)
ƒƒ 
;
ƒƒ 
}
≈≈ 	
[
ÃÃ 	
HttpGet
ÃÃ	 
(
ÃÃ 
$str
ÃÃ +
)
ÃÃ+ ,
]
ÃÃ, -
[
ÕÕ 	"
ProducesResponseType
ÕÕ	 
(
ÕÕ 
typeof
ÕÕ $
(
ÕÕ$ %
int
ÕÕ% (
)
ÕÕ( )
,
ÕÕ) *
StatusCodes
ÕÕ+ 6
.
ÕÕ6 7
Status200OK
ÕÕ7 B
)
ÕÕB C
]
ÕÕC D
[
ŒŒ 	"
ProducesResponseType
ŒŒ	 
(
ŒŒ 
StatusCodes
ŒŒ )
.
ŒŒ) *!
Status400BadRequest
ŒŒ* =
)
ŒŒ= >
]
ŒŒ> ?
[
œœ 	"
ProducesResponseType
œœ	 
(
œœ 
StatusCodes
œœ )
.
œœ) *
Status404NotFound
œœ* ;
)
œœ; <
]
œœ< =
[
–– 	"
ProducesResponseType
––	 
(
–– 
typeof
–– $
(
––$ %
ProblemDetails
––% 3
)
––3 4
,
––4 5
StatusCodes
––6 A
.
––A B*
Status500InternalServerError
––B ^
)
––^ _
]
––_ `
public
—— 
async
—— 
Task
—— 
<
—— 
ActionResult
—— &
<
——& '
int
——' *
>
——* +
>
——+ ,#
GetCustomerStatistics
——- B
(
——B C
[
““ 
	FromQuery
““ 
]
““ 
Guid
““ 

customerId
““ '
,
““' (
CancellationToken
”” 
cancellationToken
”” /
=
””0 1
default
””2 9
)
””9 :
{
‘‘ 	
var
’’ 
result
’’ 
=
’’ 
await
’’ 
	_mediator
’’ (
.
’’( )
Send
’’) -
(
’’- .
new
’’. 1(
GetCustomerStatisticsQuery
’’2 L
(
’’L M

customerId
’’M W
:
’’W X

customerId
’’Y c
)
’’c d
,
’’d e
cancellationToken
’’f w
)
’’w x
;
’’x y
return
÷÷ 
Ok
÷÷ 
(
÷÷ 
result
÷÷ 
)
÷÷ 
;
÷÷ 
}
◊◊ 	
[
›› 	
HttpGet
››	 
(
›› 
$str
›› ;
)
››; <
]
››< =
[
ﬁﬁ 	"
ProducesResponseType
ﬁﬁ	 
(
ﬁﬁ 
typeof
ﬁﬁ $
(
ﬁﬁ$ %
List
ﬁﬁ% )
<
ﬁﬁ) *
CustomerDto
ﬁﬁ* 5
>
ﬁﬁ5 6
)
ﬁﬁ6 7
,
ﬁﬁ7 8
StatusCodes
ﬁﬁ9 D
.
ﬁﬁD E
Status200OK
ﬁﬁE P
)
ﬁﬁP Q
]
ﬁﬁQ R
[
ﬂﬂ 	"
ProducesResponseType
ﬂﬂ	 
(
ﬂﬂ 
StatusCodes
ﬂﬂ )
.
ﬂﬂ) *!
Status400BadRequest
ﬂﬂ* =
)
ﬂﬂ= >
]
ﬂﬂ> ?
[
‡‡ 	"
ProducesResponseType
‡‡	 
(
‡‡ 
typeof
‡‡ $
(
‡‡$ %
ProblemDetails
‡‡% 3
)
‡‡3 4
,
‡‡4 5
StatusCodes
‡‡6 A
.
‡‡A B*
Status500InternalServerError
‡‡B ^
)
‡‡^ _
]
‡‡_ `
public
·· 
async
·· 
Task
·· 
<
·· 
ActionResult
·· &
<
··& '
List
··' +
<
··+ ,
CustomerDto
··, 7
>
··7 8
>
··8 9
>
··9 :$
GetCustomersWithParams
··; Q
(
··Q R
[
‚‚ 
	FromRoute
‚‚ 
]
‚‚ 
bool
‚‚ 
isActive
‚‚ %
,
‚‚% &
[
„„ 
	FromRoute
„„ 
]
„„ 
string
„„ 
?
„„ 
name
„„  $
,
„„$ %
[
‰‰ 
	FromRoute
‰‰ 
]
‰‰ 
string
‰‰ 
?
‰‰ 
surname
‰‰  '
,
‰‰' (
CancellationToken
ÂÂ 
cancellationToken
ÂÂ /
=
ÂÂ0 1
default
ÂÂ2 9
)
ÂÂ9 :
{
ÊÊ 	
var
ÁÁ 
result
ÁÁ 
=
ÁÁ 
await
ÁÁ 
	_mediator
ÁÁ (
.
ÁÁ( )
Send
ÁÁ) -
(
ÁÁ- .
new
ÁÁ. 1)
GetCustomersWithParamsQuery
ÁÁ2 M
(
ÁÁM N
isActive
ÁÁN V
:
ÁÁV W
isActive
ÁÁX `
,
ÁÁ` a
name
ÁÁb f
:
ÁÁf g
name
ÁÁh l
,
ÁÁl m
surname
ÁÁn u
:
ÁÁu v
surname
ÁÁw ~
)
ÁÁ~ 
,ÁÁ Ä!
cancellationTokenÁÁÅ í
)ÁÁí ì
;ÁÁì î
return
ËË 
Ok
ËË 
(
ËË 
result
ËË 
)
ËË 
;
ËË 
}
ÈÈ 	
private
ÎÎ 
static
ÎÎ 
void
ÎÎ "
ValidateODataOptions
ÎÎ 0
<
ÎÎ0 1
TDto
ÎÎ1 5
>
ÎÎ5 6
(
ÎÎ6 7
ODataQueryOptions
ÎÎ7 H
<
ÎÎH I
TDto
ÎÎI M
>
ÎÎM N
options
ÎÎO V
,
ÎÎV W
bool
ÎÎX \
enableSelect
ÎÎ] i
=
ÎÎj k
false
ÎÎl q
)
ÎÎq r
{
ÏÏ 	
var
ÌÌ 
settings
ÌÌ 
=
ÌÌ 
new
ÌÌ %
ODataValidationSettings
ÌÌ 6
(
ÌÌ6 7
)
ÌÌ7 8
;
ÌÌ8 9
if
ÔÔ 
(
ÔÔ 
!
ÔÔ 
enableSelect
ÔÔ 
)
ÔÔ 
{
 
settings
ÒÒ 
.
ÒÒ !
AllowedQueryOptions
ÒÒ ,
=
ÒÒ- .!
AllowedQueryOptions
ÒÒ/ B
.
ÒÒB C
All
ÒÒC F
&
ÒÒG H
~
ÒÒI J!
AllowedQueryOptions
ÒÒJ ]
.
ÒÒ] ^
Select
ÒÒ^ d
;
ÒÒd e
}
ÚÚ 
options
ÛÛ 
.
ÛÛ 
Validate
ÛÛ 
(
ÛÛ 
settings
ÛÛ %
)
ÛÛ% &
;
ÛÛ& '
}
ÙÙ 	
}
ıı 
}ˆˆ úq
ëD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Controllers\BasicsController.cs
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
ÄÄ 
	FromQuery
ÄÄ 
]
ÄÄ 
int
ÄÄ 
pageSize
ÄÄ $
,
ÄÄ$ %
[
ÅÅ 
	FromQuery
ÅÅ 
]
ÅÅ 
string
ÅÅ 
?
ÅÅ 
orderBy
ÅÅ  '
,
ÅÅ' (
CancellationToken
ÇÇ 
cancellationToken
ÇÇ /
=
ÇÇ0 1
default
ÇÇ2 9
)
ÇÇ9 :
{
ÉÉ 	
var
ÑÑ 
result
ÑÑ 
=
ÑÑ 
await
ÑÑ 
	_mediator
ÑÑ (
.
ÑÑ( )
Send
ÑÑ) -
(
ÑÑ- .
new
ÑÑ. 1$
GetBasicsNullableQuery
ÑÑ2 H
(
ÑÑH I
pageNo
ÑÑI O
:
ÑÑO P
pageNo
ÑÑQ W
,
ÑÑW X
pageSize
ÑÑY a
:
ÑÑa b
pageSize
ÑÑc k
,
ÑÑk l
orderBy
ÑÑm t
:
ÑÑt u
orderBy
ÑÑv }
)
ÑÑ} ~
,
ÑÑ~ !
cancellationTokenÑÑÄ ë
)ÑÑë í
;ÑÑí ì
return
ÖÖ 
Ok
ÖÖ 
(
ÖÖ 
result
ÖÖ 
)
ÖÖ 
;
ÖÖ 
}
ÜÜ 	
[
åå 	
HttpGet
åå	 
(
åå 
$str
åå 
)
åå 
]
åå 
[
çç 	"
ProducesResponseType
çç	 
(
çç 
typeof
çç $
(
çç$ %
PagedResult
çç% 0
<
çç0 1
BasicDto
çç1 9
>
çç9 :
)
çç: ;
,
çç; <
StatusCodes
çç= H
.
ççH I
Status200OK
ççI T
)
ççT U
]
ççU V
[
éé 	"
ProducesResponseType
éé	 
(
éé 
StatusCodes
éé )
.
éé) *!
Status400BadRequest
éé* =
)
éé= >
]
éé> ?
[
èè 	"
ProducesResponseType
èè	 
(
èè 
typeof
èè $
(
èè$ %
ProblemDetails
èè% 3
)
èè3 4
,
èè4 5
StatusCodes
èè6 A
.
èèA B*
Status500InternalServerError
èèB ^
)
èè^ _
]
èè_ `
public
êê 
async
êê 
Task
êê 
<
êê 
ActionResult
êê &
<
êê& '
PagedResult
êê' 2
<
êê2 3
BasicDto
êê3 ;
>
êê; <
>
êê< =
>
êê= >
	GetBasics
êê? H
(
êêH I
[
ëë 
	FromQuery
ëë 
]
ëë 
int
ëë 
pageNo
ëë "
,
ëë" #
[
íí 
	FromQuery
íí 
]
íí 
int
íí 
pageSize
íí $
,
íí$ %
[
ìì 
	FromQuery
ìì 
]
ìì 
string
ìì 
orderBy
ìì &
,
ìì& '
CancellationToken
îî 
cancellationToken
îî /
=
îî0 1
default
îî2 9
)
îî9 :
{
ïï 	
var
ññ 
result
ññ 
=
ññ 
await
ññ 
	_mediator
ññ (
.
ññ( )
Send
ññ) -
(
ññ- .
new
ññ. 1
GetBasicsQuery
ññ2 @
(
ññ@ A
pageNo
ññA G
:
ññG H
pageNo
ññI O
,
ññO P
pageSize
ññQ Y
:
ññY Z
pageSize
ññ[ c
,
ññc d
orderBy
ññe l
:
ññl m
orderBy
ññn u
)
ññu v
,
ññv w 
cancellationTokenññx â
)ññâ ä
;ññä ã
return
óó 
Ok
óó 
(
óó 
result
óó 
)
óó 
;
óó 
}
òò 	
}
ôô 
}öö ÄZ
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\SwashbuckleConfiguration.cs
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
$str	%%~ Ç
"
%%Ç É
)
%%É Ñ
;
%%Ñ Ö
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
GetName	++| É
(
++É Ñ
)
++Ñ Ö
.
++Ö Ü
Name
++Ü ä
}
++ä ã
$str
++ã è
"
++è ê
)
++ê ë
;
++ë í
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
$str	77& æ
,
77æ ø
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
"	jj Ä
)
jjÄ Å
;
jjÅ Ç
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
}}} Ù
ûD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ProblemDetailsConfiguration.cs
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
)	 Ä
;
Ä Å
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
}$$ ñ
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\HealthChecksConfiguration.cs
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
}## â
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ApplicationSecurityConfiguration.cs
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
}44 º
ûD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ApiVersionSwaggerGenOptions.cs
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
}// ±
ùD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Api\Configuration\ApiVersioningConfiguration.cs
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