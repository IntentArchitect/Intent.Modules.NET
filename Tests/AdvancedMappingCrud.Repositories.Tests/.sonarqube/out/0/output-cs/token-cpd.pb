Ù1
ÉD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\ValueObject.cs
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
$str ?
,? @
VersionA H
=I J
$strK P
)P Q
]Q R
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
{

 
public 

abstract 
class 
ValueObject %
{ 
	protected 
static 
bool 
EqualOperator +
(+ ,
ValueObject, 7
left8 <
,< =
ValueObject> I
rightJ O
)O P
{ 	
if 
( 
left 
is 
null 
^ 
right $
is% '
null( ,
), -
{ 
return 
false 
; 
} 
return 
ReferenceEquals "
(" #
left# '
,' (
right) .
). /
||0 2
left3 7
!7 8
.8 9
Equals9 ?
(? @
right@ E
)E F
;F G
} 	
	protected 
static 
bool 
NotEqualOperator .
(. /
ValueObject/ :
left; ?
,? @
ValueObjectA L
rightM R
)R S
{ 	
return 
! 
EqualOperator !
(! "
left" &
,& '
right( -
)- .
;. /
} 	
	protected   
abstract   
IEnumerable   &
<  & '
object  ' -
?  - .
>  . /!
GetEqualityComponents  0 E
(  E F
)  F G
;  G H
public"" 
override"" 
bool"" 
Equals"" #
(""# $
object""$ *
?""* +
obj"", /
)""/ 0
{## 	
if$$ 
($$ 
obj$$ 
==$$ 
null$$ 
||$$ 
!$$  
AreSameType$$  +
($$+ ,
obj$$, /
,$$/ 0
this$$1 5
)$$5 6
)$$6 7
{%% 
return&& 
false&& 
;&& 
}'' 
var)) 
other)) 
=)) 
()) 
ValueObject)) $
)))$ %
obj))% (
;))( )
return++ !
GetEqualityComponents++ (
(++( )
)++) *
.++* +
SequenceEqual+++ 8
(++8 9
other++9 >
.++> ?!
GetEqualityComponents++? T
(++T U
)++U V
)++V W
;++W X
},, 	
public.. 
override.. 
int.. 
GetHashCode.. '
(..' (
)..( )
{// 	
return00 !
GetEqualityComponents00 (
(00( )
)00) *
.11 
Select11 
(11 
x11 
=>11 
x11 
!=11 !
null11" &
?11' (
x11) *
.11* +
GetHashCode11+ 6
(116 7
)117 8
:119 :
$num11; <
)11< =
.22 
	Aggregate22 
(22 
(22 
x22 
,22 
y22  
)22  !
=>22" $
x22% &
^22' (
y22) *
)22* +
;22+ ,
}33 	
public55 
static55 
bool55 
operator55 #
==55$ &
(55& '
ValueObject55' 2
one553 6
,556 7
ValueObject558 C
two55D G
)55G H
{66 	
return77 
EqualOperator77  
(77  !
one77! $
,77$ %
two77& )
)77) *
;77* +
}88 	
public:: 
static:: 
bool:: 
operator:: #
!=::$ &
(::& '
ValueObject::' 2
one::3 6
,::6 7
ValueObject::8 C
two::D G
)::G H
{;; 	
return<< 
NotEqualOperator<< #
(<<# $
one<<$ '
,<<' (
two<<) ,
)<<, -
;<<- .
}== 	
public?? 
static?? 
bool?? 
AreSameType?? &
(??& '
object??' -
obj1??. 2
,??2 3
object??4 :
obj2??; ?
)??? @
{@@ 	
ifAA 
(AA 
obj1AA 
==AA 
nullAA 
||AA 
obj2AA  $
==AA% '
nullAA( ,
)AA, -
{BB 
returnCC 
falseCC 
;CC 
}DD 
varFF 
type1FF 
=FF 
obj1FF 
.FF 
GetTypeFF $
(FF$ %
)FF% &
;FF& '
varGG 
type2GG 
=GG 
obj2GG 
.GG 
GetTypeGG $
(GG$ %
)GG% &
;GG& '
ifII 
(II 
	IsEFProxyII 
(II 
type1II 
)II  
)II  !
{JJ 
type1KK 
=KK 
type1KK 
.KK 
BaseTypeKK &
!KK& '
;KK' (
}LL 
ifNN 
(NN 
	IsEFProxyNN 
(NN 
type2NN 
)NN  
)NN  !
{OO 
type2PP 
=PP 
type2PP 
.PP 
BaseTypePP &
!PP& '
;PP' (
}QQ 
returnSS 
type1SS 
==SS 
type2SS !
;SS! "
}TT 	
privateVV 
staticVV 
boolVV 
	IsEFProxyVV %
(VV% &
TypeVV& *
typeVV+ /
)VV/ 0
{WW 	
returnXX 
typeXX 
.XX 
	NamespaceXX !
==XX" $
$strXX% 5
;XX5 6
}YY 	
}ZZ 
}[[ ™
{D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Tag.cs
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
$str ;
,; <
Version= D
=E F
$strG L
)L M
]M N
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
{ 
public		 

class		 
Tag		 
:		 
ValueObject		 "
{

 
	protected 
Tag 
( 
) 
{ 	
} 	
public 
Tag 
( 
string 
name 
, 
string  &
value' ,
), -
{ 	
Name 
= 
name 
; 
Value 
= 
value 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 
string 
Value 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
	protected 
override 
IEnumerable &
<& '
object' -
>- .!
GetEqualityComponents/ D
(D E
)E F
{ 	
yield 
return 
Name 
; 
yield 
return 
Value 
; 
} 	
} 
} “
èD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\PricingService.cs
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
$str M
,M N
VersionO V
=W X
$strY ^
)^ _
]_ `
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
.		7 8
Services		8 @
{

 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 
PricingService 
:  !
IPricingService" 1
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
PricingService 
( 
) 
{ 	
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
async 
Task 
< 
decimal !
>! " 
GetProductPriceAsync# 7
(7 8
Guid8 <
	productId= F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
decimal 
	SumPrices  
(  !
decimal! (
prices) /
)/ 0
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
} 	
} 
} ì

êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\IPricingService.cs
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
$str H
,H I
VersionJ Q
=R S
$strT Y
)Y Z
]Z [
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
.		7 8
Services		8 @
{

 
public 

	interface 
IPricingService $
{ 
Task 
< 
decimal 
>  
GetProductPriceAsync *
(* +
Guid+ /
	productId0 9
,9 :
CancellationToken; L
cancellationTokenM ^
=_ `
defaulta h
)h i
;i j
decimal 
	SumPrices 
( 
decimal !
prices" (
)( )
;) *
} 
} ˛	
ëD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\ICustomerManager.cs
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
$str H
,H I
VersionJ Q
=R S
$strT Y
)Y Z
]Z [
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
0 1
Domain

1 7
.

7 8
Services

8 @
{ 
public 

	interface 
ICustomerManager %
{ 
CustomerStatistics !
GetCustomerStatistics 0
(0 1
Guid1 5

customerId6 @
)@ A
;A B
Task #
DeactivateCustomerAsync $
($ %
Guid% )

customerId* 4
,4 5
CancellationToken6 G
cancellationTokenH Y
=Z [
default\ c
)c d
;d e
} 
} „
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\ExtensiveDomainServices\IExtensiveDomainService.cs
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
$str		 H
,		H I
Version		J Q
=		R S
$str		T Y
)		Y Z
]		Z [
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Services8 @
.@ A#
ExtensiveDomainServicesA X
{ 
public 

	interface #
IExtensiveDomainService ,
{ 
void 
PerformEntityA 
( 
ConcreteEntityA +
entity, 2
)2 3
;3 4
Task 
PerformEntityAAsync  
(  !
ConcreteEntityA! 0
entity1 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
;g h
void 
PerformEntityB 
( 
ConcreteEntityB +
entity, 2
)2 3
;3 4
Task 
PerformEntityBAsync  
(  !
ConcreteEntityB! 0
entity1 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
;g h
void 
PerformPassthrough 
(  
PassthroughObj  .
obj/ 2
)2 3
;3 4
Task #
PerformPassthroughAsync $
($ %
PassthroughObj% 3
obj4 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
;g h
void 
PerformValueObj 
( 
SimpleVO %
vo& (
)( )
;) *
Task  
PerformValueObjAsync !
(! "
SimpleVO" *
vo+ -
,- .
CancellationToken/ @
cancellationTokenA R
=S T
defaultU \
)\ ]
;] ^
} 
} ç3
ØD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\ExtensiveDomainServices\ExtensiveDomainService.cs
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
 M
,

M N
Version

O V
=

W X
$str

Y ^
)

^ _
]

_ `
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Services8 @
.@ A#
ExtensiveDomainServicesA X
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class "
ExtensiveDomainService '
:( )#
IExtensiveDomainService* A
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public "
ExtensiveDomainService %
(% &
)& '
{ 	
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
void 
PerformEntityA "
(" #
ConcreteEntityA# 2
entity3 9
)9 :
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
async 
Task 
PerformEntityAAsync -
(- .
ConcreteEntityA. =
entity> D
,D E
CancellationTokenF W
cancellationTokenX i
=j k
defaultl s
)s t
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
}   	
["" 	
IntentManaged""	 
("" 
Mode"" 
."" 
Fully"" !
,""! "
Body""# '
=""( )
Mode""* .
."". /
Ignore""/ 5
)""5 6
]""6 7
public## 
void## 
PerformEntityB## "
(##" #
ConcreteEntityB### 2
entity##3 9
)##9 :
{$$ 	
throw%% 
new%% #
NotImplementedException%% -
(%%- .
$str%%. [
)%%[ \
;%%\ ]
}&& 	
[(( 	
IntentManaged((	 
((( 
Mode(( 
.(( 
Fully(( !
,((! "
Body((# '
=((( )
Mode((* .
.((. /
Ignore((/ 5
)((5 6
]((6 7
public)) 
async)) 
Task)) 
PerformEntityBAsync)) -
())- .
ConcreteEntityB)). =
entity))> D
,))D E
CancellationToken))F W
cancellationToken))X i
=))j k
default))l s
)))s t
{** 	
throw++ 
new++ #
NotImplementedException++ -
(++- .
$str++. [
)++[ \
;++\ ]
},, 	
[.. 	
IntentManaged..	 
(.. 
Mode.. 
... 
Fully.. !
,..! "
Body..# '
=..( )
Mode..* .
.... /
Ignore../ 5
)..5 6
]..6 7
public// 
void// 
PerformPassthrough// &
(//& '
PassthroughObj//' 5
obj//6 9
)//9 :
{00 	
throw11 
new11 #
NotImplementedException11 -
(11- .
$str11. [
)11[ \
;11\ ]
}22 	
[44 	
IntentManaged44	 
(44 
Mode44 
.44 
Fully44 !
,44! "
Body44# '
=44( )
Mode44* .
.44. /
Ignore44/ 5
)445 6
]446 7
public55 
async55 
Task55 #
PerformPassthroughAsync55 1
(551 2
PassthroughObj552 @
obj55A D
,55D E
CancellationToken55F W
cancellationToken55X i
=55j k
default55l s
)55s t
{66 	
throw77 
new77 #
NotImplementedException77 -
(77- .
$str77. [
)77[ \
;77\ ]
}88 	
[:: 	
IntentManaged::	 
(:: 
Mode:: 
.:: 
Fully:: !
,::! "
Body::# '
=::( )
Mode::* .
.::. /
Ignore::/ 5
)::5 6
]::6 7
public;; 
void;; 
PerformValueObj;; #
(;;# $
SimpleVO;;$ ,
vo;;- /
);;/ 0
{<< 	
throw== 
new== #
NotImplementedException== -
(==- .
$str==. [
)==[ \
;==\ ]
}>> 	
[@@ 	
IntentManaged@@	 
(@@ 
Mode@@ 
.@@ 
Fully@@ !
,@@! "
Body@@# '
=@@( )
Mode@@* .
.@@. /
Ignore@@/ 5
)@@5 6
]@@6 7
publicAA 
asyncAA 
TaskAA  
PerformValueObjAsyncAA .
(AA. /
SimpleVOAA/ 7
voAA8 :
,AA: ;
CancellationTokenAA< M
cancellationTokenAAN _
=AA` a
defaultAAb i
)AAi j
{BB 	
throwCC 
newCC #
NotImplementedExceptionCC -
(CC- .
$strCC. [
)CC[ \
;CC\ ]
}DD 	
}EE 
}FF ä
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\DomainServices\MyDomainService.cs
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
$str M
,M N
VersionO V
=W X
$strY ^
)^ _
]_ `
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Services8 @
.@ A
DomainServicesA O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Merge		 
,		 
	Signature		 (
=		) *
Mode		+ /
.		/ 0
Fully		0 5
)		5 6
]		6 7
public

 

class

 
MyDomainService

  
:

! "
IMyDomainService

# 3
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
MyDomainService 
( 
)  
{ 	
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
void 
DoSomething 
(  
)  !
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
} 	
} 
} ˜
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\DomainServices\IMyDomainService.cs
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
$str H
,H I
VersionJ Q
=R S
$strT Y
)Y Z
]Z [
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Services8 @
.@ A
DomainServicesA O
{ 
public 

	interface 
IMyDomainService %
{		 
void

 
DoSomething

 
(

 
)

 
;

 
} 
} ø
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Services\CustomerManager.cs
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
0 1
Domain

1 7
.

7 8
Services

8 @
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 
CustomerManager  
:! "
ICustomerManager# 3
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
CustomerManager 
( 
)  
{ 	
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
CustomerStatistics !!
GetCustomerStatistics" 7
(7 8
Guid8 <

customerId= G
)G H
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
async 
Task #
DeactivateCustomerAsync 1
(1 2
Guid2 6

customerId7 A
,A B
CancellationTokenC T
cancellationTokenU f
=g h
defaulti p
)p q
{ 	
throw 
new #
NotImplementedException -
(- .
$str. [
)[ \
;\ ]
} 	
} 
}   Û
™D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\MappingTests\INestingParentRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E
MappingTestsE Q
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface $
INestingParentRepository -
:. /
IEFRepository0 =
<= >
NestingParent> K
,K L
NestingParentM Z
>Z [
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
NestingParent 
? 
> 
FindByIdAsync *
(* +
Guid+ /
id0 2
,2 3
CancellationToken4 E
cancellationTokenF W
=X Y
defaultZ a
)a b
;b c
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
NestingParent 
>  
>  !
FindByIdsAsync" 0
(0 1
Guid1 5
[5 6
]6 7
ids8 ;
,; <
CancellationToken= N
cancellationTokenO `
=a b
defaultc j
)j k
;k l
} 
} •
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IWarehouseRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface  
IWarehouseRepository )
:* +
IEFRepository, 9
<9 :
	Warehouse: C
,C D
	WarehouseE N
>N O
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
	Warehouse 
? 
> 
FindByIdAsync &
(& '
Guid' +
id, .
,. /
CancellationToken0 A
cancellationTokenB S
=T U
defaultV ]
)] ^
;^ _
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
	Warehouse 
> 
> 
FindByIdsAsync ,
(, -
Guid- 1
[1 2
]2 3
ids4 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
;g h
} 
} á
îD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IUserRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IUserRepository $
:% &
IEFRepository' 4
<4 5
User5 9
,9 :
User; ?
>? @
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
User 
? 
> 
FindByIdAsync !
(! "
Guid" &
id' )
,) *
CancellationToken+ <
cancellationToken= N
=O P
defaultQ X
)X Y
;Y Z
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
User 
> 
> 
FindByIdsAsync '
(' (
Guid( ,
[, -
]- .
ids/ 2
,2 3
CancellationToken4 E
cancellationTokenF W
=X Y
defaultZ a
)a b
;b c
} 
} ã
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IRepository.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Fully 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public		 

	interface		 
IRepository		  
<		  !
in		! #
TDomain		$ +
>		+ ,
{

 
void 
Add 
( 
TDomain 
entity 
)  
;  !
void 
Update 
( 
TDomain 
entity "
)" #
;# $
void 
Remove 
( 
TDomain 
entity "
)" #
;# $
} 
} ç
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IQuoteRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IQuoteRepository %
:& '
IEFRepository( 5
<5 6
Quote6 ;
,; <
Quote= B
>B C
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Quote 
? 
> 
FindByIdAsync "
(" #
Guid# '
id( *
,* +
CancellationToken, =
cancellationToken> O
=P Q
defaultR Y
)Y Z
;Z [
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Quote 
> 
> 
FindByIdsAsync (
(( )
Guid) -
[- .
]. /
ids0 3
,3 4
CancellationToken5 F
cancellationTokenG X
=Y Z
default[ b
)b c
;c d
} 
} ô
óD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IProductRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IProductRepository '
:( )
IEFRepository* 7
<7 8
Product8 ?
,? @
ProductA H
>H I
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Product 
? 
> 
FindByIdAsync $
($ %
Guid% )
id* ,
,, -
CancellationToken. ?
cancellationToken@ Q
=R S
defaultT [
)[ \
;\ ]
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Product 
> 
> 
FindByIdsAsync *
(* +
Guid+ /
[/ 0
]0 1
ids2 5
,5 6
CancellationToken7 H
cancellationTokenI Z
=[ \
default] d
)d e
;e f
} 
} ì
ñD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IPersonRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IPersonRepository &
:' (
IEFRepository) 6
<6 7
Person7 =
,= >
Person? E
>E F
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Person 
? 
> 
FindByIdAsync #
(# $
Guid$ (
id) +
,+ ,
CancellationToken- >
cancellationToken? P
=Q R
defaultS Z
)Z [
;[ \
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Person 
> 
> 
FindByIdsAsync )
() *
Guid* .
[. /
]/ 0
ids1 4
,4 5
CancellationToken6 G
cancellationTokenH Y
=Z [
default\ c
)c d
;d e
} 
} ü
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IPagingTSRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IPagingTSRepository (
:) *
IEFRepository+ 8
<8 9
PagingTS9 A
,A B
PagingTSC K
>K L
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
PagingTS 
? 
> 
FindByIdAsync %
(% &
Guid& *
id+ -
,- .
CancellationToken/ @
cancellationTokenA R
=S T
defaultU \
)\ ]
;] ^
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
PagingTS 
> 
> 
FindByIdsAsync +
(+ ,
Guid, 0
[0 1
]1 2
ids3 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
} 
} Ú

èD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IPagedList.cs
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
$str O
,O P
VersionQ X
=Y Z
$str[ `
)` a
]a b
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
public 

	interface 

IPagedList 
<  
T  !
>! "
:# $
IList% *
<* +
T+ ,
>, -
{ 
int 

TotalCount 
{ 
get 
; 
} 
int 
	PageCount 
{ 
get 
; 
} 
int 
PageNo 
{ 
get 
; 
} 
int 
PageSize 
{ 
get 
; 
} 
} 
} ç
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IOrderRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IOrderRepository %
:& '
IEFRepository( 5
<5 6
Order6 ;
,; <
Order= B
>B C
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Order 
? 
> 
FindByIdAsync "
(" #
Guid# '
id( *
,* +
CancellationToken, =
cancellationToken> O
=P Q
defaultR Y
)Y Z
;Z [
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Order 
> 
> 
FindByIdsAsync (
(( )
Guid) -
[- .
]. /
ids0 3
,3 4
CancellationToken5 F
cancellationTokenG X
=Y Z
default[ b
)b c
;c d
} 
} Î
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\Indexing\IFilteredIndexRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E
IndexingE M
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface $
IFilteredIndexRepository -
:. /
IEFRepository0 =
<= >
FilteredIndex> K
,K L
FilteredIndexM Z
>Z [
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
FilteredIndex 
? 
> 
FindByIdAsync *
(* +
Guid+ /
id0 2
,2 3
CancellationToken4 E
cancellationTokenF W
=X Y
defaultZ a
)a b
;b c
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
FilteredIndex 
>  
>  !
FindByIdsAsync" 0
(0 1
Guid1 5
[5 6
]6 7
ids8 ;
,; <
CancellationToken= N
cancellationTokenO `
=a b
defaultc j
)j k
;k l
} 
} ’
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IFuneralCoverQuoteRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface (
IFuneralCoverQuoteRepository 1
:2 3
IEFRepository4 A
<A B
FuneralCoverQuoteB S
,S T
FuneralCoverQuoteU f
>f g
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
FuneralCoverQuote 
? 
>  
FindByIdAsync! .
(. /
Guid/ 3
id4 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
FuneralCoverQuote #
># $
>$ %
FindByIdsAsync& 4
(4 5
Guid5 9
[9 :
]: ;
ids< ?
,? @
CancellationTokenA R
cancellationTokenS d
=e f
defaultg n
)n o
;o p
} 
} ü
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IOptionalRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IOptionalRepository (
:) *
IEFRepository+ 8
<8 9
Optional9 A
,A B
OptionalC K
>K L
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Optional 
? 
> 
FindByIdAsync %
(% &
Guid& *
id+ -
,- .
CancellationToken/ @
cancellationTokenA R
=S T
defaultU \
)\ ]
;] ^
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Optional 
> 
> 
FindByIdsAsync +
(+ ,
Guid, 0
[0 1
]1 2
ids3 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
} 
} ´
öD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IFileUploadRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface !
IFileUploadRepository *
:+ ,
IEFRepository- :
<: ;

FileUpload; E
,E F

FileUploadG Q
>Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 

FileUpload 
? 
> 
FindByIdAsync '
(' (
Guid( ,
id- /
,/ 0
CancellationToken1 B
cancellationTokenC T
=U V
defaultW ^
)^ _
;_ `
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 

FileUpload 
> 
> 
FindByIdsAsync -
(- .
Guid. 2
[2 3
]3 4
ids5 8
,8 9
CancellationToken: K
cancellationTokenL ]
=^ _
default` g
)g h
;h i
} 
} ü
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\ICustomerRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
ICustomerRepository (
:) *
IEFRepository+ 8
<8 9
Customer9 A
,A B
CustomerC K
>K L
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Customer 
? 
> 
FindByIdAsync %
(% &
Guid& *
id+ -
,- .
CancellationToken/ @
cancellationTokenA R
=S T
defaultU \
)\ ]
;] ^
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Customer 
> 
> 
FindByIdsAsync +
(+ ,
Guid, 0
[0 1
]1 2
ids3 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
} 
} ÷ê
íD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IEFRepository.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str Y
,Y Z
Version[ b
=c d
$stre j
)j k
]k l
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
public 

	interface 
IEFRepository "
<" #
TDomain# *
,* +
TPersistence, 8
>8 9
:: ;
IRepository< G
<G H
TDomainH O
>O P
{ 
IUnitOfWork 

UnitOfWork 
{  
get! $
;$ %
}& '
Task 
< 
TDomain 
? 
> 
	FindAsync  
(  !

Expression! +
<+ ,
Func, 0
<0 1
TPersistence1 =
,= >
bool? C
>C D
>D E
filterExpressionF V
,V W
CancellationTokenX i
cancellationTokenj {
=| }
default	~ Ö
)
Ö Ü
;
Ü á
Task 
< 
TDomain 
? 
> 
	FindAsync  
(  !

Expression! +
<+ ,
Func, 0
<0 1
TPersistence1 =
,= >
bool? C
>C D
>D E
filterExpressionF V
,V W
FuncX \
<\ ]

IQueryable] g
<g h
TPersistenceh t
>t u
,u v

IQueryable	w Å
<
Å Ç
TPersistence
Ç é
>
é è
>
è ê
queryOptions
ë ù
,
ù û
CancellationToken
ü ∞
cancellationToken
± ¬
=
√ ƒ
default
≈ Ã
)
Ã Õ
;
Õ Œ
Task 
< 
TDomain 
? 
> 
	FindAsync  
(  !
Func! %
<% &

IQueryable& 0
<0 1
TPersistence1 =
>= >
,> ?

IQueryable@ J
<J K
TPersistenceK W
>W X
>X Y
queryOptionsZ f
,f g
CancellationTokenh y
cancellationToken	z ã
=
å ç
default
é ï
)
ï ñ
;
ñ ó
Task 
< 
List 
< 
TDomain 
> 
> 
FindAllAsync (
(( )
CancellationToken) :
cancellationToken; L
=M N
defaultO V
)V W
;W X
Task 
< 
List 
< 
TDomain 
> 
> 
FindAllAsync (
(( )

Expression) 3
<3 4
Func4 8
<8 9
TPersistence9 E
,E F
boolG K
>K L
>L M
filterExpressionN ^
,^ _
CancellationToken` q
cancellationToken	r É
=
Ñ Ö
default
Ü ç
)
ç é
;
é è
Task 
< 
List 
< 
TDomain 
> 
> 
FindAllAsync (
(( )

Expression) 3
<3 4
Func4 8
<8 9
TPersistence9 E
,E F
boolG K
>K L
>L M
filterExpressionN ^
,^ _
Func` d
<d e

IQueryablee o
<o p
TPersistencep |
>| }
,} ~

IQueryable	 â
<
â ä
TPersistence
ä ñ
>
ñ ó
>
ó ò
queryOptions
ô •
,
• ¶
CancellationToken
ß ∏
cancellationToken
π  
=
À Ã
default
Õ ‘
)
‘ ’
;
’ ÷
Task 
< 

IPagedList 
< 
TDomain 
>  
>  !
FindAllAsync" .
(. /
int/ 2
pageNo3 9
,9 :
int; >
pageSize? G
,G H
CancellationTokenI Z
cancellationToken[ l
=m n
defaulto v
)v w
;w x
Task 
< 

IPagedList 
< 
TDomain 
>  
>  !
FindAllAsync" .
(. /

Expression/ 9
<9 :
Func: >
<> ?
TPersistence? K
,K L
boolM Q
>Q R
>R S
filterExpressionT d
,d e
intf i
pageNoj p
,p q
intr u
pageSizev ~
,~ 
CancellationToken
Ä ë
cancellationToken
í £
=
§ •
default
¶ ≠
)
≠ Æ
;
Æ Ø
Task 
< 

IPagedList 
< 
TDomain 
>  
>  !
FindAllAsync" .
(. /

Expression/ 9
<9 :
Func: >
<> ?
TPersistence? K
,K L
boolM Q
>Q R
>R S
filterExpressionT d
,d e
intf i
pageNoj p
,p q
intr u
pageSizev ~
,~ 
Func
Ä Ñ
<
Ñ Ö

IQueryable
Ö è
<
è ê
TPersistence
ê ú
>
ú ù
,
ù û

IQueryable
ü ©
<
© ™
TPersistence
™ ∂
>
∂ ∑
>
∑ ∏
queryOptions
π ≈
,
≈ ∆
CancellationToken
« ÿ
cancellationToken
Ÿ Í
=
Î Ï
default
Ì Ù
)
Ù ı
;
ı ˆ
Task 
< 
List 
< 
TDomain 
> 
> 
FindAllAsync (
(( )
Func) -
<- .

IQueryable. 8
<8 9
TPersistence9 E
>E F
,F G

IQueryableH R
<R S
TPersistenceS _
>_ `
>` a
queryOptionsb n
,n o
CancellationToken	p Å
cancellationToken
Ç ì
=
î ï
default
ñ ù
)
ù û
;
û ü
Task 
< 

IPagedList 
< 
TDomain 
>  
>  !
FindAllAsync" .
(. /
int/ 2
pageNo3 9
,9 :
int; >
pageSize? G
,G H
FuncI M
<M N

IQueryableN X
<X Y
TPersistenceY e
>e f
,f g

IQueryableh r
<r s
TPersistences 
>	 Ä
>
Ä Å
queryOptions
Ç é
,
é è
CancellationToken
ê °
cancellationToken
¢ ≥
=
¥ µ
default
∂ Ω
)
Ω æ
;
æ ø
Task 
< 
int 
> 

CountAsync 
( 

Expression '
<' (
Func( ,
<, -
TPersistence- 9
,9 :
bool; ?
>? @
>@ A
filterExpressionB R
,R S
CancellationTokenT e
cancellationTokenf w
=x y
default	z Å
)
Å Ç
;
Ç É
Task 
< 
int 
> 

CountAsync 
( 
Func !
<! "

IQueryable" ,
<, -
TPersistence- 9
>9 :
,: ;

IQueryable< F
<F G
TPersistenceG S
>S T
>T U
?U V
queryOptionsW c
=d e
defaultf m
,m n
CancellationToken	o Ä
cancellationToken
Å í
=
ì î
default
ï ú
)
ú ù
;
ù û
Task   
<   
bool   
>   
AnyAsync   
(   

Expression   &
<  & '
Func  ' +
<  + ,
TPersistence  , 8
,  8 9
bool  : >
>  > ?
>  ? @
filterExpression  A Q
,  Q R
CancellationToken  S d
cancellationToken  e v
=  w x
default	  y Ä
)
  Ä Å
;
  Å Ç
Task!! 
<!! 
bool!! 
>!! 
AnyAsync!! 
(!! 
Func!!  
<!!  !

IQueryable!!! +
<!!+ ,
TPersistence!!, 8
>!!8 9
,!!9 :

IQueryable!!; E
<!!E F
TPersistence!!F R
>!!R S
>!!S T
?!!T U
queryOptions!!V b
=!!c d
default!!e l
,!!l m
CancellationToken!!n 
cancellationToken
!!Ä ë
=
!!í ì
default
!!î õ
)
!!õ ú
;
!!ú ù
Task"" 
<"" 
List"" 
<"" 
TProjection"" 
>"" 
>"" !
FindAllProjectToAsync""  5
<""5 6
TProjection""6 A
>""A B
(""B C
CancellationToken""C T
cancellationToken""U f
=""g h
default""i p
)""p q
;""q r
Task## 
<## 
List## 
<## 
TProjection## 
>## 
>## !
FindAllProjectToAsync##  5
<##5 6
TProjection##6 A
>##A B
(##B C
Func##C G
<##G H

IQueryable##H R
<##R S
TPersistence##S _
>##_ `
,##` a

IQueryable##b l
<##l m
TPersistence##m y
>##y z
>##z {
queryOptions	##| à
,
##à â
CancellationToken
##ä õ
cancellationToken
##ú ≠
=
##Æ Ø
default
##∞ ∑
)
##∑ ∏
;
##∏ π
Task$$ 
<$$ 
List$$ 
<$$ 
TProjection$$ 
>$$ 
>$$ !
FindAllProjectToAsync$$  5
<$$5 6
TProjection$$6 A
>$$A B
($$B C

Expression$$C M
<$$M N
Func$$N R
<$$R S
TPersistence$$S _
,$$_ `
bool$$a e
>$$e f
>$$f g
filterExpression$$h x
,$$x y
CancellationToken	$$z ã
cancellationToken
$$å ù
=
$$û ü
default
$$† ß
)
$$ß ®
;
$$® ©
Task%% 
<%% 
List%% 
<%% 
TProjection%% 
>%% 
>%% !
FindAllProjectToAsync%%  5
<%%5 6
TProjection%%6 A
>%%A B
(%%B C

Expression%%C M
<%%M N
Func%%N R
<%%R S
TPersistence%%S _
,%%_ `
bool%%a e
>%%e f
>%%f g
filterExpression%%h x
,%%x y
Func%%z ~
<%%~ 

IQueryable	%% â
<
%%â ä
TPersistence
%%ä ñ
>
%%ñ ó
,
%%ó ò

IQueryable
%%ô £
<
%%£ §
TPersistence
%%§ ∞
>
%%∞ ±
>
%%± ≤
queryOptions
%%≥ ø
,
%%ø ¿
CancellationToken
%%¡ “
cancellationToken
%%” ‰
=
%%Â Ê
default
%%Á Ó
)
%%Ó Ô
;
%%Ô 
Task&& 
<&& 

IPagedList&& 
<&& 
TProjection&& #
>&&# $
>&&$ %!
FindAllProjectToAsync&&& ;
<&&; <
TProjection&&< G
>&&G H
(&&H I
int&&I L
pageNo&&M S
,&&S T
int&&U X
pageSize&&Y a
,&&a b
CancellationToken&&c t
cancellationToken	&&u Ü
=
&&á à
default
&&â ê
)
&&ê ë
;
&&ë í
Task'' 
<'' 

IPagedList'' 
<'' 
TProjection'' #
>''# $
>''$ %!
FindAllProjectToAsync''& ;
<''; <
TProjection''< G
>''G H
(''H I
int''I L
pageNo''M S
,''S T
int''U X
pageSize''Y a
,''a b
Func''c g
<''g h

IQueryable''h r
<''r s
TPersistence''s 
>	'' Ä
,
''Ä Å

IQueryable
''Ç å
<
''å ç
TPersistence
''ç ô
>
''ô ö
>
''ö õ
queryOptions
''ú ®
,
''® ©
CancellationToken
''™ ª
cancellationToken
''º Õ
=
''Œ œ
default
''– ◊
)
''◊ ÿ
;
''ÿ Ÿ
Task(( 
<(( 

IPagedList(( 
<(( 
TProjection(( #
>((# $
>(($ %!
FindAllProjectToAsync((& ;
<((; <
TProjection((< G
>((G H
(((H I

Expression((I S
<((S T
Func((T X
<((X Y
TPersistence((Y e
,((e f
bool((g k
>((k l
>((l m
filterExpression((n ~
,((~ 
int
((Ä É
pageNo
((Ñ ä
,
((ä ã
int
((å è
pageSize
((ê ò
,
((ò ô
CancellationToken
((ö ´
cancellationToken
((¨ Ω
=
((æ ø
default
((¿ «
)
((« »
;
((» …
Task)) 
<)) 

IPagedList)) 
<)) 
TProjection)) #
>))# $
>))$ %!
FindAllProjectToAsync))& ;
<)); <
TProjection))< G
>))G H
())H I

Expression))I S
<))S T
Func))T X
<))X Y
TPersistence))Y e
,))e f
bool))g k
>))k l
>))l m
filterExpression))n ~
,))~ 
int
))Ä É
pageNo
))Ñ ä
,
))ä ã
int
))å è
pageSize
))ê ò
,
))ò ô
Func
))ö û
<
))û ü

IQueryable
))ü ©
<
))© ™
TPersistence
))™ ∂
>
))∂ ∑
,
))∑ ∏

IQueryable
))π √
<
))√ ƒ
TPersistence
))ƒ –
>
))– —
>
))— “
queryOptions
))” ﬂ
,
))ﬂ ‡
CancellationToken
))· Ú
cancellationToken
))Û Ñ
=
))Ö Ü
default
))á é
)
))é è
;
))è ê
Task** 
<** 
List** 
<** 
TProjection** 
>** 
>** !
FindAllProjectToAsync**  5
<**5 6
TProjection**6 A
>**A B
(**B C

Expression**C M
<**M N
Func**N R
<**R S
TPersistence**S _
,**_ `
bool**a e
>**e f
>**f g
?**g h
filterExpression**i y
,**y z
Func**{ 
<	** Ä

IQueryable
**Ä ä
<
**ä ã
TProjection
**ã ñ
>
**ñ ó
,
**ó ò

IQueryable
**ô £
>
**£ §
filterProjection
**• µ
,
**µ ∂
CancellationToken
**∑ »
cancellationToken
**… ⁄
=
**€ ‹
default
**› ‰
)
**‰ Â
;
**Â Ê
Task++ 
<++ 
TProjection++ 
?++ 
>++ 
FindProjectToAsync++ -
<++- .
TProjection++. 9
>++9 :
(++: ;

Expression++; E
<++E F
Func++F J
<++J K
TPersistence++K W
,++W X
bool++Y ]
>++] ^
>++^ _
filterExpression++` p
,++p q
CancellationToken	++r É
cancellationToken
++Ñ ï
=
++ñ ó
default
++ò ü
)
++ü †
;
++† °
Task,, 
<,, 
TProjection,, 
?,, 
>,, 
FindProjectToAsync,, -
<,,- .
TProjection,,. 9
>,,9 :
(,,: ;

Expression,,; E
<,,E F
Func,,F J
<,,J K
TPersistence,,K W
,,,W X
bool,,Y ]
>,,] ^
>,,^ _
filterExpression,,` p
,,,p q
Func,,r v
<,,v w

IQueryable	,,w Å
<
,,Å Ç
TPersistence
,,Ç é
>
,,é è
,
,,è ê

IQueryable
,,ë õ
<
,,õ ú
TPersistence
,,ú ®
>
,,® ©
>
,,© ™
queryOptions
,,´ ∑
,
,,∑ ∏
CancellationToken
,,π  
cancellationToken
,,À ‹
=
,,› ﬁ
default
,,ﬂ Ê
)
,,Ê Á
;
,,Á Ë
Task-- 
<-- 
TProjection-- 
?-- 
>-- 
FindProjectToAsync-- -
<--- .
TProjection--. 9
>--9 :
(--: ;
Func--; ?
<--? @

IQueryable--@ J
<--J K
TPersistence--K W
>--W X
,--X Y

IQueryable--Z d
<--d e
TPersistence--e q
>--q r
>--r s
queryOptions	--t Ä
,
--Ä Å
CancellationToken
--Ç ì
cancellationToken
--î •
=
--¶ ß
default
--® Ø
)
--Ø ∞
;
--∞ ±
Task.. 
<.. 
IEnumerable.. 
>.. 3
'FindAllProjectToWithTransformationAsync.. A
<..A B
TProjection..B M
>..M N
(..N O

Expression..O Y
<..Y Z
Func..Z ^
<..^ _
TPersistence.._ k
,..k l
bool..m q
>..q r
>..r s
?..s t
filterExpression	..u Ö
,
..Ö Ü
Func
..á ã
<
..ã å

IQueryable
..å ñ
<
..ñ ó
TProjection
..ó ¢
>
..¢ £
,
..£ §

IQueryable
..• Ø
>
..Ø ∞
	transform
..± ∫
,
..∫ ª
CancellationToken
..º Õ
cancellationToken
..Œ ﬂ
=
..‡ ·
default
..‚ È
)
..È Í
;
..Í Î
}// 
}00 é
™D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\ICorporateFuneralCoverQuoteRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 1
%ICorporateFuneralCoverQuoteRepository :
:; <
IEFRepository= J
<J K&
CorporateFuneralCoverQuoteK e
,e f'
CorporateFuneralCoverQuote	g Å
>
Å Ç
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< &
CorporateFuneralCoverQuote '
?' (
>( )
FindByIdAsync* 7
(7 8
Guid8 <
id= ?
,? @
CancellationTokenA R
cancellationTokenS d
=e f
defaultg n
)n o
;o p
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< &
CorporateFuneralCoverQuote ,
>, -
>- .
FindByIdsAsync/ =
(= >
Guid> B
[B C
]C D
idsE H
,H I
CancellationTokenJ [
cancellationToken\ m
=n o
defaultp w
)w x
;x y
} 
} ü
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IContractRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IContractRepository (
:) *
IEFRepository+ 8
<8 9
Contract9 A
,A B
ContractC K
>K L
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Contract 
? 
> 
FindByIdAsync %
(% &
Guid& *
id+ -
,- .
CancellationToken/ @
cancellationTokenA R
=S T
defaultU \
)\ ]
;] ^
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Contract 
> 
> 
FindByIdsAsync +
(+ ,
Guid, 0
[0 1
]1 2
ids3 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
} 
} ç
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\IBasicRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface 
IBasicRepository %
:& '
IEFRepository( 5
<5 6
Basic6 ;
,; <
Basic= B
>B C
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
Basic 
? 
> 
FindByIdAsync "
(" #
Guid# '
id( *
,* +
CancellationToken, =
cancellationToken> O
=P Q
defaultR Y
)Y Z
;Z [
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
Basic 
> 
> 
FindByIdsAsync (
(( )
Guid) -
[- .
]. /
ids0 3
,3 4
CancellationToken5 F
cancellationTokenG X
=Y Z
default[ b
)b c
;c d
} 
} ï
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\ExtensiveDomainServices\IConcreteEntityBRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E#
ExtensiveDomainServicesE \
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface &
IConcreteEntityBRepository /
:0 1
IEFRepository2 ?
<? @
ConcreteEntityB@ O
,O P
ConcreteEntityBQ `
>` a
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
ConcreteEntityB 
? 
> 
FindByIdAsync ,
(, -
Guid- 1
id2 4
,4 5
CancellationToken6 G
cancellationTokenH Y
=Z [
default\ c
)c d
;d e
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
ConcreteEntityB !
>! "
>" #
FindByIdsAsync$ 2
(2 3
Guid3 7
[7 8
]8 9
ids: =
,= >
CancellationToken? P
cancellationTokenQ b
=c d
defaulte l
)l m
;m n
} 
} ï
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\ExtensiveDomainServices\IConcreteEntityARepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E#
ExtensiveDomainServicesE \
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface &
IConcreteEntityARepository /
:0 1
IEFRepository2 ?
<? @
ConcreteEntityA@ O
,O P
ConcreteEntityAQ `
>` a
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
ConcreteEntityA 
? 
> 
FindByIdAsync ,
(, -
Guid- 1
id2 4
,4 5
CancellationToken6 G
cancellationTokenH Y
=Z [
default\ c
)c d
;d e
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
ConcreteEntityA !
>! "
>" #
FindByIdsAsync$ 2
(2 3
Guid3 7
[7 8
]8 9
ids: =
,= >
CancellationToken? P
cancellationTokenQ b
=c d
defaulte l
)l m
;m n
} 
} ˝
≥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\ExtensiveDomainServices\IBaseEntityBRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E#
ExtensiveDomainServicesE \
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface "
IBaseEntityBRepository +
:, -
IEFRepository. ;
<; <
BaseEntityB< G
,G H
BaseEntityBI T
>T U
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
BaseEntityB 
? 
> 
FindByIdAsync (
(( )
Guid) -
id. 0
,0 1
CancellationToken2 C
cancellationTokenD U
=V W
defaultX _
)_ `
;` a
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
BaseEntityB 
> 
> 
FindByIdsAsync  .
(. /
Guid/ 3
[3 4
]4 5
ids6 9
,9 :
CancellationToken; L
cancellationTokenM ^
=_ `
defaulta h
)h i
;i j
} 
} ˝
≥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\ExtensiveDomainServices\IBaseEntityARepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E#
ExtensiveDomainServicesE \
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface "
IBaseEntityARepository +
:, -
IEFRepository. ;
<; <
BaseEntityA< G
,G H
BaseEntityAI T
>T U
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
BaseEntityA 
? 
> 
FindByIdAsync (
(( )
Guid) -
id. 0
,0 1
CancellationToken2 C
cancellationTokenD U
=V W
defaultX _
)_ `
;` a
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
BaseEntityA 
> 
> 
FindByIdsAsync  .
(. /
Guid/ 3
[3 4
]4 5
ids6 9
,9 :
CancellationToken; L
cancellationTokenM ^
=_ `
defaulta h
)h i
;i j
} 
} è
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\DomainServices\IDomainServiceTestRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E
DomainServicesE S
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface (
IDomainServiceTestRepository 1
:2 3
IEFRepository4 A
<A B
DomainServiceTestB S
,S T
DomainServiceTestU f
>f g
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
DomainServiceTest 
? 
>  
FindByIdAsync! .
(. /
Guid/ 3
id4 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< 
DomainServiceTest #
># $
>$ %
FindByIdsAsync& 4
(4 5
Guid5 9
[9 :
]: ;
ids< ?
,? @
CancellationTokenA R
cancellationTokenS d
=e f
defaultg n
)n o
;o p
} 
} π
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\DomainServices\IClassicDomainServiceTestRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E
DomainServicesE S
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface /
#IClassicDomainServiceTestRepository 8
:9 :
IEFRepository; H
<H I$
ClassicDomainServiceTestI a
,a b$
ClassicDomainServiceTestc {
>{ |
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< $
ClassicDomainServiceTest %
?% &
>& '
FindByIdAsync( 5
(5 6
Guid6 :
id; =
,= >
CancellationToken? P
cancellationTokenQ b
=c d
defaulte l
)l m
;m n
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< $
ClassicDomainServiceTest *
>* +
>+ ,
FindByIdsAsync- ;
(; <
Guid< @
[@ A
]A B
idsC F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
} 
} °
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Repositories\AnemicChild\IParentWithAnemicChildRepository.cs
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
$str		 V
,		V W
Version		X _
=		` a
$str		b g
)		g h
]		h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Repositories8 D
.D E
AnemicChildE P
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

	interface ,
 IParentWithAnemicChildRepository 5
:6 7
IEFRepository8 E
<E F!
ParentWithAnemicChildF [
,[ \!
ParentWithAnemicChild] r
>r s
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
TProjection 
? 
> "
FindByIdProjectToAsync 1
<1 2
TProjection2 =
>= >
(> ?
Guid? C
idD F
,F G
CancellationTokenH Y
cancellationTokenZ k
=l m
defaultn u
)u v
;v w
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< !
ParentWithAnemicChild "
?" #
># $
FindByIdAsync% 2
(2 3
Guid3 7
id8 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
;j k
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
Task 
< 
List 
< !
ParentWithAnemicChild '
>' (
>( )
FindByIdsAsync* 8
(8 9
Guid9 =
[= >
]> ?
ids@ C
,C D
CancellationTokenE V
cancellationTokenW h
=i j
defaultk r
)r s
;s t
} 
} †
ÉD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\OrderStatus.cs
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
$str 6
,6 7
Version8 ?
=@ A
$strB G
)G H
]H I
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
{ 
public 

enum 
OrderStatus 
{		 
Paid

 
=

 
$num

 
,

 
Shipped 
= 
$num 
} 
} î
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\ExtensiveDomainServices\SimpleVO.cs
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
$str ;
,; <
Version= D
=E F
$strG L
)L M
]M N
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8#
ExtensiveDomainServices8 O
{ 
public		 

class		 
SimpleVO		 
:		 
ValueObject		 '
{

 
	protected 
SimpleVO 
( 
) 
{ 	
} 	
public 
SimpleVO 
( 
string 
value1 %
,% &
int' *
value2+ 1
)1 2
{ 	
Value1 
= 
value1 
; 
Value2 
= 
value2 
; 
} 	
public 
string 
Value1 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 
int 
Value2 
{ 
get 
;  
private! (
set) ,
;, -
}. /
	protected 
override 
IEnumerable &
<& '
object' -
>- .!
GetEqualityComponents/ D
(D E
)E F
{ 	
yield 
return 
Value1 
;  
yield 
return 
Value2 
;  
} 	
} 
} “	
éD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Events\NewQuoteCreated.cs
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
$str ;
,; <
Version= D
=E F
$strG L
)L M
]M N
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
0 1
Domain

1 7
.

7 8
Events

8 >
{ 
public 

class 
NewQuoteCreated  
:! "
DomainEvent# .
{ 
public 
NewQuoteCreated 
( 
Quote $
quote% *
)* +
{ 	
Quote 
= 
quote 
; 
} 	
public 
Quote 
Quote 
{ 
get  
;  !
}" #
} 
} «
äD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Warehouse.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
	Warehouse

 
:

 
IHasDomainEvent

 ,
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
Size 
{ 
get 
; 
set "
;" #
}$ %
public 
Address 
? 
Address 
{  !
get" %
;% &
set' *
;* +
}, -
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ˆ
ìD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\UserDefaultAddress.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class 
UserDefaultAddress #
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
} 
} ≤
åD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\UserAddress.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class 
UserAddress 
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Postal 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} Í
ÖD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\User.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public		 

class		 
User		 
:		 
Person		 
{

 
public 
User 
( 
string 
name 
,  
string! '
surname( /
,/ 0
string1 7
email8 =
,= >
Guid? C
quoteIdD K
)K L
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
Email 
= 
email 
; 
QuoteId 
= 
quoteId 
; 
} 	
	protected 
User 
( 
) 
{ 	
Email 
= 
null 
! 
; 
Quote 
= 
null 
! 
; "
DefaultDeliveryAddress "
=# $
null% )
!) *
;* +
} 	
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
QuoteId 
{ 
get !
;! "
set# &
;& '
}( )
public   
virtual   
Quote   
Quote   "
{  # $
get  % (
;  ( )
set  * -
;  - .
}  / 0
public"" 
virtual"" 
ICollection"" "
<""" #
UserAddress""# .
>"". /
	Addresses""0 9
{"": ;
get""< ?
;""? @
set""A D
;""D E
}""F G
=""H I
[""J K
]""K L
;""L M
public$$ 
virtual$$ 
UserDefaultAddress$$ )"
DefaultDeliveryAddress$$* @
{$$A B
get$$C F
;$$F G
set$$H K
;$$K L
}$$M N
public&& 
virtual&& 
UserDefaultAddress&& )
?&&) *!
DefaultBillingAddress&&+ @
{&&A B
get&&C F
;&&F G
set&&H K
;&&K L
}&&M N
public(( 
void(( 

AddAddress(( 
((( 
IEnumerable(( *
<((* +
UserAddress((+ 6
>((6 7
	addresses((8 A
)((A B
{)) 	
	Addresses** 
.** 
Clear** 
(** 
)** 
;** 
foreach,, 
(,, 
var,, 
item,, 
in,,  
	addresses,,! *
),,* +
{-- 
	Addresses.. 
... 
Add.. 
(.. 
item.. "
).." #
;..# $
}// 
}00 	
}11 
}22 Â
äD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\QuoteLine.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class 
	QuoteLine 
{		 
public

 
	QuoteLine

 
(

 
Guid

 
	productId

 '
)

' (
{ 	
	ProductId 
= 
	productId !
;! "
} 	
	protected 
	QuoteLine 
( 
) 
{ 	
Product 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
Guid 
QuoteId 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
Units 
{ 
get 
; 
set  #
;# $
}% &
public 
decimal 
	UnitPrice  
{! "
get# &
;& '
set( +
;+ ,
}- .
public   
virtual   
Product   
Product   &
{  ' (
get  ) ,
;  , -
set  . 1
;  1 2
}  3 4
}!! 
}"" „
ÜD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Quote.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
.		7 8
Entities		8 @
{

 
public 

class 
Quote 
: 
IHasDomainEvent (
{ 
public 
Quote 
( 
string 
refNo !
,! "
Guid# '
personId( 0
,0 1
string2 8
?8 9
personEmail: E
)E F
{ 	
RefNo 
= 
refNo 
; 
PersonId 
= 
personId 
;  
PersonEmail 
= 
personEmail %
;% &
DomainEvents 
. 
Add 
( 
new  
NewQuoteCreated! 0
(0 1
quote1 6
:6 7
this8 <
)< =
)= >
;> ?
} 	
	protected 
Quote 
( 
) 
{ 	
RefNo 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public!! 
Guid!! 
PersonId!! 
{!! 
get!! "
;!!" #
set!!$ '
;!!' (
}!!) *
public## 
string## 
?## 
PersonEmail## "
{### $
get##% (
;##( )
set##* -
;##- .
}##/ 0
public%% 
virtual%% 
ICollection%% "
<%%" #
	QuoteLine%%# ,
>%%, -

QuoteLines%%. 8
{%%9 :
get%%; >
;%%> ?
set%%@ C
;%%C D
}%%E F
=%%G H
[%%I J
]%%J K
;%%K L
public'' 
List'' 
<'' 
DomainEvent'' 
>''  
DomainEvents''! -
{''. /
get''0 3
;''3 4
set''5 8
;''8 9
}'': ;
=''< =
[''> ?
]''? @
;''@ A
public)) 
void)) 
NotifyQuoteCreated)) &
())& '
)))' (
{** 	
DomainEvents++ 
.++ 
Add++ 
(++ 
new++  
NewQuoteCreated++! 0
(++0 1
quote++1 6
:++6 7
this++8 <
)++< =
)++= >
;++> ?
},, 	
}-- 
}.. ã
àD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Product.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Product

 
:

 
IHasDomainEvent

 *
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
ICollection 
< 
Tag 
> 
Tags  $
{% &
get' *
;* +
set, /
;/ 0
}1 2
=3 4
[5 6
]6 7
;7 8
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} Ï
åD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Preferences.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class 
Preferences 
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
bool 

Newsletter 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
bool 
Specials 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} ú
áD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Person.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Person

 
:

 
IHasDomainEvent

 )
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} Ö

âD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\PagingTS.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
PagingTS

 
:

 
IHasDomainEvent

 +
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ‰
äD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\OrderItem.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class 
	OrderItem 
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
int 
Quantity 
{ 
get !
;! "
set# &
;& '
}( )
public 
int 
Units 
{ 
get 
; 
set  #
;# $
}% &
public 
decimal 
	UnitPrice  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
virtual 
Product 
Product &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
} 
} ë
ÜD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Order.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Order

 
:

 
IHasDomainEvent

 (
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
	OrderDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
OrderStatus 
OrderStatus &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
Guid 

CustomerId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
virtual 
Customer 
Customer  (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
virtual 
ICollection "
<" #
	OrderItem# ,
>, -

OrderItems. 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
=G H
[I J
]J K
;K L
public 
Address 
DeliveryAddress &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
Address 
? 
BillingAddress &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
}   Ö

âD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Optional.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Optional

 
:

 
IHasDomainEvent

 +
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ˆ
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\MappingTests\NestingParent.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A
MappingTestsA M
{		 
public

 

class

 
NestingParent

 
:

  
IHasDomainEvent

! 0
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
virtual 
ICollection "
<" #
NestingChild# /
>/ 0
NestingChildren1 @
{A B
getC F
;F G
setH K
;K L
}M N
=O P
[Q R
]R S
;S T
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ê
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\MappingTests\NestingChildChild.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A
MappingTestsA M
{ 
public 

class 
NestingChildChild "
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
} 
} Û

öD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\MappingTests\NestingChild.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A
MappingTestsA M
{ 
public 

class 
NestingChild 
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
Guid 
NestingParentId #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
virtual 
NestingChildChild (
NestingChildChild) :
{; <
get= @
;@ A
setB E
;E F
}G H
} 
} ◊
óD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Indexing\FilteredIndex.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A
IndexingA I
{		 
public

 

class

 
FilteredIndex

 
:

  
IHasDomainEvent

! 0
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ”
íD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\FuneralCoverQuote.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class 
FuneralCoverQuote "
:# $
Quote% *
{		 
public

 
FuneralCoverQuote

  
(

  !
string

! '
refNo

( -
,

- .
Guid

/ 3
personId

4 <
,

< =
string

> D
?

D E
personEmail

F Q
)

Q R
{ 	
RefNo 
= 
refNo 
; 
PersonId 
= 
personId 
;  
PersonEmail 
= 
personEmail %
;% &
} 	
	protected 
FuneralCoverQuote #
(# $
)$ %
{ 	
} 	
public 
decimal 
Amount 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} „
ãD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\FileUpload.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 

FileUpload

 
:

 
IHasDomainEvent

 -
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Filename 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
byte 
[ 
] 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
ContentType !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ˜
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\ExtensiveDomainServices\ConcreteEntityB.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A#
ExtensiveDomainServicesA X
{ 
public 

class 
ConcreteEntityB  
:! "
BaseEntityB# .
{ 
public		 
ConcreteEntityB		 
(		 
string		 %
baseAttr		& .
,		. /
string		0 6
concreteAttr		7 C
)		C D
{

 	
BaseAttr 
= 
baseAttr 
;  
ConcreteAttr 
= 
concreteAttr '
;' (
} 	
	protected 
ConcreteEntityB !
(! "
)" #
{ 	
ConcreteAttr 
= 
null 
!  
;  !
} 	
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} æ
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\ExtensiveDomainServices\ConcreteEntityA.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A#
ExtensiveDomainServicesA X
{ 
public 

class 
ConcreteEntityA  
:! "
BaseEntityA# .
{ 
public		 
string		 
ConcreteAttr		 "
{		# $
get		% (
;		( )
set		* -
;		- .
}		/ 0
}

 
} ˘
§D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\ExtensiveDomainServices\BaseEntityB.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A#
ExtensiveDomainServicesA X
{		 
public

 

class

 
BaseEntityB

 
:

 
IHasDomainEvent

 .
{ 
public 
BaseEntityB 
( 
string !
baseAttr" *
)* +
{ 	
BaseAttr 
= 
baseAttr 
;  
} 	
	protected 
BaseEntityB 
( 
) 
{ 	
BaseAttr 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} €

§D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\ExtensiveDomainServices\BaseEntityA.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A#
ExtensiveDomainServicesA X
{		 
public

 

class

 
BaseEntityA

 
:

 
IHasDomainEvent

 .
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ¸
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\DomainServices\DomainServiceTest.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
.		7 8
Entities		8 @
.		@ A
DomainServices		A O
{

 
public 

class 
DomainServiceTest "
:# $
IHasDomainEvent% 4
{ 
public 
DomainServiceTest  
(  !
IMyDomainService! 1
service2 9
)9 :
{ 	
} 	
	protected 
DomainServiceTest #
(# $
)$ %
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
public 
void 
MyOp 
( 
IMyDomainService )
service* 1
)1 2
{ 	
throw 
new #
NotImplementedException -
(- .
$str. S
)S T
;T U
}   	
}!! 
}"" ù
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\DomainServices\ClassicDomainServiceTest.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
.		7 8
Entities		8 @
.		@ A
DomainServices		A O
{

 
public 

class $
ClassicDomainServiceTest )
:* +
IHasDomainEvent, ;
{ 
public $
ClassicDomainServiceTest '
(' (
IMyDomainService( 8
service9 @
)@ A
{ 	
} 	
	protected $
ClassicDomainServiceTest *
(* +
)+ ,
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
public 
void 
	ClassicOp 
( 
IMyDomainService .
service/ 6
)6 7
{ 	
throw 
new #
NotImplementedException -
(- .
$str. S
)S T
;T U
}   	
}!! 
}"" Ç
âD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Customer.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Customer

 
:

 
IHasDomainEvent

 +
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
virtual 
Preferences "
?" #
Preferences$ /
{0 1
get2 5
;5 6
set7 :
;: ;
}< =
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ∑
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\CorporateFuneralCoverQuote.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{ 
public 

class &
CorporateFuneralCoverQuote +
:, -
FuneralCoverQuote. ?
{		 
public

 &
CorporateFuneralCoverQuote

 )
(

) *
string

* 0
refNo

1 6
,

6 7
Guid

8 <
personId

= E
,

E F
string

G M
?

M N
personEmail

O Z
)

Z [
{ 	
base 
. 
RefNo 
= 
refNo 
; 
base 
. 
PersonId 
= 
personId $
;$ %
base 
. 
PersonEmail 
= 
personEmail *
;* +
} 	
	protected &
CorporateFuneralCoverQuote ,
(, -
)- .
{ 	
	Corporate 
= 
null 
! 
; 
Registration 
= 
null 
!  
;  !
} 	
public 
string 
	Corporate 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
Registration "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} ü
âD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Contract.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Contract

 
:

 
IHasDomainEvent

 +
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ö
ÜD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\Basic.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
{		 
public

 

class

 
Basic

 
:

 
IHasDomainEvent

 (
{ 
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
List 
< 
DomainEvent 
>  
DomainEvents! -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
=< =
[> ?
]? @
;@ A
} 
} ‹
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\AnemicChild\ParentWithAnemicChild.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A
AnemicChildA L
{		 
public

 

class

 !
ParentWithAnemicChild

 &
:

' (
IHasDomainEvent

) 8
{ 
public !
ParentWithAnemicChild $
($ %
string% +
name, 0
,0 1
string2 8
surname9 @
,@ A
IEnumerableB M
<M N
AnemicChildN Y
>Y Z
anemicChildren[ i
)i j
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
AnemicChildren 
= 
new  
List! %
<% &
AnemicChild& 1
>1 2
(2 3
anemicChildren3 A
)A B
;B C
} 	
	protected !
ParentWithAnemicChild '
(' (
)( )
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public   
string   
Surname   
{   
get    #
;  # $
set  % (
;  ( )
}  * +
public"" 
virtual"" 
ICollection"" "
<""" #
AnemicChild""# .
>"". /
AnemicChildren""0 >
{""? @
get""A D
;""D E
set""F I
;""I J
}""K L
=""M N
[""O P
]""P Q
;""Q R
public$$ 
List$$ 
<$$ 
DomainEvent$$ 
>$$  
DomainEvents$$! -
{$$. /
get$$0 3
;$$3 4
set$$5 8
;$$8 9
}$$: ;
=$$< =
[$$> ?
]$$? @
;$$@ A
}%% 
}&& ›
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Entities\AnemicChild\AnemicChild.cs
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Entities8 @
.@ A
AnemicChildA L
{ 
public 

class 
AnemicChild 
{		 
public

 
Guid

 
Id

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
public 
Guid #
ParentWithAnemicChildId +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
} 
} ˆ
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Contracts\ExtensiveDomainServices\PassthroughObj.cs
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
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
	Contracts8 A
.A B#
ExtensiveDomainServicesB Y
{ 
public 

record 
PassthroughObj  
:! "
PassthroughBaseObj# 5
{		 
public

 
PassthroughObj

 
(

 
string

 $
baseAttr

% -
,

- .
string

/ 5
concreteAttr

6 B
)

B C
:

D E
base

F J
(

J K
baseAttr

K S
)

S T
{ 	
ConcreteAttr 
= 
concreteAttr '
;' (
} 	
public 
string 
ConcreteAttr "
{# $
get% (
;( )
init* .
;. /
}0 1
} 
} µ

¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Contracts\ExtensiveDomainServices\PassthroughBaseObj.cs
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
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
	Contracts8 A
.A B#
ExtensiveDomainServicesB Y
{ 
public 

record 
PassthroughBaseObj $
{		 
public

 
PassthroughBaseObj

 !
(

! "
string

" (
baseAttr

) 1
)

1 2
{ 	
BaseAttr 
= 
baseAttr 
;  
} 	
public 
string 
BaseAttr 
{  
get! $
;$ %
init& *
;* +
}, -
} 
} ≠
îD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Contracts\CustomerStatistics.cs
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
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
	Contracts8 A
{ 
public 

record 
CustomerStatistics $
{		 
public

 
CustomerStatistics

 !
(

! "
int

" %

noOfOrders

& 0
,

0 1
decimal

2 9
averageCartValue

: J
)

J K
{ 	

NoOfOrders 
= 

noOfOrders #
;# $
AverageCartValue 
= 
averageCartValue /
;/ 0
} 	
public 
int 

NoOfOrders 
{ 
get  #
;# $
init% )
;) *
}+ ,
public 
decimal 
AverageCartValue '
{( )
get* -
;- .
init/ 3
;3 4
}5 6
} 
} ¶
ãD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Common\UpdateHelper.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str 8
,8 9
Version: A
=B C
$strD I
)I J
]J K
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Common8 >
{		 
public 

static 
class 
UpdateHelper $
{ 
public 
static 
ICollection !
<! "
	TOriginal" +
>+ ,$
CreateOrUpdateCollection- E
<E F
TChangedF N
,N O
	TOriginalP Y
>Y Z
(Z [
ICollection 
< 
	TOriginal !
>! "
baseCollection# 1
,1 2
IEnumerable 
< 
TChanged  
>  !
?! "
changedCollection# 4
,4 5
Func   
<   
	TOriginal   
,   
TChanged   $
,  $ %
bool  & *
>  * +
equalityCheck  , 9
,  9 :
Func!! 
<!! 
	TOriginal!! 
?!! 
,!! 
TChanged!! %
,!!% &
	TOriginal!!' 0
>!!0 1
assignmentAction!!2 B
)!!B C
{"" 	
if## 
(## 
changedCollection## !
==##" $
null##% )
)##) *
{$$ 
return%% 
new%% 
List%% 
<%%  
	TOriginal%%  )
>%%) *
(%%* +
)%%+ ,
;%%, -
}&& 
baseCollection(( 
??=(( 
new(( "
List((# '
<((' (
	TOriginal((( 1
>((1 2
(((2 3
)((3 4
!((4 5
;((5 6
var** 
result** 
=** 
baseCollection** '
.**' (
CompareCollections**( :
(**: ;
changedCollection**; L
,**L M
equalityCheck**N [
)**[ \
;**\ ]
foreach++ 
(++ 
var++ 
elementToAdd++ %
in++& (
result++) /
.++/ 0
ToAdd++0 5
)++5 6
{,, 
var-- 
	newEntity-- 
=-- 
assignmentAction--  0
(--0 1
default--1 8
,--8 9
elementToAdd--: F
)--F G
;--G H
baseCollection// 
.// 
Add// "
(//" #
	newEntity//# ,
)//, -
;//- .
}00 
foreach22 
(22 
var22 
elementToRemove22 (
in22) +
result22, 2
.222 3
ToRemove223 ;
)22; <
{33 
baseCollection44 
.44 
Remove44 %
(44% &
elementToRemove44& 5
)445 6
;446 7
}55 
foreach77 
(77 
var77 
elementToEdit77 &
in77' )
result77* 0
.770 1
PossibleEdits771 >
)77> ?
{88 
assignmentAction99  
(99  !
elementToEdit99! .
.99. /
Original99/ 7
,997 8
elementToEdit999 F
.99F G
Changed99G N
)99N O
;99O P
}:: 
return<< 
baseCollection<< !
;<<! "
}== 	
}>> 
}?? ¥	
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Common\Interfaces\IUnitOfWork.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Common8 >
.> ?

Interfaces? I
{		 
public

 

	interface

 
IUnitOfWork

  
{ 
Task 
< 
int 
> 
SaveChangesAsync "
(" #
CancellationToken# 4
cancellationToken5 F
=G H
defaultI P
(P Q
CancellationTokenQ b
)b c
)c d
;d e
} 
} †
éD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Common\IHasDomainEvent.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str G
,G H
VersionI P
=Q R
$strS X
)X Y
]Y Z
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Common8 >
{		 
public

 

	interface

 
IHasDomainEvent

 $
{ 
List 
< 
DomainEvent 
> 
DomainEvents &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
} 
} Î
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Common\Exceptions\NotFoundException.cs
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
$str =
,= >
Version? F
=G H
$strI N
)N O
]O P
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Common8 >
.> ?

Exceptions? I
{ 
public		 

class		 
NotFoundException		 "
:		# $
	Exception		% .
{

 
public 
NotFoundException  
(  !
)! "
{ 	
} 	
public 
NotFoundException  
(  !
string! '
message( /
)/ 0
:1 2
base3 7
(7 8
message8 ?
)? @
{ 	
} 	
public 
NotFoundException  
(  !
string! '
message( /
,/ 0
	Exception1 :
innerException; I
)I J
:K L
baseM Q
(Q R
messageR Y
,Y Z
innerException[ i
)i j
{ 	
} 	
} 
} °
D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Address.cs
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
$str ;
,; <
Version= D
=E F
$strG L
)L M
]M N
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
{ 
public		 

class		 
Address		 
:		 
ValueObject		 &
{

 
	protected 
Address 
( 
) 
{ 	
} 	
public 
Address 
( 
string 
line1 #
,# $
string% +
line2, 1
,1 2
string3 9
city: >
,> ?
string@ F
postalG M
)M N
{ 	
Line1 
= 
line1 
; 
Line2 
= 
line2 
; 
City 
= 
city 
; 
Postal 
= 
postal 
; 
} 	
public 
string 
Line1 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 
string 
Line2 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
public 
string 
City 
{ 
get  
;  !
private" )
set* -
;- .
}/ 0
public 
string 
Postal 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
	protected 
override 
IEnumerable &
<& '
object' -
>- .!
GetEqualityComponents/ D
(D E
)E F
{ 	
yield 
return 
Line1 
; 
yield   
return   
Line2   
;   
yield!! 
return!! 
City!! 
;!! 
yield"" 
return"" 
Postal"" 
;""  
}## 	
}$$ 
}%% ®
äD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Common\DomainEvent.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str ?
,? @
VersionA H
=I J
$strK P
)P Q
]Q R
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Domain1 7
.7 8
Common8 >
{		 
public

 

abstract

 
class

 
DomainEvent

 %
{ 
	protected 
DomainEvent 
( 
) 
{ 	
DateOccurred 
= 
DateTimeOffset )
.) *
UtcNow* 0
;0 1
} 	
public 
bool 
IsPublished 
{  !
get" %
;% &
set' *
;* +
}, -
public 
DateTimeOffset 
DateOccurred *
{+ ,
get- 0
;0 1
	protected2 ;
set< ?
;? @
}A B
} 
} ä:
ìD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Domain\Common\CollectionExtensions.cs
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
$str @
,@ A
VersionB I
=J K
$strL Q
)Q R
]R S
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Domain		1 7
.		7 8
Common		8 >
{

 
public 

static 
class  
CollectionExtensions ,
{ 
public 
static 
ComparisonResult &
<& '
TChanged' /
,/ 0
	TOriginal1 :
>: ;
CompareCollections< N
<N O
TChangedO W
,W X
	TOriginalY b
>b c
(c d
this 
ICollection 
< 
	TOriginal &
>& '
baseCollection( 6
,6 7
IEnumerable 
< 
TChanged  
>  !
changedCollection" 3
,3 4
Func 
< 
	TOriginal 
, 
TChanged $
,$ %
bool& *
>* +
equalityCheck, 9
)9 :
{ 	
changedCollection 
??= !
new" %
List& *
<* +
TChanged+ 3
>3 4
(4 5
)5 6
;6 7
var   
toRemove   
=   
baseCollection   )
.  ) *
Where  * /
(  / 0
baseElement  0 ;
=>  < >
changedCollection  ? P
.  P Q
All  Q T
(  T U
changedElement  U c
=>  d f
!  g h
equalityCheck  h u
(  u v
baseElement	  v Å
,
  Å Ç
changedElement
  É ë
)
  ë í
)
  í ì
)
  ì î
.
  î ï
ToList
  ï õ
(
  õ ú
)
  ú ù
;
  ù û
var!! 
toAdd!! 
=!! 
changedCollection!! )
.!!) *
Where!!* /
(!!/ 0
changedElement!!0 >
=>!!? A
baseCollection!!B P
.!!P Q
All!!Q T
(!!T U
baseElement!!U `
=>!!a c
!!!d e
equalityCheck!!e r
(!!r s
baseElement!!s ~
,!!~ 
changedElement
!!Ä é
)
!!é è
)
!!è ê
)
!!ê ë
.
!!ë í
ToList
!!í ò
(
!!ò ô
)
!!ô ö
;
!!ö õ
var## 
possibleEdits## 
=## 
new##  #
List##$ (
<##( )
Match##) .
<##. /
TChanged##/ 7
,##7 8
	TOriginal##9 B
>##B C
>##C D
(##D E
)##E F
;##F G
foreach$$ 
($$ 
var$$ 
changedElement$$ '
in$$( *
changedCollection$$+ <
)$$< =
{%% 
var&& 
match&& 
=&& 
baseCollection&& *
.&&* +
FirstOrDefault&&+ 9
(&&9 :
baseElement&&: E
=>&&F H
equalityCheck&&I V
(&&V W
baseElement&&W b
,&&b c
changedElement&&d r
)&&r s
)&&s t
;&&t u
if'' 
('' 
match'' 
is'' 
not''  
null''! %
)''% &
{(( 
possibleEdits)) !
.))! "
Add))" %
())% &
new))& )
Match))* /
<))/ 0
TChanged))0 8
,))8 9
	TOriginal)): C
>))C D
())D E
changedElement))E S
,))S T
match))U Z
)))Z [
)))[ \
;))\ ]
}** 
}++ 
return-- 
new-- 
ComparisonResult-- '
<--' (
TChanged--( 0
,--0 1
	TOriginal--2 ;
>--; <
(--< =
toAdd--= B
,--B C
toRemove--D L
,--L M
possibleEdits--N [
)--[ \
;--\ ]
}.. 	
public55 
class55 
ComparisonResult55 %
<55% &
TChanged55& .
,55. /
	TOriginal550 9
>559 :
{66 	
public== 
ComparisonResult== #
(==# $
ICollection==$ /
<==/ 0
TChanged==0 8
>==8 9
toAdd==: ?
,==? @
ICollection>> 
<>> 
	TOriginal>> %
>>>% &
toRemove>>' /
,>>/ 0
ICollection?? 
<?? 
Match?? !
<??! "
TChanged??" *
,??* +
	TOriginal??, 5
>??5 6
>??6 7
possibleEdits??8 E
)??E F
{@@ 
ToAddAA 
=AA 
toAddAA 
;AA 
ToRemoveBB 
=BB 
toRemoveBB #
;BB# $
PossibleEditsCC 
=CC 
possibleEditsCC  -
;CC- .
}DD 
publicII 
ICollectionII 
<II 
TChangedII '
>II' (
ToAddII) .
{II/ 0
getII1 4
;II4 5
}II6 7
publicNN 
ICollectionNN 
<NN 
	TOriginalNN (
>NN( )
ToRemoveNN* 2
{NN3 4
getNN5 8
;NN8 9
}NN: ;
publicSS 
ICollectionSS 
<SS 
MatchSS $
<SS$ %
TChangedSS% -
,SS- .
	TOriginalSS/ 8
>SS8 9
>SS9 :
PossibleEditsSS; H
{SSI J
getSSK N
;SSN O
}SSP Q
publicYY 
boolYY 

HasChangesYY "
(YY" #
)YY# $
{ZZ 
return[[ 
ToAdd[[ 
.[[ 
Count[[ "
>[[# $
$num[[% &
||[[' )
ToRemove[[* 2
.[[2 3
Count[[3 8
>[[9 :
$num[[; <
||[[= ?
PossibleEdits[[@ M
.[[M N
Count[[N S
>[[T U
$num[[V W
;[[W X
}\\ 
}]] 	
publicdd 
classdd 
Matchdd 
<dd 
TChangeddd #
,dd# $
	TOriginaldd% .
>dd. /
{ee 	
publickk 
Matchkk 
(kk 
TChangedkk !
changedkk" )
,kk) *
	TOriginalkk+ 4
originalkk5 =
)kk= >
{ll 
Changedmm 
=mm 
changedmm !
;mm! "
Originalnn 
=nn 
originalnn #
;nn# $
}oo 
publictt 
TChangedtt 
Changedtt #
{tt$ %
gettt& )
;tt) *
privatett+ 2
settt3 6
;tt6 7
}tt8 9
publicyy 
	TOriginalyy 
Originalyy %
{yy& '
getyy( +
;yy+ ,
privateyy- 4
setyy5 8
;yy8 9
}yy: ;
}zz 	
}{{ 
}|| 