Ñ
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\WarehouseDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =

Warehouses

= G
{ 
public 

static 
class )
WarehouseDtoMappingExtensions 5
{ 
public 
static 
WarehouseDto "
MapToWarehouseDto# 4
(4 5
this5 9
	Warehouse: C
projectFromD O
,O P
IMapperQ X
mapperY _
)_ `
=> 
mapper 
. 
Map 
< 
WarehouseDto &
>& '
(' (
projectFrom( 3
)3 4
;4 5
public 
static 
List 
< 
WarehouseDto '
>' (!
MapToWarehouseDtoList) >
(> ?
this? C
IEnumerableD O
<O P
	WarehouseP Y
>Y Z
projectFrom[ f
,f g
IMapperh o
mapperp v
)v w
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToWarehouseDto) :
(: ;
mapper; A
)A B
)B C
.C D
ToListD J
(J K
)K L
;L M
} 
} ﬁ
îD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\WarehouseDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =

Warehouses

= G
{ 
public 

class 
WarehouseDto 
: 
IMapFrom  (
<( )
	Warehouse) 2
>2 3
{ 
public 
WarehouseDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
Size 
{ 
get 
; 
set "
;" #
}$ %
public 
static 
WarehouseDto "
Create# )
() *
Guid* .
id/ 1
,1 2
string3 9
name: >
,> ?
int@ C
sizeD H
)H I
{ 	
return 
new 
WarehouseDto #
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Size 
= 
size 
} 
; 
} 	
public!! 
void!! 
Mapping!! 
(!! 
Profile!! #
profile!!$ +
)!!+ ,
{"" 	
profile## 
.## 
	CreateMap## 
<## 
	Warehouse## '
,##' (
WarehouseDto##) 5
>##5 6
(##6 7
)##7 8
;##8 9
}$$ 	
}%% 
}&& Ù
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\UpdateWarehouse\UpdateWarehouseCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
UpdateWarehouseH W
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class +
UpdateWarehouseCommandValidator 0
:1 2
AbstractValidator3 D
<D E"
UpdateWarehouseCommandE [
>[ \
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
UpdateWarehouseCommandValidator .
(. /
IValidatorProvider/ A
providerB J
)J K
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Address "
)" #
. 
SetValidator 
( 
provider &
.& '
GetValidator' 3
<3 4,
 UpdateWarehouseCommandAddressDto4 T
>T U
(U V
)V W
!W X
)X Y
;Y Z
} 	
} 
} Ó 
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\UpdateWarehouse\UpdateWarehouseCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
UpdateWarehouseH W
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class )
UpdateWarehouseCommandHandler .
:/ 0
IRequestHandler1 @
<@ A"
UpdateWarehouseCommandA W
>W X
{ 
private 
readonly  
IWarehouseRepository - 
_warehouseRepository. B
;B C
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
UpdateWarehouseCommandHandler ,
(, - 
IWarehouseRepository- A
warehouseRepositoryB U
)U V
{ 	 
_warehouseRepository  
=! "
warehouseRepository# 6
;6 7
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !"
UpdateWarehouseCommand! 7
request8 ?
,? @
CancellationTokenA R
cancellationTokenS d
)d e
{ 	
var 
	warehouse 
= 
await ! 
_warehouseRepository" 6
.6 7
FindByIdAsync7 D
(D E
requestE L
.L M
IdM O
,O P
cancellationTokenQ b
)b c
;c d
if 
( 
	warehouse 
is 
null !
)! "
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . H
{  H I
request  I P
.  P Q
Id  Q S
}  S T
$str  T U
"  U V
)  V W
;  W X
}!! 
	warehouse## 
.## 
Name## 
=## 
request## $
.##$ %
Name##% )
;##) *
	warehouse$$ 
.$$ 
Size$$ 
=$$ 
request$$ $
.$$$ %
Size$$% )
;$$) *
	warehouse%% 
.%% 
Address%% 
=%% 
request%%  '
.%%' (
Address%%( /
is%%0 2
not%%3 6
null%%7 ;
?&& 
new&& 
Address&& 
(&& 
line1'' 
:'' 
request'' "
.''" #
Address''# *
.''* +
Line1''+ 0
,''0 1
line2(( 
:(( 
request(( "
.((" #
Address((# *
.((* +
Line2((+ 0
,((0 1
city)) 
:)) 
request)) !
.))! "
Address))" )
.))) *
City))* .
,)). /
postal** 
:** 
request** #
.**# $
Address**$ +
.**+ ,
Postal**, 2
)**2 3
:++ 
null++ 
;++ 
},, 	
}-- 
}.. Ó
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\UpdateWarehouse\UpdateWarehouseCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =

Warehouses		= G
.		G H
UpdateWarehouse		H W
{

 
public 

class "
UpdateWarehouseCommand '
:( )
IRequest* 2
,2 3
ICommand4 <
{ 
public "
UpdateWarehouseCommand %
(% &
string& ,
name- 1
,1 2
int3 6
size7 ;
,; <
Guid= A
idB D
,D E,
 UpdateWarehouseCommandAddressDtoF f
?f g
addressh o
)o p
{ 	
Name 
= 
name 
; 
Size 
= 
size 
; 
Id 
= 
id 
; 
Address 
= 
address 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
Size 
{ 
get 
; 
set "
;" #
}$ %
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public ,
 UpdateWarehouseCommandAddressDto /
?/ 0
Address1 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
} 
} »
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\UpdateWarehouseCommandAddressDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 5
)UpdateWarehouseCommandAddressDtoValidator

 :
:

; <
AbstractValidator

= N
<

N O,
 UpdateWarehouseCommandAddressDto

O o
>

o p
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 5
)UpdateWarehouseCommandAddressDtoValidator 8
(8 9
)9 :
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line2  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
City 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Postal !
)! "
. 
NotNull 
( 
) 
; 
} 	
}   
}!! ©
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\UpdateWarehouseCommandAddressDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
{ 
public 

class ,
 UpdateWarehouseCommandAddressDto 1
{		 
public

 ,
 UpdateWarehouseCommandAddressDto

 /
(

/ 0
)

0 1
{ 	
Line1 
= 
null 
! 
; 
Line2 
= 
null 
! 
; 
City 
= 
null 
! 
; 
Postal 
= 
null 
! 
; 
} 	
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Postal 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static ,
 UpdateWarehouseCommandAddressDto 6
Create7 =
(= >
string> D
line1E J
,J K
stringL R
line2S X
,X Y
stringZ `
citya e
,e f
stringg m
postaln t
)t u
{ 	
return 
new ,
 UpdateWarehouseCommandAddressDto 7
{ 
Line1 
= 
line1 
, 
Line2 
= 
line2 
, 
City 
= 
city 
, 
Postal 
= 
postal 
} 
; 
}   	
}!! 
}"" ¯
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\GetWarehouses\GetWarehousesQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
GetWarehousesH U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
GetWarehousesQueryValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
GetWarehousesQuery

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
GetWarehousesQueryValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} €
ØD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\GetWarehouses\GetWarehousesQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
GetWarehousesH U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class %
GetWarehousesQueryHandler *
:+ ,
IRequestHandler- <
<< =
GetWarehousesQuery= O
,O P
ListQ U
<U V
WarehouseDtoV b
>b c
>c d
{ 
private 
readonly  
IWarehouseRepository - 
_warehouseRepository. B
;B C
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
GetWarehousesQueryHandler (
(( ) 
IWarehouseRepository) =
warehouseRepository> Q
,Q R
IMapperS Z
mapper[ a
)a b
{ 	 
_warehouseRepository  
=! "
warehouseRepository# 6
;6 7
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
WarehouseDto +
>+ ,
>, -
Handle. 4
(4 5
GetWarehousesQuery5 G
requestH O
,O P
CancellationTokenQ b
cancellationTokenc t
)t u
{ 	
var 

warehouses 
= 
await " 
_warehouseRepository# 7
.7 8
FindAllAsync8 D
(D E
cancellationTokenE V
)V W
;W X
return   

warehouses   
.   !
MapToWarehouseDtoList   3
(  3 4
_mapper  4 ;
)  ; <
;  < =
}!! 	
}"" 
}## À	
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\GetWarehouses\GetWarehousesQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =

Warehouses		= G
.		G H
GetWarehouses		H U
{

 
public 

class 
GetWarehousesQuery #
:$ %
IRequest& .
<. /
List/ 3
<3 4
WarehouseDto4 @
>@ A
>A B
,B C
IQueryD J
{ 
public 
GetWarehousesQuery !
(! "
)" #
{ 	
} 	
} 
} ä
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\GetWarehouseById\GetWarehouseByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
GetWarehouseByIdH X
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
GetWarehouseByIdQueryValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
GetWarehouseByIdQuery

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
GetWarehouseByIdQueryValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ÿ
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\GetWarehouseById\GetWarehouseByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
GetWarehouseByIdH X
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class (
GetWarehouseByIdQueryHandler -
:. /
IRequestHandler0 ?
<? @!
GetWarehouseByIdQuery@ U
,U V
WarehouseDtoW c
>c d
{ 
private 
readonly  
IWarehouseRepository - 
_warehouseRepository. B
;B C
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
GetWarehouseByIdQueryHandler +
(+ , 
IWarehouseRepository, @
warehouseRepositoryA T
,T U
IMapperV ]
mapper^ d
)d e
{ 	 
_warehouseRepository  
=! "
warehouseRepository# 6
;6 7
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
WarehouseDto &
>& '
Handle( .
(. /!
GetWarehouseByIdQuery/ D
requestE L
,L M
CancellationTokenN _
cancellationToken` q
)q r
{ 	
var 
	warehouse 
= 
await ! 
_warehouseRepository" 6
.6 7
FindByIdAsync7 D
(D E
requestE L
.L M
IdM O
,O P
cancellationTokenQ b
)b c
;c d
if   
(   
	warehouse   
is   
null   !
)  ! "
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". H
{""H I
request""I P
.""P Q
Id""Q S
}""S T
$str""T U
"""U V
)""V W
;""W X
}## 
return$$ 
	warehouse$$ 
.$$ 
MapToWarehouseDto$$ .
($$. /
_mapper$$/ 6
)$$6 7
;$$7 8
}%% 	
}&& 
}'' û
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\GetWarehouseById\GetWarehouseByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =

Warehouses		= G
.		G H
GetWarehouseById		H X
{

 
public 

class !
GetWarehouseByIdQuery &
:' (
IRequest) 1
<1 2
WarehouseDto2 >
>> ?
,? @
IQueryA G
{ 
public !
GetWarehouseByIdQuery $
($ %
Guid% )
id* ,
), -
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} å
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\DeleteWarehouse\DeleteWarehouseCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
DeleteWarehouseH W
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 +
DeleteWarehouseCommandValidator

 0
:

1 2
AbstractValidator

3 D
<

D E"
DeleteWarehouseCommand

E [
>

[ \
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
DeleteWarehouseCommandValidator .
(. /
)/ 0
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ö
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\DeleteWarehouse\DeleteWarehouseCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
DeleteWarehouseH W
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
class )
DeleteWarehouseCommandHandler .
:/ 0
IRequestHandler1 @
<@ A"
DeleteWarehouseCommandA W
>W X
{ 
private 
readonly  
IWarehouseRepository - 
_warehouseRepository. B
;B C
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
DeleteWarehouseCommandHandler ,
(, - 
IWarehouseRepository- A
warehouseRepositoryB U
)U V
{ 	 
_warehouseRepository  
=! "
warehouseRepository# 6
;6 7
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !"
DeleteWarehouseCommand! 7
request8 ?
,? @
CancellationTokenA R
cancellationTokenS d
)d e
{ 	
var 
	warehouse 
= 
await ! 
_warehouseRepository" 6
.6 7
FindByIdAsync7 D
(D E
requestE L
.L M
IdM O
,O P
cancellationTokenQ b
)b c
;c d
if 
( 
	warehouse 
is 
null !
)! "
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. H
{H I
requestI P
.P Q
IdQ S
}S T
$strT U
"U V
)V W
;W X
}    
_warehouseRepository""  
.""  !
Remove""! '
(""' (
	warehouse""( 1
)""1 2
;""2 3
}## 	
}$$ 
}%% È

ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\DeleteWarehouse\DeleteWarehouseCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =

Warehouses		= G
.		G H
DeleteWarehouse		H W
{

 
public 

class "
DeleteWarehouseCommand '
:( )
IRequest* 2
,2 3
ICommand4 <
{ 
public "
DeleteWarehouseCommand %
(% &
Guid& *
id+ -
)- .
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ù
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\CreateWarehouse\CreateWarehouseCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
CreateWarehouseH W
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class +
CreateWarehouseCommandValidator 0
:1 2
AbstractValidator3 D
<D E"
CreateWarehouseCommandE [
>[ \
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
CreateWarehouseCommandValidator .
(. /
IValidatorProvider/ A
providerB J
)J K
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Address "
)" #
. 
SetValidator 
( 
provider &
.& '
GetValidator' 3
<3 4,
 CreateWarehouseCommandAddressDto4 T
>T U
(U V
)V W
!W X
)X Y
;Y Z
} 	
} 
} œ
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\CreateWarehouse\CreateWarehouseCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
.G H
CreateWarehouseH W
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class )
CreateWarehouseCommandHandler .
:/ 0
IRequestHandler1 @
<@ A"
CreateWarehouseCommandA W
,W X
GuidY ]
>] ^
{ 
private 
readonly  
IWarehouseRepository - 
_warehouseRepository. B
;B C
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
CreateWarehouseCommandHandler ,
(, - 
IWarehouseRepository- A
warehouseRepositoryB U
)U V
{ 	 
_warehouseRepository  
=! "
warehouseRepository# 6
;6 7
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '"
CreateWarehouseCommand' =
request> E
,E F
CancellationTokenG X
cancellationTokenY j
)j k
{ 	
var 
	warehouse 
= 
new 
	Warehouse  )
{ 
Name 
= 
request 
. 
Name #
,# $
Size   
=   
request   
.   
Size   #
,  # $
Address!! 
=!! 
request!! !
.!!! "
Address!!" )
is!!* ,
not!!- 0
null!!1 5
?"" 
new"" 
Address"" !
(""! "
line1## 
:## 
request## &
.##& '
Address##' .
.##. /
Line1##/ 4
,##4 5
line2$$ 
:$$ 
request$$ &
.$$& '
Address$$' .
.$$. /
Line2$$/ 4
,$$4 5
city%% 
:%% 
request%% %
.%%% &
Address%%& -
.%%- .
City%%. 2
,%%2 3
postal&& 
:&& 
request&&  '
.&&' (
Address&&( /
.&&/ 0
Postal&&0 6
)&&6 7
:'' 
null'' 
}(( 
;((  
_warehouseRepository**  
.**  !
Add**! $
(**$ %
	warehouse**% .
)**. /
;**/ 0
await++  
_warehouseRepository++ &
.++& '

UnitOfWork++' 1
.++1 2
SaveChangesAsync++2 B
(++B C
cancellationToken++C T
)++T U
;++U V
return,, 
	warehouse,, 
.,, 
Id,, 
;,,  
}-- 	
}.. 
}// õ
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\CreateWarehouse\CreateWarehouseCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =

Warehouses		= G
.		G H
CreateWarehouse		H W
{

 
public 

class "
CreateWarehouseCommand '
:( )
IRequest* 2
<2 3
Guid3 7
>7 8
,8 9
ICommand: B
{ 
public "
CreateWarehouseCommand %
(% &
string& ,
name- 1
,1 2
int3 6
size7 ;
,; <,
 CreateWarehouseCommandAddressDto= ]
?] ^
address_ f
)f g
{ 	
Name 
= 
name 
; 
Size 
= 
size 
; 
Address 
= 
address 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
Size 
{ 
get 
; 
set "
;" #
}$ %
public ,
 CreateWarehouseCommandAddressDto /
?/ 0
Address1 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
} 
} »
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\CreateWarehouseCommandAddressDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 5
)CreateWarehouseCommandAddressDtoValidator

 :
:

; <
AbstractValidator

= N
<

N O,
 CreateWarehouseCommandAddressDto

O o
>

o p
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 5
)CreateWarehouseCommandAddressDtoValidator 8
(8 9
)9 :
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line2  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
City 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Postal !
)! "
. 
NotNull 
( 
) 
; 
} 	
}   
}!! ©
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Warehouses\CreateWarehouseCommandAddressDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Warehouses= G
{ 
public 

class ,
 CreateWarehouseCommandAddressDto 1
{		 
public

 ,
 CreateWarehouseCommandAddressDto

 /
(

/ 0
)

0 1
{ 	
Line1 
= 
null 
! 
; 
Line2 
= 
null 
! 
; 
City 
= 
null 
! 
; 
Postal 
= 
null 
! 
; 
} 	
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Postal 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static ,
 CreateWarehouseCommandAddressDto 6
Create7 =
(= >
string> D
line1E J
,J K
stringL R
line2S X
,X Y
stringZ `
citya e
,e f
stringg m
postaln t
)t u
{ 	
return 
new ,
 CreateWarehouseCommandAddressDto 7
{ 
Line1 
= 
line1 
, 
Line2 
= 
line2 
, 
City 
= 
city 
, 
Postal 
= 
postal 
} 
; 
}   	
}!! 
}"" »
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UserDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Users

= B
{ 
public 

static 
class $
UserDtoMappingExtensions 0
{ 
public 
static 
UserDto 
MapToUserDto *
(* +
this+ /
User0 4
projectFrom5 @
,@ A
IMapperB I
mapperJ P
)P Q
=> 
mapper 
. 
Map 
< 
UserDto !
>! "
(" #
projectFrom# .
). /
;/ 0
public 
static 
List 
< 
UserDto "
>" #
MapToUserDtoList$ 4
(4 5
this5 9
IEnumerable: E
<E F
UserF J
>J K
projectFromL W
,W X
IMapperY `
mappera g
)g h
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToUserDto) 5
(5 6
mapper6 <
)< =
)= >
.> ?
ToList? E
(E F
)F G
;G H
} 
} é
äD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UserDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
Users

= B
{ 
public 

class 
UserDto 
: 
IMapFrom #
<# $
User$ (
>( )
{ 
public 
UserDto 
( 
) 
{ 	
Email 
= 
null 
! 
; 
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
QuoteId 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static 
UserDto 
Create $
($ %
Guid% )
id* ,
,, -
string. 4
email5 :
,: ;
Guid< @
quoteIdA H
,H I
stringJ P
nameQ U
,U V
stringW ]
surname^ e
)e f
{ 	
return 
new 
UserDto 
{ 
Id 
= 
id 
, 
Email   
=   
email   
,   
QuoteId!! 
=!! 
quoteId!! !
,!!! "
Name"" 
="" 
name"" 
,"" 
Surname## 
=## 
surname## !
}$$ 
;$$ 
}%% 	
public'' 
void'' 
Mapping'' 
('' 
Profile'' #
profile''$ +
)''+ ,
{(( 	
profile)) 
.)) 
	CreateMap)) 
<)) 
User)) "
,))" #
UserDto))$ +
>))+ ,
()), -
)))- .
;)). /
}** 	
}++ 
},, é
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UserAddressDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Users

= B
{ 
public 

static 
class +
UserAddressDtoMappingExtensions 7
{ 
public 
static 
UserAddressDto $
MapToUserAddressDto% 8
(8 9
this9 =
UserAddress> I
projectFromJ U
,U V
IMapperW ^
mapper_ e
)e f
=> 
mapper 
. 
Map 
< 
UserAddressDto (
>( )
() *
projectFrom* 5
)5 6
;6 7
public 
static 
List 
< 
UserAddressDto )
>) *#
MapToUserAddressDtoList+ B
(B C
thisC G
IEnumerableH S
<S T
UserAddressT _
>_ `
projectFroma l
,l m
IMappern u
mapperv |
)| }
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToUserAddressDto) <
(< =
mapper= C
)C D
)D E
.E F
ToListF L
(L M
)M N
;N O
} 
} ¢
ëD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UserAddressDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
Users

= B
{ 
public 

class 
UserAddressDto 
:  !
IMapFrom" *
<* +
UserAddress+ 6
>6 7
{ 
public 
UserAddressDto 
( 
) 
{ 	
Line1 
= 
null 
! 
; 
Line2 
= 
null 
! 
; 
City 
= 
null 
! 
; 
Postal 
= 
null 
! 
; 
} 	
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Postal 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static 
UserAddressDto $
Create% +
(+ ,
Guid, 0
userId1 7
,7 8
Guid9 =
id> @
,@ A
stringB H
line1I N
,N O
stringP V
line2W \
,\ ]
string^ d
citye i
,i j
stringk q
postalr x
)x y
{ 	
return 
new 
UserAddressDto %
{   
UserId!! 
=!! 
userId!! 
,!!  
Id"" 
="" 
id"" 
,"" 
Line1## 
=## 
line1## 
,## 
Line2$$ 
=$$ 
line2$$ 
,$$ 
City%% 
=%% 
city%% 
,%% 
Postal&& 
=&& 
postal&& 
}'' 
;'' 
}(( 	
public** 
void** 
Mapping** 
(** 
Profile** #
profile**$ +
)**+ ,
{++ 	
profile,, 
.,, 
	CreateMap,, 
<,, 
UserAddress,, )
,,,) *
UserAddressDto,,+ 9
>,,9 :
(,,: ;
),,; <
;,,< =
}-- 	
}.. 
}// â
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UpdateUser\UpdateUserCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C

UpdateUserC M
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
UpdateUserCommandValidator

 +
:

, -
AbstractValidator

. ?
<

? @
UpdateUserCommand

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
UpdateUserCommandValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Email  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor   
(   
v   
=>   
v   
.   
Line2    
)    !
.!! 
NotNull!! 
(!! 
)!! 
;!! 
}"" 	
}## 
}$$ Ì"
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UpdateUser\UpdateUserCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C

UpdateUserC M
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class $
UpdateUserCommandHandler )
:* +
IRequestHandler, ;
<; <
UpdateUserCommand< M
>M N
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
UpdateUserCommandHandler '
(' (
IUserRepository( 7
userRepository8 F
)F G
{ 	
_userRepository 
= 
userRepository ,
;, -
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
UpdateUserCommand! 2
request3 :
,: ;
CancellationToken< M
cancellationTokenN _
)_ `
{ 	
var 
user 
= 
await 
_userRepository ,
., -
FindByIdAsync- :
(: ;
request; B
.B C
IdC E
,E F
cancellationTokenG X
)X Y
;Y Z
if 
( 
user 
is 
null 
) 
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . C
{  C D
request  D K
.  K L
Id  L N
}  N O
$str  O P
"  P Q
)  Q R
;  R S
}!! 
user## 
.## 
Email## 
=## 
request##  
.##  !
Email##! &
;##& '
user$$ 
.$$ 
Name$$ 
=$$ 
request$$ 
.$$  
Name$$  $
;$$$ %
user%% 
.%% 
Surname%% 
=%% 
request%% "
.%%" #
Surname%%# *
;%%* +
user&& 
.&& 
QuoteId&& 
=&& 
request&& "
.&&" #
QuoteId&&# *
;&&* +
user'' 
.'' "
DefaultDeliveryAddress'' '
.''' (
Line1''( -
=''. /
request''0 7
.''7 8
Line1''8 =
;''= >
user(( 
.(( "
DefaultDeliveryAddress(( '
.((' (
Line2((( -
=((. /
request((0 7
.((7 8
Line2((8 =
;((= >
user)) 
.)) !
DefaultBillingAddress)) &
??=))' *
new))+ .
UserDefaultAddress))/ A
())A B
)))B C
;))C D
user** 
.** !
DefaultBillingAddress** &
.**& '
Line1**' ,
=**- .
request**/ 6
.**6 7
Line1**7 <
;**< =
user++ 
.++ !
DefaultBillingAddress++ &
.++& '
Line2++' ,
=++- .
request++/ 6
.++6 7
Line2++7 <
;++< =
},, 	
}-- 
}.. ¿
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UpdateUser\UpdateUserCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C

UpdateUser		C M
{

 
public 

class 
UpdateUserCommand "
:# $
IRequest% -
,- .
ICommand/ 7
{ 
public 
UpdateUserCommand  
(  !
Guid! %
id& (
,( )
string* 0
email1 6
,6 7
Guid8 <
quoteId= D
,D E
stringF L
nameM Q
,Q R
stringS Y
surnameZ a
,a b
stringc i
line1j o
,o p
stringq w
line2x }
)} ~
{ 	
Id 
= 
id 
; 
Email 
= 
email 
; 
QuoteId 
= 
quoteId 
; 
Name 
= 
name 
; 
Surname 
= 
surname 
; 
Line1 
= 
line1 
; 
Line2 
= 
line2 
; 
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
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
QuoteId 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
} 
}   ﬁ
∂D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UpdateUserAddress\UpdateUserAddressCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
UpdateUserAddressC T
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 -
!UpdateUserAddressCommandValidator

 2
:

3 4
AbstractValidator

5 F
<

F G$
UpdateUserAddressCommand

G _
>

_ `
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!UpdateUserAddressCommandValidator 0
(0 1
)1 2
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line2  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
City 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Postal !
)! "
. 
NotNull 
( 
) 
; 
} 	
}   
}!! ˜"
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UpdateUserAddress\UpdateUserAddressCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
UpdateUserAddressC T
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class +
UpdateUserAddressCommandHandler 0
:1 2
IRequestHandler3 B
<B C$
UpdateUserAddressCommandC [
>[ \
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
UpdateUserAddressCommandHandler .
(. /
IUserRepository/ >
userRepository? M
)M N
{ 	
_userRepository 
= 
userRepository ,
;, -
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !$
UpdateUserAddressCommand! 9
request: A
,A B
CancellationTokenC T
cancellationTokenU f
)f g
{ 	
var 
user 
= 
await 
_userRepository ,
., -
FindByIdAsync- :
(: ;
request; B
.B C
UserIdC I
,I J
cancellationTokenK \
)\ ]
;] ^
if 
( 
user 
is 
null 
) 
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . J
{  J K
request  K R
.  R S
UserId  S Y
}  Y Z
$str  Z [
"  [ \
)  \ ]
;  ] ^
}!! 
var## 
address## 
=## 
user## 
.## 
	Addresses## (
.##( )
FirstOrDefault##) 7
(##7 8
x##8 9
=>##: <
x##= >
.##> ?
Id##? A
==##B D
request##E L
.##L M
Id##M O
)##O P
;##P Q
if$$ 
($$ 
address$$ 
is$$ 
null$$ 
)$$  
{%% 
throw&& 
new&& 
NotFoundException&& +
(&&+ ,
$"&&, .
$str&&. J
{&&J K
request&&K R
.&&R S
Id&&S U
}&&U V
$str&&V W
"&&W X
)&&X Y
;&&Y Z
}'' 
address)) 
.)) 
UserId)) 
=)) 
request)) $
.))$ %
UserId))% +
;))+ ,
address** 
.** 
Line1** 
=** 
request** #
.**# $
Line1**$ )
;**) *
address++ 
.++ 
Line2++ 
=++ 
request++ #
.++# $
Line2++$ )
;++) *
address,, 
.,, 
City,, 
=,, 
request,, "
.,," #
City,,# '
;,,' (
address-- 
.-- 
Postal-- 
=-- 
request-- $
.--$ %
Postal--% +
;--+ ,
}.. 	
}// 
}00 »
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\UpdateUserAddress\UpdateUserAddressCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C
UpdateUserAddress		C T
{

 
public 

class $
UpdateUserAddressCommand )
:* +
IRequest, 4
,4 5
ICommand6 >
{ 
public $
UpdateUserAddressCommand '
(' (
Guid( ,
userId- 3
,3 4
Guid5 9
id: <
,< =
string> D
line1E J
,J K
stringL R
line2S X
,X Y
stringZ `
citya e
,e f
stringg m
postaln t
)t u
{ 	
UserId 
= 
userId 
; 
Id 
= 
id 
; 
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
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Postal 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} –
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUsers\GetUsersQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUsersC K
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 "
GetUsersQueryValidator

 '
:

( )
AbstractValidator

* ;
<

; <
GetUsersQuery

< I
>

I J
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public "
GetUsersQueryValidator %
(% &
)& '
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ˆ#
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUsers\GetUsersQueryHandler.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUsersC K
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class  
GetUsersQueryHandler %
:& '
IRequestHandler( 7
<7 8
GetUsersQuery8 E
,E F
PagedResultG R
<R S
UserDtoS Z
>Z [
>[ \
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public  
GetUsersQueryHandler #
(# $
IUserRepository$ 3
userRepository4 B
,B C
IMapperD K
mapperL R
)R S
{ 	
_userRepository 
= 
userRepository ,
;, -
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public   
async   
Task   
<   
PagedResult   %
<  % &
UserDto  & -
>  - .
>  . /
Handle  0 6
(  6 7
GetUsersQuery  7 D
request  E L
,  L M
CancellationToken  N _
cancellationToken  ` q
)  q r
{!! 	

IQueryable"" 
<"" 
User"" 
>"" 
FilterUsers"" (
(""( )

IQueryable"") 3
<""3 4
User""4 8
>""8 9
	queryable"": C
)""C D
{## 
if$$ 
($$ 
request$$ 
.$$ 
Name$$  
!=$$! #
null$$$ (
)$$( )
{%% 
	queryable&& 
=&& 
	queryable&&  )
.&&) *
Where&&* /
(&&/ 0
x&&0 1
=>&&2 4
x&&5 6
.&&6 7
Name&&7 ;
==&&< >
request&&? F
.&&F G
Name&&G K
)&&K L
;&&L M
}'' 
if)) 
()) 
request)) 
.)) 
Surname)) #
!=))$ &
null))' +
)))+ ,
{** 
	queryable++ 
=++ 
	queryable++  )
.++) *
Where++* /
(++/ 0
x++0 1
=>++2 4
x++5 6
.++6 7
Surname++7 >
==++? A
request++B I
.++I J
Surname++J Q
)++Q R
;++R S
},, 
return.. 
	queryable..  
;..  !
}// 
var11 
users11 
=11 
await11 
_userRepository11 -
.11- .
FindAllAsync11. :
(11: ;
request11; B
.11B C
PageNo11C I
,11I J
request11K R
.11R S
PageSize11S [
,11[ \
FilterUsers11] h
,11h i
cancellationToken11j {
)11{ |
;11| }
return22 
users22 
.22 
MapToPagedResult22 )
(22) *
x22* +
=>22, .
x22/ 0
.220 1
MapToUserDto221 =
(22= >
_mapper22> E
)22E F
)22F G
;22G H
}33 	
}44 
}55 ´
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUsers\GetUsersQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
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
0 1
Application

1 <
.

< =
Users

= B
.

B C
GetUsers

C K
{ 
public 

class 
GetUsersQuery 
:  
IRequest! )
<) *
PagedResult* 5
<5 6
UserDto6 =
>= >
>> ?
,? @
IQueryA G
{ 
public 
GetUsersQuery 
( 
string #
?# $
name% )
,) *
string+ 1
?1 2
surname3 :
,: ;
int< ?
pageNo@ F
,F G
intH K
pageSizeL T
)T U
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
} 	
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Surname 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
int 
PageNo 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
} 
} ‚
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserById\GetUserByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUserByIdC N
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 %
GetUserByIdQueryValidator

 *
:

+ ,
AbstractValidator

- >
<

> ?
GetUserByIdQuery

? O
>

O P
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
GetUserByIdQueryValidator (
(( )
)) *
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Í
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserById\GetUserByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUserByIdC N
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class #
GetUserByIdQueryHandler (
:) *
IRequestHandler+ :
<: ;
GetUserByIdQuery; K
,K L
UserDtoM T
>T U
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public #
GetUserByIdQueryHandler &
(& '
IUserRepository' 6
userRepository7 E
,E F
IMapperG N
mapperO U
)U V
{ 	
_userRepository 
= 
userRepository ,
;, -
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
UserDto !
>! "
Handle# )
() *
GetUserByIdQuery* :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 	
var 
user 
= 
await 
_userRepository ,
., -
FindByIdAsync- :
(: ;
request; B
.B C
IdC E
,E F
cancellationTokenG X
)X Y
;Y Z
if   
(   
user   
is   
null   
)   
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". C
{""C D
request""D K
.""K L
Id""L N
}""N O
$str""O P
"""P Q
)""Q R
;""R S
}## 
return$$ 
user$$ 
.$$ 
MapToUserDto$$ $
($$$ %
_mapper$$% ,
)$$, -
;$$- .
}%% 	
}&& 
}'' ˆ

üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserById\GetUserByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C
GetUserById		C N
{

 
public 

class 
GetUserByIdQuery !
:" #
IRequest$ ,
<, -
UserDto- 4
>4 5
,5 6
IQuery7 =
{ 
public 
GetUserByIdQuery 
(  
Guid  $
id% '
)' (
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ä
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserAddresses\GetUserAddressesQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUserAddressesC S
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
GetUserAddressesQueryValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
GetUserAddressesQuery

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
GetUserAddressesQueryValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Í
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserAddresses\GetUserAddressesQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUserAddressesC S
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class (
GetUserAddressesQueryHandler -
:. /
IRequestHandler0 ?
<? @!
GetUserAddressesQuery@ U
,U V
ListW [
<[ \
UserAddressDto\ j
>j k
>k l
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
GetUserAddressesQueryHandler +
(+ ,
IUserRepository, ;
userRepository< J
,J K
IMapperL S
mapperT Z
)Z [
{ 	
_userRepository 
= 
userRepository ,
;, -
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
UserAddressDto -
>- .
>. /
Handle0 6
(6 7!
GetUserAddressesQuery7 L
requestM T
,T U
CancellationTokenV g
cancellationTokenh y
)y z
{   	
var!! 
user!! 
=!! 
await!! 
_userRepository!! ,
.!!, -
FindByIdAsync!!- :
(!!: ;
request!!; B
.!!B C
UserId!!C I
,!!I J
cancellationToken!!K \
)!!\ ]
;!!] ^
if"" 
("" 
user"" 
is"" 
null"" 
)"" 
{## 
throw$$ 
new$$ 
NotFoundException$$ +
($$+ ,
$"$$, .
$str$$. J
{$$J K
request$$K R
.$$R S
UserId$$S Y
}$$Y Z
$str$$Z [
"$$[ \
)$$\ ]
;$$] ^
}%% 
var'' 
	addresses'' 
='' 
user''  
.''  !
	Addresses''! *
.''* +
Where''+ 0
(''0 1
x''1 2
=>''3 5
x''6 7
.''7 8
UserId''8 >
==''? A
request''B I
.''I J
UserId''J P
)''P Q
;''Q R
return(( 
	addresses(( 
.(( #
MapToUserAddressDtoList(( 4
(((4 5
_mapper((5 <
)((< =
;((= >
})) 	
}** 
}++ ÷
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserAddresses\GetUserAddressesQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
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
0 1
Application

1 <
.

< =
Users

= B
.

B C
GetUserAddresses

C S
{ 
public 

class !
GetUserAddressesQuery &
:' (
IRequest) 1
<1 2
List2 6
<6 7
UserAddressDto7 E
>E F
>F G
,G H
IQueryI O
{ 
public !
GetUserAddressesQuery $
($ %
Guid% )
userId* 0
)0 1
{ 	
UserId 
= 
userId 
; 
} 	
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
} 
} å
∂D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserAddressById\GetUserAddressByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUserAddressByIdC U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 ,
 GetUserAddressByIdQueryValidator

 1
:

2 3
AbstractValidator

4 E
<

E F#
GetUserAddressByIdQuery

F ]
>

] ^
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 GetUserAddressByIdQueryValidator /
(/ 0
)0 1
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ã#
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserAddressById\GetUserAddressByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
GetUserAddressByIdC U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class *
GetUserAddressByIdQueryHandler /
:0 1
IRequestHandler2 A
<A B#
GetUserAddressByIdQueryB Y
,Y Z
UserAddressDto[ i
>i j
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
GetUserAddressByIdQueryHandler -
(- .
IUserRepository. =
userRepository> L
,L M
IMapperN U
mapperV \
)\ ]
{ 	
_userRepository 
= 
userRepository ,
;, -
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
UserAddressDto (
>( )
Handle* 0
(0 1#
GetUserAddressByIdQuery1 H
requestI P
,P Q
CancellationTokenR c
cancellationTokend u
)u v
{ 	
var   
user   
=   
await   
_userRepository   ,
.  , -
FindByIdAsync  - :
(  : ;
request  ; B
.  B C
UserId  C I
,  I J
cancellationToken  K \
)  \ ]
;  ] ^
if!! 
(!! 
user!! 
is!! 
null!! 
)!! 
{"" 
throw## 
new## 
NotFoundException## +
(##+ ,
$"##, .
$str##. J
{##J K
request##K R
.##R S
UserId##S Y
}##Y Z
$str##Z [
"##[ \
)##\ ]
;##] ^
}$$ 
var&& 
address&& 
=&& 
user&& 
.&& 
	Addresses&& (
.&&( )
FirstOrDefault&&) 7
(&&7 8
x&&8 9
=>&&: <
x&&= >
.&&> ?
Id&&? A
==&&B D
request&&E L
.&&L M
Id&&M O
&&&&P R
x&&S T
.&&T U
UserId&&U [
==&&\ ^
request&&_ f
.&&f g
UserId&&g m
)&&m n
;&&n o
if'' 
('' 
address'' 
is'' 
null'' 
)''  
{(( 
throw)) 
new)) 
NotFoundException)) +
())+ ,
$")), .
$str)). K
{))K L
request))L S
.))S T
Id))T V
}))V W
$str))W Y
{))Y Z
request))Z a
.))a b
UserId))b h
}))h i
$str))i k
"))k l
)))l m
;))m n
}** 
return++ 
address++ 
.++ 
MapToUserAddressDto++ .
(++. /
_mapper++/ 6
)++6 7
;++7 8
},, 	
}-- 
}.. ≥
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\GetUserAddressById\GetUserAddressByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C
GetUserAddressById		C U
{

 
public 

class #
GetUserAddressByIdQuery (
:) *
IRequest+ 3
<3 4
UserAddressDto4 B
>B C
,C D
IQueryE K
{ 
public #
GetUserAddressByIdQuery &
(& '
Guid' +
userId, 2
,2 3
Guid4 8
id9 ;
); <
{ 	
UserId 
= 
userId 
; 
Id 
= 
id 
; 
} 	
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ‰
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\DeleteUser\DeleteUserCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C

DeleteUserC M
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
DeleteUserCommandValidator

 +
:

, -
AbstractValidator

. ?
<

? @
DeleteUserCommand

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
DeleteUserCommandValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} °
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\DeleteUser\DeleteUserCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C

DeleteUserC M
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
class $
DeleteUserCommandHandler )
:* +
IRequestHandler, ;
<; <
DeleteUserCommand< M
>M N
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
DeleteUserCommandHandler '
(' (
IUserRepository( 7
userRepository8 F
)F G
{ 	
_userRepository 
= 
userRepository ,
;, -
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
DeleteUserCommand! 2
request3 :
,: ;
CancellationToken< M
cancellationTokenN _
)_ `
{ 	
var 
user 
= 
await 
_userRepository ,
., -
FindByIdAsync- :
(: ;
request; B
.B C
IdC E
,E F
cancellationTokenG X
)X Y
;Y Z
if 
( 
user 
is 
null 
) 
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. C
{C D
requestD K
.K L
IdL N
}N O
$strO P
"P Q
)Q R
;R S
}   
_userRepository"" 
."" 
Remove"" "
(""" #
user""# '
)""' (
;""( )
}## 	
}$$ 
}%% ∆

üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\DeleteUser\DeleteUserCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C

DeleteUser		C M
{

 
public 

class 
DeleteUserCommand "
:# $
IRequest% -
,- .
ICommand/ 7
{ 
public 
DeleteUserCommand  
(  !
Guid! %
id& (
)( )
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} é
∂D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\DeleteUserAddress\DeleteUserAddressCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
DeleteUserAddressC T
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 -
!DeleteUserAddressCommandValidator

 2
:

3 4
AbstractValidator

5 F
<

F G$
DeleteUserAddressCommand

G _
>

_ `
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!DeleteUserAddressCommandValidator 0
(0 1
)1 2
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ÷
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\DeleteUserAddress\DeleteUserAddressCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
DeleteUserAddressC T
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class +
DeleteUserAddressCommandHandler 0
:1 2
IRequestHandler3 B
<B C$
DeleteUserAddressCommandC [
>[ \
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
DeleteUserAddressCommandHandler .
(. /
IUserRepository/ >
userRepository? M
)M N
{ 	
_userRepository 
= 
userRepository ,
;, -
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !$
DeleteUserAddressCommand! 9
request: A
,A B
CancellationTokenC T
cancellationTokenU f
)f g
{ 	
var 
user 
= 
await 
_userRepository ,
., -
FindByIdAsync- :
(: ;
request; B
.B C
UserIdC I
,I J
cancellationTokenK \
)\ ]
;] ^
if 
( 
user 
is 
null 
) 
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . J
{  J K
request  K R
.  R S
UserId  S Y
}  Y Z
$str  Z [
"  [ \
)  \ ]
;  ] ^
}!! 
var## 
address## 
=## 
user## 
.## 
	Addresses## (
.##( )
FirstOrDefault##) 7
(##7 8
x##8 9
=>##: <
x##= >
.##> ?
Id##? A
==##B D
request##E L
.##L M
Id##M O
)##O P
;##P Q
if$$ 
($$ 
address$$ 
is$$ 
null$$ 
)$$  
{%% 
throw&& 
new&& 
NotFoundException&& +
(&&+ ,
$"&&, .
$str&&. J
{&&J K
request&&K R
.&&R S
Id&&S U
}&&U V
$str&&V W
"&&W X
)&&X Y
;&&Y Z
}'' 
user)) 
.)) 
	Addresses)) 
.)) 
Remove)) !
())! "
address))" )
)))) *
;))* +
}** 	
}++ 
},, ¸
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\DeleteUserAddress\DeleteUserAddressCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C
DeleteUserAddress		C T
{

 
public 

class $
DeleteUserAddressCommand )
:* +
IRequest, 4
,4 5
ICommand6 >
{ 
public $
DeleteUserAddressCommand '
(' (
Guid( ,
userId- 3
,3 4
Guid5 9
id: <
)< =
{ 	
UserId 
= 
userId 
; 
Id 
= 
id 
; 
} 	
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ·
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\CreateUser\CreateUserCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C

CreateUserC M
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
CreateUserCommandValidator

 +
:

, -
AbstractValidator

. ?
<

? @
CreateUserCommand

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
CreateUserCommandValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Email  
)  !
. 
NotNull 
( 
) 
; 
} 	
} 
} å
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\CreateUser\CreateUserCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C

CreateUserC M
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
class $
CreateUserCommandHandler )
:* +
IRequestHandler, ;
<; <
CreateUserCommand< M
,M N
GuidO S
>S T
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
CreateUserCommandHandler '
(' (
IUserRepository( 7
userRepository8 F
)F G
{ 	
_userRepository 
= 
userRepository ,
;, -
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '
CreateUserCommand' 8
request9 @
,@ A
CancellationTokenB S
cancellationTokenT e
)e f
{ 	
var 
user 
= 
new 
User 
(  
name 
: 
request 
. 
Name "
," #
surname 
: 
request  
.  !
Surname! (
,( )
email 
: 
request 
. 
Email $
,$ %
quoteId   
:   
request    
.    !
QuoteId  ! (
)  ( )
;  ) *
_userRepository"" 
."" 
Add"" 
(""  
user""  $
)""$ %
;""% &
await## 
_userRepository## !
.##! "

UnitOfWork##" ,
.##, -
SaveChangesAsync##- =
(##= >
cancellationToken##> O
)##O P
;##P Q
return$$ 
user$$ 
.$$ 
Id$$ 
;$$ 
}%% 	
}&& 
}'' «
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\CreateUser\CreateUserCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C

CreateUser		C M
{

 
public 

class 
CreateUserCommand "
:# $
IRequest% -
<- .
Guid. 2
>2 3
,3 4
ICommand5 =
{ 
public 
CreateUserCommand  
(  !
string! '
name( ,
,, -
string. 4
surname5 <
,< =
string> D
emailE J
,J K
GuidL P
quoteIdQ X
)X Y
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
Email 
= 
email 
; 
QuoteId 
= 
quoteId 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
QuoteId 
{ 
get !
;! "
set# &
;& '
}( )
} 
} ﬁ
∂D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\CreateUserAddress\CreateUserAddressCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
CreateUserAddressC T
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 -
!CreateUserAddressCommandValidator

 2
:

3 4
AbstractValidator

5 F
<

F G$
CreateUserAddressCommand

G _
>

_ `
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!CreateUserAddressCommandValidator 0
(0 1
)1 2
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line2  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
City 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Postal !
)! "
. 
NotNull 
( 
) 
; 
} 	
}   
}!! Ù 
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\CreateUserAddress\CreateUserAddressCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Users= B
.B C
CreateUserAddressC T
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class +
CreateUserAddressCommandHandler 0
:1 2
IRequestHandler3 B
<B C$
CreateUserAddressCommandC [
,[ \
Guid] a
>a b
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
CreateUserAddressCommandHandler .
(. /
IUserRepository/ >
userRepository? M
)M N
{ 	
_userRepository 
= 
userRepository ,
;, -
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '$
CreateUserAddressCommand' ?
request@ G
,G H
CancellationTokenI Z
cancellationToken[ l
)l m
{ 	
var 
user 
= 
await 
_userRepository ,
., -
FindByIdAsync- :
(: ;
request; B
.B C
UserIdC I
,I J
cancellationTokenK \
)\ ]
;] ^
if 
( 
user 
is 
null 
) 
{   
throw!! 
new!! 
NotFoundException!! +
(!!+ ,
$"!!, .
$str!!. J
{!!J K
request!!K R
.!!R S
UserId!!S Y
}!!Y Z
$str!!Z [
"!![ \
)!!\ ]
;!!] ^
}"" 
var## 
address## 
=## 
new## 
UserAddress## )
{$$ 
UserId%% 
=%% 
request%%  
.%%  !
UserId%%! '
,%%' (
Line1&& 
=&& 
request&& 
.&&  
Line1&&  %
,&&% &
Line2'' 
='' 
request'' 
.''  
Line2''  %
,''% &
City(( 
=(( 
request(( 
.(( 
City(( #
,((# $
Postal)) 
=)) 
request))  
.))  !
Postal))! '
}** 
;** 
user,, 
.,, 
	Addresses,, 
.,, 
Add,, 
(,, 
address,, &
),,& '
;,,' (
await-- 
_userRepository-- !
.--! "

UnitOfWork--" ,
.--, -
SaveChangesAsync--- =
(--= >
cancellationToken--> O
)--O P
;--P Q
return.. 
address.. 
... 
Id.. 
;.. 
}// 	
}00 
}11 ı
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Users\CreateUserAddress\CreateUserAddressCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Users		= B
.		B C
CreateUserAddress		C T
{

 
public 

class $
CreateUserAddressCommand )
:* +
IRequest, 4
<4 5
Guid5 9
>9 :
,: ;
ICommand< D
{ 
public $
CreateUserAddressCommand '
(' (
Guid( ,
userId- 3
,3 4
string5 ;
line1< A
,A B
stringC I
line2J O
,O P
stringQ W
cityX \
,\ ]
string^ d
postale k
)k l
{ 	
UserId 
= 
userId 
; 
Line1 
= 
line1 
; 
Line2 
= 
line2 
; 
City 
= 
city 
; 
Postal 
= 
postal 
; 
} 	
public 
Guid 
UserId 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Postal 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} Á
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\UpdateProductTagDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 (
UpdateProductTagDtoValidator

 -
:

. /
AbstractValidator

0 A
<

A B
UpdateProductTagDto

B U
>

U V
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
UpdateProductTagDtoValidator +
(+ ,
), -
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Value  
)  !
. 
NotNull 
( 
) 
; 
} 	
} 
} ï
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\UpdateProductTagDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{ 
public 

class 
UpdateProductTagDto $
{		 
public

 
UpdateProductTagDto

 "
(

" #
)

# $
{ 	
Name 
= 
null 
! 
; 
Value 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Value 
{ 
get !
;! "
set# &
;& '
}( )
public 
static 
UpdateProductTagDto )
Create* 0
(0 1
string1 7
name8 <
,< =
string> D
valueE J
)J K
{ 	
return 
new 
UpdateProductTagDto *
{ 
Name 
= 
name 
, 
Value 
= 
value 
} 
; 
} 	
} 
} ƒ
ùD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\TagDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Products

= E
{ 
public 

static 
class #
TagDtoMappingExtensions /
{ 
public 
static 
TagDto 
MapToTagDto (
(( )
this) -
Tag. 1
projectFrom2 =
,= >
IMapper? F
mapperG M
)M N
=> 
mapper 
. 
Map 
< 
TagDto  
>  !
(! "
projectFrom" -
)- .
;. /
public 
static 
List 
< 
TagDto !
>! "
MapToTagDtoList# 2
(2 3
this3 7
IEnumerable8 C
<C D
TagD G
>G H
projectFromI T
,T U
IMapperV ]
mapper^ d
)d e
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToTagDto) 4
(4 5
mapper5 ;
); <
)< =
.= >
ToList> D
(D E
)E F
;F G
} 
} É
åD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\TagDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Products		= E
{

 
public 

class 
TagDto 
: 
IMapFrom "
<" #
Tag# &
>& '
{ 
public 
TagDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
Value 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Value 
{ 
get !
;! "
set# &
;& '
}( )
public 
static 
TagDto 
Create #
(# $
string$ *
name+ /
,/ 0
string1 7
value8 =
)= >
{ 	
return 
new 
TagDto 
{ 
Name 
= 
name 
, 
Value 
= 
value 
} 
; 
} 	
public 
void 
Mapping 
( 
Profile #
profile$ +
)+ ,
{   	
profile!! 
.!! 
	CreateMap!! 
<!! 
Tag!! !
,!!! "
TagDto!!# )
>!!) *
(!!* +
)!!+ ,
;!!, -
}"" 	
}## 
}$$ æ
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\ProductUpdateDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class %
ProductUpdateDtoValidator *
:+ ,
AbstractValidator- >
<> ?
ProductUpdateDto? O
>O P
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
ProductUpdateDtoValidator (
(( )
IValidatorProvider) ;
provider< D
)D E
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Tags 
)  
. 
NotNull 
( 
) 
. 
ForEach 
( 
x 
=> 
x 
.  
SetValidator  ,
(, -
provider- 5
.5 6
GetValidator6 B
<B C
UpdateProductTagDtoC V
>V W
(W X
)X Y
!Y Z
)Z [
)[ \
;\ ]
} 	
} 
} ˛
ñD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\ProductUpdateDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{		 
public

 

class

 
ProductUpdateDto

 !
{ 
public 
ProductUpdateDto 
(  
)  !
{ 	
Name 
= 
null 
! 
; 
Tags 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
List 
< 
UpdateProductTagDto '
>' (
Tags) -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
public 
static 
ProductUpdateDto &
Create' -
(- .
Guid. 2
id3 5
,5 6
string7 =
name> B
,B C
ListD H
<H I
UpdateProductTagDtoI \
>\ ]
tags^ b
)b c
{ 	
return 
new 
ProductUpdateDto '
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Tags 
= 
tags 
} 
; 
} 	
} 
}   Ï
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\ProductDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Products

= E
{ 
public 

static 
class '
ProductDtoMappingExtensions 3
{ 
public 
static 

ProductDto  
MapToProductDto! 0
(0 1
this1 5
Product6 =
projectFrom> I
,I J
IMapperK R
mapperS Y
)Y Z
=> 
mapper 
. 
Map 
< 

ProductDto $
>$ %
(% &
projectFrom& 1
)1 2
;2 3
public 
static 
List 
< 

ProductDto %
>% &
MapToProductDtoList' :
(: ;
this; ?
IEnumerable@ K
<K L
ProductL S
>S T
projectFromU `
,` a
IMapperb i
mapperj p
)p q
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToProductDto) 8
(8 9
mapper9 ?
)? @
)@ A
.A B
ToListB H
(H I
)I J
;J K
} 
} Í
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\ProductDto.cs
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
$str		 <
,		< =
Version		> E
=		F G
$str		H M
)		M N
]		N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{ 
public 

class 

ProductDto 
: 
IMapFrom &
<& '
Product' .
>. /
{ 
public 

ProductDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
Tags 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
List 
< 
TagDto 
> 
Tags  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
static 

ProductDto  
Create! '
(' (
Guid( ,
id- /
,/ 0
string1 7
name8 <
,< =
List> B
<B C
TagDtoC I
>I J
tagsK O
)O P
{ 	
return 
new 

ProductDto !
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Tags 
= 
tags 
}   
;   
}!! 	
public## 
void## 
Mapping## 
(## 
Profile## #
profile##$ +
)##+ ,
{$$ 	
profile%% 
.%% 
	CreateMap%% 
<%% 
Product%% %
,%%% &

ProductDto%%' 1
>%%1 2
(%%2 3
)%%3 4
.&& 
	ForMember&& 
(&& 
d&& 
=>&& 
d&&  !
.&&! "
Tags&&" &
,&&& '
opt&&( +
=>&&, .
opt&&/ 2
.&&2 3
MapFrom&&3 :
(&&: ;
src&&; >
=>&&? A
src&&B E
.&&E F
Tags&&F J
)&&J K
)&&K L
;&&L M
}'' 	
}(( 
})) æ
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\ProductCreateDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class %
ProductCreateDtoValidator *
:+ ,
AbstractValidator- >
<> ?
ProductCreateDto? O
>O P
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
ProductCreateDtoValidator (
(( )
IValidatorProvider) ;
provider< D
)D E
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Tags 
)  
. 
NotNull 
( 
) 
. 
ForEach 
( 
x 
=> 
x 
.  
SetValidator  ,
(, -
provider- 5
.5 6
GetValidator6 B
<B C
CreateProductTagDtoC V
>V W
(W X
)X Y
!Y Z
)Z [
)[ \
;\ ]
} 	
} 
} ˚
ñD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\ProductCreateDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{ 
public		 

class		 
ProductCreateDto		 !
{

 
public 
ProductCreateDto 
(  
)  !
{ 	
Name 
= 
null 
! 
; 
Tags 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
List 
< 
CreateProductTagDto '
>' (
Tags) -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
public 
static 
ProductCreateDto &
Create' -
(- .
string. 4
name5 9
,9 :
List; ?
<? @
CreateProductTagDto@ S
>S T
tagsU Y
)Y Z
{ 	
return 
new 
ProductCreateDto '
{ 
Name 
= 
name 
, 
Tags 
= 
tags 
} 
; 
} 	
} 
} Á
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\CreateProductTagDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 (
CreateProductTagDtoValidator

 -
:

. /
AbstractValidator

0 A
<

A B
CreateProductTagDto

B U
>

U V
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
CreateProductTagDtoValidator +
(+ ,
), -
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Value  
)  !
. 
NotNull 
( 
) 
; 
} 	
} 
} ï
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Products\CreateProductTagDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Products= E
{ 
public 

class 
CreateProductTagDto $
{		 
public

 
CreateProductTagDto

 "
(

" #
)

# $
{ 	
Name 
= 
null 
! 
; 
Value 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Value 
{ 
get !
;! "
set# &
;& '
}( )
public 
static 
CreateProductTagDto )
Create* 0
(0 1
string1 7
name8 <
,< =
string> D
valueE J
)J K
{ 	
return 
new 
CreateProductTagDto *
{ 
Name 
= 
name 
, 
Value 
= 
value 
} 
; 
} 	
} 
} ô
›D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\UpdateParentWithAnemicChild\UpdateParentWithAnemicChildCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
UpdateParentWithAnemicChildV q
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 7
+UpdateParentWithAnemicChildCommandValidator

 <
:

= >
AbstractValidator

? P
<

P Q.
"UpdateParentWithAnemicChildCommand

Q s
>

s t
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 7
+UpdateParentWithAnemicChildCommandValidator :
(: ;
); <
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} é
€D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\UpdateParentWithAnemicChild\UpdateParentWithAnemicChildCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
UpdateParentWithAnemicChildV q
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
class 5
)UpdateParentWithAnemicChildCommandHandler :
:; <
IRequestHandler= L
<L M.
"UpdateParentWithAnemicChildCommandM o
>o p
{ 
private 
readonly ,
 IParentWithAnemicChildRepository 9,
 _parentWithAnemicChildRepository: Z
;Z [
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 5
)UpdateParentWithAnemicChildCommandHandler 8
(8 9,
 IParentWithAnemicChildRepository9 Y+
parentWithAnemicChildRepositoryZ y
)y z
{ 	,
 _parentWithAnemicChildRepository ,
=- .+
parentWithAnemicChildRepository/ N
;N O
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !.
"UpdateParentWithAnemicChildCommand! C
requestD K
,K L
CancellationTokenM ^
cancellationToken_ p
)p q
{ 	
var !
parentWithAnemicChild %
=& '
await( -,
 _parentWithAnemicChildRepository. N
.N O
FindByIdAsyncO \
(\ ]
request] d
.d e
Ide g
,g h
cancellationTokeni z
)z {
;{ |
if 
( !
parentWithAnemicChild %
is& (
null) -
)- .
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. T
{T U
requestU \
.\ ]
Id] _
}_ `
$str` a
"a b
)b c
;c d
}   !
parentWithAnemicChild"" !
.""! "
Name""" &
=""' (
request"") 0
.""0 1
Name""1 5
;""5 6!
parentWithAnemicChild## !
.##! "
Surname##" )
=##* +
request##, 3
.##3 4
Surname##4 ;
;##; <
}$$ 	
}%% 
}&& Î
‘D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\UpdateParentWithAnemicChild\UpdateParentWithAnemicChildCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =$
ParentWithAnemicChildren		= U
.		U V'
UpdateParentWithAnemicChild		V q
{

 
public 

class .
"UpdateParentWithAnemicChildCommand 3
:4 5
IRequest6 >
,> ?
ICommand@ H
{ 
public .
"UpdateParentWithAnemicChildCommand 1
(1 2
string2 8
name9 =
,= >
string? E
surnameF M
,M N
GuidO S
idT V
)V W
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
Id 
= 
id 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ω
…D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\UpdateParentWithAnemicChildCommandAnemicChildrenDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
{ 
public		 

class		 ?
3UpdateParentWithAnemicChildCommandAnemicChildrenDto		 D
{

 
public ?
3UpdateParentWithAnemicChildCommandAnemicChildrenDto B
(B C
)C D
{ 	
Line1 
= 
null 
! 
; 
Line2 
= 
null 
! 
; 
City 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
static ?
3UpdateParentWithAnemicChildCommandAnemicChildrenDto I
CreateJ P
(P Q
Guid 
id 
, 
string 
line1 
, 
string 
line2 
, 
string 
city 
) 
{ 	
return 
new ?
3UpdateParentWithAnemicChildCommandAnemicChildrenDto J
{ 
Id 
= 
id 
, 
Line1   
=   
line1   
,   
Line2!! 
=!! 
line2!! 
,!! 
City"" 
="" 
city"" 
}## 
;## 
}$$ 	
}%% 
}&& §
øD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\ParentWithAnemicChildDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =$
ParentWithAnemicChildren

= U
{ 
public 

static 
class 5
)ParentWithAnemicChildDtoMappingExtensions A
{ 
public 
static $
ParentWithAnemicChildDto .)
MapToParentWithAnemicChildDto/ L
(L M
thisM Q!
ParentWithAnemicChildR g
projectFromh s
,s t
IMapperu |
mapper	} É
)
É Ñ
=> 
mapper 
. 
Map 
< $
ParentWithAnemicChildDto 2
>2 3
(3 4
projectFrom4 ?
)? @
;@ A
public 
static 
List 
< $
ParentWithAnemicChildDto 3
>3 4-
!MapToParentWithAnemicChildDtoList5 V
(V W
thisW [
IEnumerable\ g
<g h!
ParentWithAnemicChildh }
>} ~
projectFrom	 ä
,
ä ã
IMapper
å ì
mapper
î ö
)
ö õ
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( ))
MapToParentWithAnemicChildDto) F
(F G
mapperG M
)M N
)N O
.O P
ToListP V
(V W
)W X
;X Y
} 
} ¿
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\ParentWithAnemicChildDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =$
ParentWithAnemicChildren

= U
{ 
public 

class $
ParentWithAnemicChildDto )
:* +
IMapFrom, 4
<4 5!
ParentWithAnemicChild5 J
>J K
{ 
public $
ParentWithAnemicChildDto '
(' (
)( )
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static $
ParentWithAnemicChildDto .
Create/ 5
(5 6
Guid6 :
id; =
,= >
string? E
nameF J
,J K
stringL R
surnameS Z
)Z [
{ 	
return 
new $
ParentWithAnemicChildDto /
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Surname 
= 
surname !
} 
; 
}   	
public"" 
void"" 
Mapping"" 
("" 
Profile"" #
profile""$ +
)""+ ,
{## 	
profile$$ 
.$$ 
	CreateMap$$ 
<$$ !
ParentWithAnemicChild$$ 3
,$$3 4$
ParentWithAnemicChildDto$$5 M
>$$M N
($$N O
)$$O P
;$$P Q
}%% 	
}&& 
}'' Ë
€D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\GetParentWithAnemicChildren\GetParentWithAnemicChildrenQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
GetParentWithAnemicChildrenV q
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 5
)GetParentWithAnemicChildrenQueryValidator

 :
:

; <
AbstractValidator

= N
<

N O,
 GetParentWithAnemicChildrenQuery

O o
>

o p
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 5
)GetParentWithAnemicChildrenQueryValidator 8
(8 9
)9 :
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ú
ŸD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\GetParentWithAnemicChildren\GetParentWithAnemicChildrenQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
GetParentWithAnemicChildrenV q
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 3
'GetParentWithAnemicChildrenQueryHandler 8
:9 :
IRequestHandler; J
<J K,
 GetParentWithAnemicChildrenQueryK k
,k l
Listm q
<q r%
ParentWithAnemicChildDto	r ä
>
ä ã
>
ã å
{ 
private 
readonly ,
 IParentWithAnemicChildRepository 9,
 _parentWithAnemicChildRepository: Z
;Z [
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 3
'GetParentWithAnemicChildrenQueryHandler 6
(6 7,
 IParentWithAnemicChildRepository7 W+
parentWithAnemicChildRepositoryX w
,w x
IMapper 
mapper 
) 
{ 	,
 _parentWithAnemicChildRepository ,
=- .+
parentWithAnemicChildRepository/ N
;N O
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< $
ParentWithAnemicChildDto 7
>7 8
>8 9
Handle: @
(@ A,
 GetParentWithAnemicChildrenQuery ,
request- 4
,4 5
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" $
parentWithAnemicChildren"" (
="") *
await""+ 0,
 _parentWithAnemicChildRepository""1 Q
.""Q R
FindAllAsync""R ^
(""^ _
cancellationToken""_ p
)""p q
;""q r
return## $
parentWithAnemicChildren## +
.##+ ,-
!MapToParentWithAnemicChildDtoList##, M
(##M N
_mapper##N U
)##U V
;##V W
}$$ 	
}%% 
}&& π

“D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\GetParentWithAnemicChildren\GetParentWithAnemicChildrenQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =$
ParentWithAnemicChildren		= U
.		U V'
GetParentWithAnemicChildren		V q
{

 
public 

class ,
 GetParentWithAnemicChildrenQuery 1
:2 3
IRequest4 <
<< =
List= A
<A B$
ParentWithAnemicChildDtoB Z
>Z [
>[ \
,\ ]
IQuery^ d
{ 
public ,
 GetParentWithAnemicChildrenQuery /
(/ 0
)0 1
{ 	
} 	
} 
} Ó
›D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\GetParentWithAnemicChildById\GetParentWithAnemicChildByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V(
GetParentWithAnemicChildByIdV r
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 6
*GetParentWithAnemicChildByIdQueryValidator

 ;
:

< =
AbstractValidator

> O
<

O P-
!GetParentWithAnemicChildByIdQuery

P q
>

q r
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 6
*GetParentWithAnemicChildByIdQueryValidator 9
(9 :
): ;
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Á
€D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\GetParentWithAnemicChildById\GetParentWithAnemicChildByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V(
GetParentWithAnemicChildByIdV r
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 4
(GetParentWithAnemicChildByIdQueryHandler 9
:: ;
IRequestHandler< K
<K L-
!GetParentWithAnemicChildByIdQueryL m
,m n%
ParentWithAnemicChildDto	o á
>
á à
{ 
private 
readonly ,
 IParentWithAnemicChildRepository 9,
 _parentWithAnemicChildRepository: Z
;Z [
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 4
(GetParentWithAnemicChildByIdQueryHandler 7
(7 8,
 IParentWithAnemicChildRepository8 X+
parentWithAnemicChildRepositoryY x
,x y
IMapper 
mapper 
) 
{ 	,
 _parentWithAnemicChildRepository ,
=- .+
parentWithAnemicChildRepository/ N
;N O
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< $
ParentWithAnemicChildDto 2
>2 3
Handle4 :
(: ;-
!GetParentWithAnemicChildByIdQuery -
request. 5
,5 6
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" !
parentWithAnemicChild"" %
=""& '
await""( -,
 _parentWithAnemicChildRepository"". N
.""N O
FindByIdAsync""O \
(""\ ]
request""] d
.""d e
Id""e g
,""g h
cancellationToken""i z
)""z {
;""{ |
if## 
(## !
parentWithAnemicChild## %
is##& (
null##) -
)##- .
{$$ 
throw%% 
new%% 
NotFoundException%% +
(%%+ ,
$"%%, .
$str%%. T
{%%T U
request%%U \
.%%\ ]
Id%%] _
}%%_ `
$str%%` a
"%%a b
)%%b c
;%%c d
}&& 
return'' !
parentWithAnemicChild'' (
.''( ))
MapToParentWithAnemicChildDto'') F
(''F G
_mapper''G N
)''N O
;''O P
}(( 	
})) 
}** Ç
‘D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\GetParentWithAnemicChildById\GetParentWithAnemicChildByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =$
ParentWithAnemicChildren		= U
.		U V(
GetParentWithAnemicChildById		V r
{

 
public 

class -
!GetParentWithAnemicChildByIdQuery 2
:3 4
IRequest5 =
<= >$
ParentWithAnemicChildDto> V
>V W
,W X
IQueryY _
{ 
public -
!GetParentWithAnemicChildByIdQuery 0
(0 1
Guid1 5
id6 8
)8 9
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} 
›D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\DeleteParentWithAnemicChild\DeleteParentWithAnemicChildCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
DeleteParentWithAnemicChildV q
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 7
+DeleteParentWithAnemicChildCommandValidator

 <
:

= >
AbstractValidator

? P
<

P Q.
"DeleteParentWithAnemicChildCommand

Q s
>

s t
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 7
+DeleteParentWithAnemicChildCommandValidator :
(: ;
); <
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ˘
€D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\DeleteParentWithAnemicChild\DeleteParentWithAnemicChildCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
DeleteParentWithAnemicChildV q
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
class 5
)DeleteParentWithAnemicChildCommandHandler :
:; <
IRequestHandler= L
<L M.
"DeleteParentWithAnemicChildCommandM o
>o p
{ 
private 
readonly ,
 IParentWithAnemicChildRepository 9,
 _parentWithAnemicChildRepository: Z
;Z [
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 5
)DeleteParentWithAnemicChildCommandHandler 8
(8 9,
 IParentWithAnemicChildRepository9 Y+
parentWithAnemicChildRepositoryZ y
)y z
{ 	,
 _parentWithAnemicChildRepository ,
=- .+
parentWithAnemicChildRepository/ N
;N O
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !.
"DeleteParentWithAnemicChildCommand! C
requestD K
,K L
CancellationTokenM ^
cancellationToken_ p
)p q
{ 	
var !
parentWithAnemicChild %
=& '
await( -,
 _parentWithAnemicChildRepository. N
.N O
FindByIdAsyncO \
(\ ]
request] d
.d e
Ide g
,g h
cancellationTokeni z
)z {
;{ |
if 
( !
parentWithAnemicChild %
is& (
null) -
)- .
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. T
{T U
requestU \
.\ ]
Id] _
}_ `
$str` a
"a b
)b c
;c d
}   ,
 _parentWithAnemicChildRepository"" ,
."", -
Remove""- 3
(""3 4!
parentWithAnemicChild""4 I
)""I J
;""J K
}## 	
}$$ 
}%% ¡
‘D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\DeleteParentWithAnemicChild\DeleteParentWithAnemicChildCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =$
ParentWithAnemicChildren		= U
.		U V'
DeleteParentWithAnemicChild		V q
{

 
public 

class .
"DeleteParentWithAnemicChildCommand 3
:4 5
IRequest6 >
,> ?
ICommand@ H
{ 
public .
"DeleteParentWithAnemicChildCommand 1
(1 2
Guid2 6
id7 9
)9 :
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} 
›D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\CreateParentWithAnemicChild\CreateParentWithAnemicChildCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
CreateParentWithAnemicChildV q
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class 7
+CreateParentWithAnemicChildCommandValidator <
:= >
AbstractValidator? P
<P Q.
"CreateParentWithAnemicChildCommandQ s
>s t
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 7
+CreateParentWithAnemicChildCommandValidator :
(: ;
IValidatorProvider; M
providerN V
)V W
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
AnemicChildren )
)) *
. 
NotNull 
( 
) 
. 
ForEach 
( 
x 
=> 
x 
.  
SetValidator  ,
(, -
provider- 5
.5 6
GetValidator6 B
<B C5
)CreateParentWithAnemicChildAnemicChildDtoC l
>l m
(m n
)n o
!o p
)p q
)q r
;r s
} 	
} 
}   ô 
€D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\CreateParentWithAnemicChild\CreateParentWithAnemicChildCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
.U V'
CreateParentWithAnemicChildV q
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 5
)CreateParentWithAnemicChildCommandHandler :
:; <
IRequestHandler= L
<L M.
"CreateParentWithAnemicChildCommandM o
,o p
Guidq u
>u v
{ 
private 
readonly ,
 IParentWithAnemicChildRepository 9,
 _parentWithAnemicChildRepository: Z
;Z [
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 5
)CreateParentWithAnemicChildCommandHandler 8
(8 9,
 IParentWithAnemicChildRepository9 Y+
parentWithAnemicChildRepositoryZ y
)y z
{ 	,
 _parentWithAnemicChildRepository ,
=- .+
parentWithAnemicChildRepository/ N
;N O
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '.
"CreateParentWithAnemicChildCommand' I
requestJ Q
,Q R
CancellationTokenS d
cancellationTokene v
)v w
{ 	
var !
parentWithAnemicChild %
=& '
new( +!
ParentWithAnemicChild, A
(A B
name 
: 
request 
. 
Name "
," #
surname 
: 
request  
.  !
Surname! (
,( )
anemicChildren   
:   
request    '
.  ' (
AnemicChildren  ( 6
.!! 
Select!! 
(!! 
c!! 
=>!!  
new!!! $
AnemicChild!!% 0
{"" 
Line1## 
=## 
c##  !
.##! "
Line1##" '
,##' (
Line2$$ 
=$$ 
c$$  !
.$$! "
Line2$$" '
,$$' (
City%% 
=%% 
c%%  
.%%  !
City%%! %
}&& 
)&& 
.'' 
ToList'' 
('' 
)'' 
)'' 
;'' ,
 _parentWithAnemicChildRepository)) ,
.)), -
Add))- 0
())0 1!
parentWithAnemicChild))1 F
)))F G
;))G H
await** ,
 _parentWithAnemicChildRepository** 2
.**2 3

UnitOfWork**3 =
.**= >
SaveChangesAsync**> N
(**N O
cancellationToken**O `
)**` a
;**a b
return++ !
parentWithAnemicChild++ (
.++( )
Id++) +
;+++ ,
},, 	
}-- 
}.. ı
‘D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\CreateParentWithAnemicChild\CreateParentWithAnemicChildCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =$
ParentWithAnemicChildren

= U
.

U V'
CreateParentWithAnemicChild

V q
{ 
public 

class .
"CreateParentWithAnemicChildCommand 3
:4 5
IRequest6 >
<> ?
Guid? C
>C D
,D E
ICommandF N
{ 
public .
"CreateParentWithAnemicChildCommand 1
(1 2
string2 8
name9 =
,= >
string 
surname 
, 
List 
< 5
)CreateParentWithAnemicChildAnemicChildDto :
>: ;
anemicChildren< J
)J K
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
AnemicChildren 
= 
anemicChildren +
;+ ,
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
List 
< 5
)CreateParentWithAnemicChildAnemicChildDto =
>= >
AnemicChildren? M
{N O
getP S
;S T
setU X
;X Y
}Z [
} 
} ∂
»D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\CreateParentWithAnemicChildAnemicChildDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 >
2CreateParentWithAnemicChildAnemicChildDtoValidator

 C
:

D E
AbstractValidator

F W
<

W X6
)CreateParentWithAnemicChildAnemicChildDto	

X Å
>


Å Ç
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public >
2CreateParentWithAnemicChildAnemicChildDtoValidator A
(A B
)B C
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line2  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
City 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} à
øD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ParentWithAnemicChildren\CreateParentWithAnemicChildAnemicChildDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =$
ParentWithAnemicChildren= U
{ 
public 

class 5
)CreateParentWithAnemicChildAnemicChildDto :
{		 
public

 5
)CreateParentWithAnemicChildAnemicChildDto

 8
(

8 9
)

9 :
{ 	
Line1 
= 
null 
! 
; 
Line2 
= 
null 
! 
; 
City 
= 
null 
! 
; 
} 	
public 
string 
Line1 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Line2 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
City 
{ 
get  
;  !
set" %
;% &
}' (
public 
static 5
)CreateParentWithAnemicChildAnemicChildDto ?
Create@ F
(F G
stringG M
line1N S
,S T
stringU [
line2\ a
,a b
stringc i
cityj n
)n o
{ 	
return 
new 5
)CreateParentWithAnemicChildAnemicChildDto @
{ 
Line1 
= 
line1 
, 
Line2 
= 
line2 
, 
City 
= 
city 
} 
; 
} 	
} 
} ã
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\PagingTS\PagingTSUpdateDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
PagingTS= E
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
PagingTSUpdateDtoValidator

 +
:

, -
AbstractValidator

. ?
<

? @
PagingTSUpdateDto

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
PagingTSUpdateDtoValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} ©
óD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\PagingTS\PagingTSUpdateDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
PagingTS= E
{ 
public		 

class		 
PagingTSUpdateDto		 "
{

 
public 
PagingTSUpdateDto  
(  !
)! "
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
static 
PagingTSUpdateDto '
Create( .
(. /
Guid/ 3
id4 6
,6 7
string8 >
name? C
)C D
{ 	
return 
new 
PagingTSUpdateDto (
{ 
Id 
= 
id 
, 
Name 
= 
name 
} 
; 
} 	
} 
} â
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\PagingTS\PagingTSDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
PagingTS

= E
{ 
public 

static 
class (
PagingTSDtoMappingExtensions 4
{ 
public 
static 
PagingTSDto !
MapToPagingTSDto" 2
(2 3
this3 7
Domain8 >
.> ?
Entities? G
.G H
PagingTSH P
projectFromQ \
,\ ]
IMapper^ e
mapperf l
)l m
=> 
mapper 
. 
Map 
< 
PagingTSDto %
>% &
(& '
projectFrom' 2
)2 3
;3 4
public 
static 
List 
< 
PagingTSDto &
>& ' 
MapToPagingTSDtoList( <
(< =
this= A
IEnumerableB M
<M N
DomainN T
.T U
EntitiesU ]
.] ^
PagingTS^ f
>f g
projectFromh s
,s t
IMapperu |
mapper	} É
)
É Ñ
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToPagingTSDto) 9
(9 :
mapper: @
)@ A
)A B
.B C
ToListC I
(I J
)J K
;K L
} 
} Ÿ
ëD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\PagingTS\PagingTSDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
PagingTS

= E
{ 
public 

class 
PagingTSDto 
: 
IMapFrom '
<' (
Domain( .
.. /
Entities/ 7
.7 8
PagingTS8 @
>@ A
{ 
public 
PagingTSDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
static 
PagingTSDto !
Create" (
(( )
string) /
name0 4
,4 5
Guid6 :
id; =
)= >
{ 	
return 
new 
PagingTSDto "
{ 
Name 
= 
name 
, 
Id 
= 
id 
} 
; 
} 	
public 
void 
Mapping 
( 
Profile #
profile$ +
)+ ,
{   	
profile!! 
.!! 
	CreateMap!! 
<!! 
Domain!! $
.!!$ %
Entities!!% -
.!!- .
PagingTS!!. 6
,!!6 7
PagingTSDto!!8 C
>!!C D
(!!D E
)!!E F
;!!F G
}"" 	
}## 
}$$ ã
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\PagingTS\PagingTSCreateDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
PagingTS= E
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
PagingTSCreateDtoValidator

 +
:

, -
AbstractValidator

. ?
<

? @
PagingTSCreateDto

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
PagingTSCreateDtoValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} ¶
óD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\PagingTS\PagingTSCreateDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
PagingTS= E
{ 
public 

class 
PagingTSCreateDto "
{		 
public

 
PagingTSCreateDto

  
(

  !
)

! "
{ 	
Name 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
static 
PagingTSCreateDto '
Create( .
(. /
string/ 5
name6 :
): ;
{ 	
return 
new 
PagingTSCreateDto (
{ 
Name 
= 
name 
} 
; 
} 	
} 
} ›
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\UpdateOrder\UpdateOrderCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
UpdateOrderD O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
UpdateOrderCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
UpdateOrderCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
UpdateOrderCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
RefNo  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
OrderStatus &
)& '
. 
NotNull 
( 
) 
. 
IsInEnum 
( 
) 
; 
} 	
} 
} ˙
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\UpdateOrder\UpdateOrderCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
UpdateOrderD O
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
class %
UpdateOrderCommandHandler *
:+ ,
IRequestHandler- <
<< =
UpdateOrderCommand= O
>O P
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
UpdateOrderCommandHandler (
(( )
IOrderRepository) 9
orderRepository: I
)I J
{ 	
_orderRepository 
= 
orderRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
UpdateOrderCommand! 3
request4 ;
,; <
CancellationToken= N
cancellationTokenO `
)` a
{ 	
var 
order 
= 
await 
_orderRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
if 
( 
order 
is 
null 
) 
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. D
{D E
requestE L
.L M
IdM O
}O P
$strP Q
"Q R
)R S
;S T
}   
order"" 
."" 
RefNo"" 
="" 
request"" !
.""! "
RefNo""" '
;""' (
order## 
.## 
	OrderDate## 
=## 
request## %
.##% &
	OrderDate##& /
;##/ 0
order$$ 
.$$ 
OrderStatus$$ 
=$$ 
request$$  '
.$$' (
OrderStatus$$( 3
;$$3 4
order%% 
.%% 

CustomerId%% 
=%% 
request%% &
.%%& '

CustomerId%%' 1
;%%1 2
}&& 	
}'' 
}(( ﬂ
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\UpdateOrder\UpdateOrderCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
Orders

= C
.

C D
UpdateOrder

D O
{ 
public 

class 
UpdateOrderCommand #
:$ %
IRequest& .
,. /
ICommand0 8
{ 
public 
UpdateOrderCommand !
(! "
Guid" &
id' )
,) *
string+ 1
refNo2 7
,7 8
DateTime9 A
	orderDateB K
,K L
OrderStatusM X
orderStatusY d
,d e
Guidf j

customerIdk u
)u v
{ 	
Id 
= 
id 
; 
RefNo 
= 
refNo 
; 
	OrderDate 
= 
	orderDate !
;! "
OrderStatus 
= 
orderStatus %
;% &

CustomerId 
= 

customerId #
;# $
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
	OrderDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
OrderStatus 
OrderStatus &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
Guid 

CustomerId 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} ¢
ΩD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\UpdateOrderOrderItem\UpdateOrderOrderItemCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D 
UpdateOrderOrderItemD X
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 0
$UpdateOrderOrderItemCommandValidator

 5
:

6 7
AbstractValidator

8 I
<

I J'
UpdateOrderOrderItemCommand

J e
>

e f
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 0
$UpdateOrderOrderItemCommandValidator 3
(3 4
)4 5
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} û!
ªD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\UpdateOrderOrderItem\UpdateOrderOrderItemCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D 
UpdateOrderOrderItemD X
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class .
"UpdateOrderOrderItemCommandHandler 3
:4 5
IRequestHandler6 E
<E F'
UpdateOrderOrderItemCommandF a
>a b
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"UpdateOrderOrderItemCommandHandler 1
(1 2
IOrderRepository2 B
orderRepositoryC R
)R S
{ 	
_orderRepository 
= 
orderRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !'
UpdateOrderOrderItemCommand! <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
var 
order 
= 
await 
_orderRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
OrderIdE L
,L M
cancellationTokenN _
)_ `
;` a
if 
( 
order 
is 
null 
) 
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . H
{  H I
request  I P
.  P Q
OrderId  Q X
}  X Y
$str  Y Z
"  Z [
)  [ \
;  \ ]
}!! 
var## 
	orderItem## 
=## 
order## !
.##! "

OrderItems##" ,
.##, -
FirstOrDefault##- ;
(##; <
x##< =
=>##> @
x##A B
.##B C
Id##C E
==##F H
request##I P
.##P Q
Id##Q S
)##S T
;##T U
if$$ 
($$ 
	orderItem$$ 
is$$ 
null$$ !
)$$! "
{%% 
throw&& 
new&& 
NotFoundException&& +
(&&+ ,
$"&&, .
$str&&. H
{&&H I
request&&I P
.&&P Q
Id&&Q S
}&&S T
$str&&T U
"&&U V
)&&V W
;&&W X
}'' 
	orderItem)) 
.)) 
Quantity)) 
=))  
request))! (
.))( )
Quantity))) 1
;))1 2
	orderItem** 
.** 
	UnitPrice** 
=**  !
request**" )
.**) *
Amount*** 0
;**0 1
	orderItem++ 
.++ 
	ProductId++ 
=++  !
request++" )
.++) *
	ProductId++* 3
;++3 4
},, 	
}-- 
}.. ‚
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\UpdateOrderOrderItem\UpdateOrderOrderItemCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Orders		= C
.		C D 
UpdateOrderOrderItem		D X
{

 
public 

class '
UpdateOrderOrderItemCommand ,
:- .
IRequest/ 7
,7 8
ICommand9 A
{ 
public '
UpdateOrderOrderItemCommand *
(* +
Guid+ /
orderId0 7
,7 8
Guid9 =
id> @
,@ A
intB E
quantityF N
,N O
decimalP W
amountX ^
,^ _
Guid` d
	productIde n
)n o
{ 	
OrderId 
= 
orderId 
; 
Id 
= 
id 
; 
Quantity 
= 
quantity 
;  
Amount 
= 
amount 
; 
	ProductId 
= 
	productId !
;! "
} 	
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
int 
Quantity 
{ 
get !
;! "
set# &
;& '
}( )
public 
decimal 
Amount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} ß
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\OrderOrderItemDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Orders

= C
{ 
public 

static 
class .
"OrderOrderItemDtoMappingExtensions :
{ 
public 
static 
OrderOrderItemDto '"
MapToOrderOrderItemDto( >
(> ?
this? C
	OrderItemD M
projectFromN Y
,Y Z
IMapper[ b
mapperc i
)i j
=> 
mapper 
. 
Map 
< 
OrderOrderItemDto +
>+ ,
(, -
projectFrom- 8
)8 9
;9 :
public 
static 
List 
< 
OrderOrderItemDto ,
>, -&
MapToOrderOrderItemDtoList. H
(H I
thisI M
IEnumerableN Y
<Y Z
	OrderItemZ c
>c d
projectFrome p
,p q
IMapperr y
mapper	z Ä
)
Ä Å
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )"
MapToOrderOrderItemDto) ?
(? @
mapper@ F
)F G
)G H
.H I
ToListI O
(O P
)P Q
;Q R
} 
} „
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\OrderOrderItemDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
Orders

= C
{ 
public 

class 
OrderOrderItemDto "
:# $
IMapFrom% -
<- .
	OrderItem. 7
>7 8
{ 
public 
OrderOrderItemDto  
(  !
)! "
{ 	
} 	
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
int 
Quantity 
{ 
get !
;! "
set# &
;& '
}( )
public 
decimal 
Amount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static 
OrderOrderItemDto '
Create( .
(. /
Guid/ 3
orderId4 ;
,; <
Guid= A
idB D
,D E
intF I
quantityJ R
,R S
decimalT [
amount\ b
,b c
Guidd h
	productIdi r
)r s
{ 	
return 
new 
OrderOrderItemDto (
{ 
OrderId 
= 
orderId !
,! "
Id 
= 
id 
, 
Quantity 
= 
quantity #
,# $
Amount 
= 
amount 
,  
	ProductId   
=   
	productId   %
}!! 
;!! 
}"" 	
public$$ 
void$$ 
Mapping$$ 
($$ 
Profile$$ #
profile$$$ +
)$$+ ,
{%% 	
profile&& 
.&& 
	CreateMap&& 
<&& 
	OrderItem&& '
,&&' (
OrderOrderItemDto&&) :
>&&: ;
(&&; <
)&&< =
.'' 
	ForMember'' 
('' 
d'' 
=>'' 
d''  !
.''! "
Amount''" (
,''( )
opt''* -
=>''. 0
opt''1 4
.''4 5
MapFrom''5 <
(''< =
src''= @
=>''A C
src''D G
.''G H
	UnitPrice''H Q
)''Q R
)''R S
;''S T
}(( 	
})) 
}** ‘
ùD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\OrderDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Orders

= C
{ 
public 

static 
class %
OrderDtoMappingExtensions 1
{ 
public 
static 
OrderDto 
MapToOrderDto ,
(, -
this- 1
Order2 7
projectFrom8 C
,C D
IMapperE L
mapperM S
)S T
=> 
mapper 
. 
Map 
< 
OrderDto "
>" #
(# $
projectFrom$ /
)/ 0
;0 1
public 
static 
List 
< 
OrderDto #
># $
MapToOrderDtoList% 6
(6 7
this7 ;
IEnumerable< G
<G H
OrderH M
>M N
projectFromO Z
,Z [
IMapper\ c
mapperd j
)j k
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToOrderDto) 6
(6 7
mapper7 =
)= >
)> ?
.? @
ToList@ F
(F G
)G H
;H I
} 
} ±
åD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\OrderDto.cs
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
$str		 <
,		< =
Version		> E
=		F G
$str		H M
)		M N
]		N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
{ 
public 

class 
OrderDto 
: 
IMapFrom $
<$ %
Order% *
>* +
{ 
public 
OrderDto 
( 
) 
{ 	
RefNo 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
	OrderDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
OrderStatus 
OrderStatus &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
Guid 

CustomerId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
static 
OrderDto 
Create %
(% &
Guid& *
id+ -
,- .
string/ 5
refNo6 ;
,; <
DateTime= E
	orderDateF O
,O P
OrderStatusQ \
orderStatus] h
,h i
Guidj n

customerIdo y
)y z
{ 	
return 
new 
OrderDto 
{ 
Id 
= 
id 
, 
RefNo 
= 
refNo 
, 
	OrderDate   
=   
	orderDate   %
,  % &
OrderStatus!! 
=!! 
orderStatus!! )
,!!) *

CustomerId"" 
="" 

customerId"" '
}## 
;## 
}$$ 	
public&& 
void&& 
Mapping&& 
(&& 
Profile&& #
profile&&$ +
)&&+ ,
{'' 	
profile(( 
.(( 
	CreateMap(( 
<(( 
Order(( #
,((# $
OrderDto((% -
>((- .
(((. /
)((/ 0
;((0 1
})) 	
}** 
}++ é
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrdersPaginated\GetOrdersPaginatedQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
GetOrdersPaginatedD V
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 ,
 GetOrdersPaginatedQueryValidator

 1
:

2 3
AbstractValidator

4 E
<

E F#
GetOrdersPaginatedQuery

F ]
>

] ^
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 GetOrdersPaginatedQueryValidator /
(/ 0
)0 1
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ÿ
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrdersPaginated\GetOrdersPaginatedQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
GetOrdersPaginatedD V
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class *
GetOrdersPaginatedQueryHandler /
:0 1
IRequestHandler2 A
<A B#
GetOrdersPaginatedQueryB Y
,Y Z
PagedResult[ f
<f g
OrderDtog o
>o p
>p q
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
GetOrdersPaginatedQueryHandler -
(- .
IOrderRepository. >
orderRepository? N
,N O
IMapperP W
mapperX ^
)^ _
{ 	
_orderRepository 
= 
orderRepository .
;. /
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
PagedResult %
<% &
OrderDto& .
>. /
>/ 0
Handle1 7
(7 8#
GetOrdersPaginatedQuery #
request$ +
,+ ,
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" 
orders"" 
="" 
await"" 
_orderRepository"" /
.""/ 0
FindAllAsync""0 <
(""< =
request""= D
.""D E
PageNo""E K
,""K L
request""M T
.""T U
PageSize""U ]
,""] ^
cancellationToken""_ p
)""p q
;""q r
return## 
orders## 
.## 
MapToPagedResult## *
(##* +
x##+ ,
=>##- /
x##0 1
.##1 2
MapToOrderDto##2 ?
(##? @
_mapper##@ G
)##G H
)##H I
;##I J
}$$ 	
}%% 
}&& ˙
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrdersPaginated\GetOrdersPaginatedQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
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
0 1
Application

1 <
.

< =
Orders

= C
.

C D
GetOrdersPaginated

D V
{ 
public 

class #
GetOrdersPaginatedQuery (
:) *
IRequest+ 3
<3 4
PagedResult4 ?
<? @
OrderDto@ H
>H I
>I J
,J K
IQueryL R
{ 
public #
GetOrdersPaginatedQuery &
(& '
int' *
pageNo+ 1
,1 2
int3 6
pageSize7 ?
)? @
{ 	
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
} 	
public 
int 
PageNo 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
} 
} é
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderOrderItems\GetOrderOrderItemsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
GetOrderOrderItemsD V
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 ,
 GetOrderOrderItemsQueryValidator

 1
:

2 3
AbstractValidator

4 E
<

E F#
GetOrderOrderItemsQuery

F ]
>

] ^
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 GetOrderOrderItemsQueryValidator /
(/ 0
)0 1
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} î
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderOrderItems\GetOrderOrderItemsQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
GetOrderOrderItemsD V
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class *
GetOrderOrderItemsQueryHandler /
:0 1
IRequestHandler2 A
<A B#
GetOrderOrderItemsQueryB Y
,Y Z
List[ _
<_ `
OrderOrderItemDto` q
>q r
>r s
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
GetOrderOrderItemsQueryHandler -
(- .
IOrderRepository. >
orderRepository? N
,N O
IMapperP W
mapperX ^
)^ _
{ 	
_orderRepository 
= 
orderRepository .
;. /
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
OrderOrderItemDto 0
>0 1
>1 2
Handle3 9
(9 :#
GetOrderOrderItemsQuery   #
request  $ +
,  + ,
CancellationToken!! 
cancellationToken!! /
)!!/ 0
{"" 	
var## 
order## 
=## 
await## 
_orderRepository## .
.##. /
FindByIdAsync##/ <
(##< =
request##= D
.##D E
OrderId##E L
,##L M
cancellationToken##N _
)##_ `
;##` a
if$$ 
($$ 
order$$ 
is$$ 
null$$ 
)$$ 
{%% 
throw&& 
new&& 
NotFoundException&& +
(&&+ ,
$"&&, .
$str&&. H
{&&H I
request&&I P
.&&P Q
OrderId&&Q X
}&&X Y
$str&&Y Z
"&&Z [
)&&[ \
;&&\ ]
}'' 
var)) 

orderItems)) 
=)) 
order)) "
.))" #

OrderItems))# -
.))- .
Where)). 3
())3 4
x))4 5
=>))6 8
x))9 :
.)): ;
OrderId)); B
==))C E
request))F M
.))M N
OrderId))N U
)))U V
;))V W
return** 

orderItems** 
.** &
MapToOrderOrderItemDtoList** 8
(**8 9
_mapper**9 @
)**@ A
;**A B
}++ 	
},, 
}-- È
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderOrderItems\GetOrderOrderItemsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
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
0 1
Application

1 <
.

< =
Orders

= C
.

C D
GetOrderOrderItems

D V
{ 
public 

class #
GetOrderOrderItemsQuery (
:) *
IRequest+ 3
<3 4
List4 8
<8 9
OrderOrderItemDto9 J
>J K
>K L
,L M
IQueryN T
{ 
public #
GetOrderOrderItemsQuery &
(& '
Guid' +
orderId, 3
)3 4
{ 	
OrderId 
= 
orderId 
; 
} 	
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
} 
} †
ΩD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderOrderItemById\GetOrderOrderItemByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D!
GetOrderOrderItemByIdD Y
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#GetOrderOrderItemByIdQueryValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
GetOrderOrderItemByIdQuery

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#GetOrderOrderItemByIdQueryValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ¡#
ªD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderOrderItemById\GetOrderOrderItemByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D!
GetOrderOrderItemByIdD Y
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class -
!GetOrderOrderItemByIdQueryHandler 2
:3 4
IRequestHandler5 D
<D E&
GetOrderOrderItemByIdQueryE _
,_ `
OrderOrderItemDtoa r
>r s
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!GetOrderOrderItemByIdQueryHandler 0
(0 1
IOrderRepository1 A
orderRepositoryB Q
,Q R
IMapperS Z
mapper[ a
)a b
{ 	
_orderRepository 
= 
orderRepository .
;. /
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
OrderOrderItemDto +
>+ ,
Handle- 3
(3 4&
GetOrderOrderItemByIdQuery &
request' .
,. /
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" 
order"" 
="" 
await"" 
_orderRepository"" .
."". /
FindByIdAsync""/ <
(""< =
request""= D
.""D E
OrderId""E L
,""L M
cancellationToken""N _
)""_ `
;""` a
if## 
(## 
order## 
is## 
null## 
)## 
{$$ 
throw%% 
new%% 
NotFoundException%% +
(%%+ ,
$"%%, .
$str%%. H
{%%H I
request%%I P
.%%P Q
OrderId%%Q X
}%%X Y
$str%%Y Z
"%%Z [
)%%[ \
;%%\ ]
}&& 
var(( 
	orderItem(( 
=(( 
order(( !
.((! "

OrderItems((" ,
.((, -
FirstOrDefault((- ;
(((; <
x((< =
=>((> @
x((A B
.((B C
OrderId((C J
==((K M
request((N U
.((U V
OrderId((V ]
&&((^ `
x((a b
.((b c
Id((c e
==((f h
request((i p
.((p q
Id((q s
)((s t
;((t u
if)) 
()) 
	orderItem)) 
is)) 
null)) !
)))! "
{** 
throw++ 
new++ 
NotFoundException++ +
(+++ ,
$"++, .
$str++. I
{++I J
request++J Q
.++Q R
OrderId++R Y
}++Y Z
$str++Z \
{++\ ]
request++] d
.++d e
Id++e g
}++g h
$str++h j
"++j k
)++k l
;++l m
},, 
return-- 
	orderItem-- 
.-- "
MapToOrderOrderItemDto-- 3
(--3 4
_mapper--4 ;
)--; <
;--< =
}.. 	
}// 
}00 À
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderOrderItemById\GetOrderOrderItemByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Orders		= C
.		C D!
GetOrderOrderItemById		D Y
{

 
public 

class &
GetOrderOrderItemByIdQuery +
:, -
IRequest. 6
<6 7
OrderOrderItemDto7 H
>H I
,I J
IQueryK Q
{ 
public &
GetOrderOrderItemByIdQuery )
() *
Guid* .
orderId/ 6
,6 7
Guid8 <
id= ?
)? @
{ 	
OrderId 
= 
orderId 
; 
Id 
= 
id 
; 
} 	
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Í
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderById\GetOrderByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
GetOrderByIdD P
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
GetOrderByIdQueryValidator

 +
:

, -
AbstractValidator

. ?
<

? @
GetOrderByIdQuery

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
GetOrderByIdQueryValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ä
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderById\GetOrderByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
GetOrderByIdD P
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class $
GetOrderByIdQueryHandler )
:* +
IRequestHandler, ;
<; <
GetOrderByIdQuery< M
,M N
OrderDtoO W
>W X
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
GetOrderByIdQueryHandler '
(' (
IOrderRepository( 8
orderRepository9 H
,H I
IMapperJ Q
mapperR X
)X Y
{ 	
_orderRepository 
= 
orderRepository .
;. /
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
OrderDto "
>" #
Handle$ *
(* +
GetOrderByIdQuery+ <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
var 
order 
= 
await 
_orderRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
if   
(   
order   
is   
null   
)   
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". D
{""D E
request""E L
.""L M
Id""M O
}""O P
$str""P Q
"""Q R
)""R S
;""S T
}## 
return$$ 
order$$ 
.$$ 
MapToOrderDto$$ &
($$& '
_mapper$$' .
)$$. /
;$$/ 0
}%% 	
}&& 
}'' ˛

¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\GetOrderById\GetOrderByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Orders		= C
.		C D
GetOrderById		D P
{

 
public 

class 
GetOrderByIdQuery "
:# $
IRequest% -
<- .
OrderDto. 6
>6 7
,7 8
IQuery9 ?
{ 
public 
GetOrderByIdQuery  
(  !
Guid! %
id& (
)( )
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ï
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\DeleteOrder\DeleteOrderCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
DeleteOrderD O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
DeleteOrderCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
DeleteOrderCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
DeleteOrderCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} µ
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\DeleteOrder\DeleteOrderCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
DeleteOrderD O
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
class %
DeleteOrderCommandHandler *
:+ ,
IRequestHandler- <
<< =
DeleteOrderCommand= O
>O P
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
DeleteOrderCommandHandler (
(( )
IOrderRepository) 9
orderRepository: I
)I J
{ 	
_orderRepository 
= 
orderRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
DeleteOrderCommand! 3
request4 ;
,; <
CancellationToken= N
cancellationTokenO `
)` a
{ 	
var 
order 
= 
await 
_orderRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
if 
( 
order 
is 
null 
) 
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. D
{D E
requestE L
.L M
IdM O
}O P
$strP Q
"Q R
)R S
;S T
}   
_orderRepository"" 
."" 
Remove"" #
(""# $
order""$ )
)"") *
;""* +
}## 	
}$$ 
}%% Õ

¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\DeleteOrder\DeleteOrderCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Orders		= C
.		C D
DeleteOrder		D O
{

 
public 

class 
DeleteOrderCommand #
:$ %
IRequest& .
,. /
ICommand0 8
{ 
public 
DeleteOrderCommand !
(! "
Guid" &
id' )
)) *
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ¢
ΩD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\DeleteOrderOrderItem\DeleteOrderOrderItemCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D 
DeleteOrderOrderItemD X
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 0
$DeleteOrderOrderItemCommandValidator

 5
:

6 7
AbstractValidator

8 I
<

I J'
DeleteOrderOrderItemCommand

J e
>

e f
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 0
$DeleteOrderOrderItemCommandValidator 3
(3 4
)4 5
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ˜ 
ªD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\DeleteOrderOrderItem\DeleteOrderOrderItemCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D 
DeleteOrderOrderItemD X
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class .
"DeleteOrderOrderItemCommandHandler 3
:4 5
IRequestHandler6 E
<E F'
DeleteOrderOrderItemCommandF a
>a b
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"DeleteOrderOrderItemCommandHandler 1
(1 2
IOrderRepository2 B
orderRepositoryC R
)R S
{ 	
_orderRepository 
= 
orderRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !'
DeleteOrderOrderItemCommand! <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
var 
order 
= 
await 
_orderRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
OrderIdE L
,L M
cancellationTokenN _
)_ `
;` a
if 
( 
order 
is 
null 
) 
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . H
{  H I
request  I P
.  P Q
OrderId  Q X
}  X Y
$str  Y Z
"  Z [
)  [ \
;  \ ]
}!! 
var## 
	orderItem## 
=## 
order## !
.##! "

OrderItems##" ,
.##, -
FirstOrDefault##- ;
(##; <
x##< =
=>##> @
x##A B
.##B C
Id##C E
==##F H
request##I P
.##P Q
Id##Q S
&&##T V
x##W X
.##X Y
OrderId##Y `
==##a c
request##d k
.##k l
OrderId##l s
)##s t
;##t u
if$$ 
($$ 
	orderItem$$ 
is$$ 
null$$ !
)$$! "
{%% 
throw&& 
new&& 
NotFoundException&& +
(&&+ ,
$"&&, .
$str&&. I
{&&I J
request&&J Q
.&&Q R
Id&&R T
}&&T U
$str&&U W
{&&W X
request&&X _
.&&_ `
OrderId&&` g
}&&g h
$str&&h j
"&&j k
)&&k l
;&&l m
}'' 
order)) 
.)) 

OrderItems)) 
.)) 
Remove)) #
())# $
	orderItem))$ -
)))- .
;)). /
}** 	
}++ 
},, ë
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\DeleteOrderOrderItem\DeleteOrderOrderItemCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Orders		= C
.		C D 
DeleteOrderOrderItem		D X
{

 
public 

class '
DeleteOrderOrderItemCommand ,
:- .
IRequest/ 7
,7 8
ICommand9 A
{ 
public '
DeleteOrderOrderItemCommand *
(* +
Guid+ /
orderId0 7
,7 8
Guid9 =
id> @
)@ A
{ 	
OrderId 
= 
orderId 
; 
Id 
= 
id 
; 
} 	
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ü
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrder\CreateOrderCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
CreateOrderD O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
CreateOrderCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
CreateOrderCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
CreateOrderCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
RefNo  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
OrderStatus &
)& '
. 
NotNull 
( 
) 
. 
IsInEnum 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 

OrderItems %
)% &
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Line1  
)  !
. 
NotNull 
( 
) 
; 
RuleFor!! 
(!! 
v!! 
=>!! 
v!! 
.!! 
Line2!!  
)!!  !
."" 
NotNull"" 
("" 
)"" 
;"" 
RuleFor$$ 
($$ 
v$$ 
=>$$ 
v$$ 
.$$ 
City$$ 
)$$  
.%% 
NotNull%% 
(%% 
)%% 
;%% 
RuleFor'' 
('' 
v'' 
=>'' 
v'' 
.'' 
Postal'' !
)''! "
.(( 
NotNull(( 
((( 
)(( 
;(( 
})) 	
}** 
}++ ﬂ'
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrder\CreateOrderCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
CreateOrderD O
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class %
CreateOrderCommandHandler *
:+ ,
IRequestHandler- <
<< =
CreateOrderCommand= O
,O P
GuidQ U
>U V
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
CreateOrderCommandHandler (
(( )
IOrderRepository) 9
orderRepository: I
)I J
{ 	
_orderRepository 
= 
orderRepository .
;. /
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '
CreateOrderCommand' 9
request: A
,A B
CancellationTokenC T
cancellationTokenU f
)f g
{ 	
var 
order 
= 
new 
Order !
{ 
RefNo   
=   
request   
.    
RefNo    %
,  % &
	OrderDate!! 
=!! 
request!! #
.!!# $
	OrderDate!!$ -
,!!- .
OrderStatus"" 
="" 
request"" %
.""% &
OrderStatus""& 1
,""1 2

CustomerId## 
=## 
request## $
.##$ %

CustomerId##% /
,##/ 0

OrderItems$$ 
=$$ 
request$$ $
.$$$ %

OrderItems$$% /
.%% 
Select%% 
(%% 
oi%% 
=>%% !
new%%" %
	OrderItem%%& /
{&& 
Quantity''  
=''! "
oi''# %
.''% &
Quantity''& .
,''. /
	UnitPrice(( !
=((" #
oi(($ &
.((& '
Amount((' -
,((- .
	ProductId)) !
=))" #
oi))$ &
.))& '
	ProductId))' 0
}** 
)** 
.++ 
ToList++ 
(++ 
)++ 
,++ 
DeliveryAddress,, 
=,,  !
new,," %
Address,,& -
(,,- .
line1-- 
:-- 
request-- "
.--" #
Line1--# (
,--( )
line2.. 
:.. 
request.. "
..." #
Line2..# (
,..( )
city// 
:// 
request// !
.//! "
City//" &
,//& '
postal00 
:00 
request00 #
.00# $
Postal00$ *
)00* +
,00+ ,
BillingAddress11 
=11  
new11! $
Address11% ,
(11, -
line122 
:22 
request22 "
.22" #
Line122# (
,22( )
line233 
:33 
request33 "
.33" #
Line233# (
,33( )
city44 
:44 
request44 !
.44! "
City44" &
,44& '
postal55 
:55 
request55 #
.55# $
Postal55$ *
)55* +
}66 
;66 
_orderRepository88 
.88 
Add88  
(88  !
order88! &
)88& '
;88' (
await99 
_orderRepository99 "
.99" #

UnitOfWork99# -
.99- .
SaveChangesAsync99. >
(99> ?
cancellationToken99? P
)99P Q
;99Q R
return:: 
order:: 
.:: 
Id:: 
;:: 
};; 	
}<< 
}== ë
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrder\CreateOrderCommand.cs
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
$str		 D
,		D E
Version		F M
=		N O
$str		P U
)		U V
]		V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D
CreateOrderD O
{ 
public 

class 
CreateOrderCommand #
:$ %
IRequest& .
<. /
Guid/ 3
>3 4
,4 5
ICommand6 >
{ 
public 
CreateOrderCommand !
(! "
string" (
refNo) .
,. /
DateTime 
	orderDate 
, 
OrderStatus 
orderStatus #
,# $
Guid 

customerId 
, 
List 
< +
CreateOrderCommandOrderItemsDto 0
>0 1

orderItems2 <
,< =
string 
line1 
, 
string 
line2 
, 
string 
city 
, 
string 
postal 
) 
{ 	
RefNo 
= 
refNo 
; 
	OrderDate 
= 
	orderDate !
;! "
OrderStatus 
= 
orderStatus %
;% &

CustomerId 
= 

customerId #
;# $

OrderItems 
= 

orderItems #
;# $
Line1 
= 
line1 
; 
Line2 
= 
line2 
; 
City   
=   
city   
;   
Postal!! 
=!! 
postal!! 
;!! 
}"" 	
public$$ 
string$$ 
RefNo$$ 
{$$ 
get$$ !
;$$! "
set$$# &
;$$& '
}$$( )
public%% 
DateTime%% 
	OrderDate%% !
{%%" #
get%%$ '
;%%' (
set%%) ,
;%%, -
}%%. /
public&& 
OrderStatus&& 
OrderStatus&& &
{&&' (
get&&) ,
;&&, -
set&&. 1
;&&1 2
}&&3 4
public'' 
Guid'' 

CustomerId'' 
{''  
get''! $
;''$ %
set''& )
;'') *
}''+ ,
public(( 
List(( 
<(( +
CreateOrderCommandOrderItemsDto(( 3
>((3 4

OrderItems((5 ?
{((@ A
get((B E
;((E F
set((G J
;((J K
}((L M
public)) 
string)) 
Line1)) 
{)) 
get)) !
;))! "
set))# &
;))& '
}))( )
public** 
string** 
Line2** 
{** 
get** !
;**! "
set**# &
;**& '
}**( )
public++ 
string++ 
City++ 
{++ 
get++  
;++  !
set++" %
;++% &
}++' (
public,, 
string,, 
Postal,, 
{,, 
get,, "
;,," #
set,,$ '
;,,' (
},,) *
}-- 
}.. ¢
ΩD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrderOrderItem\CreateOrderOrderItemCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D 
CreateOrderOrderItemD X
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 0
$CreateOrderOrderItemCommandValidator

 5
:

6 7
AbstractValidator

8 I
<

I J'
CreateOrderOrderItemCommand

J e
>

e f
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 0
$CreateOrderOrderItemCommandValidator 3
(3 4
)4 5
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ≤!
ªD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrderOrderItem\CreateOrderOrderItemCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
.C D 
CreateOrderOrderItemD X
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class .
"CreateOrderOrderItemCommandHandler 3
:4 5
IRequestHandler6 E
<E F'
CreateOrderOrderItemCommandF a
,a b
Guidc g
>g h
{ 
private 
readonly 
IOrderRepository )
_orderRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"CreateOrderOrderItemCommandHandler 1
(1 2
IOrderRepository2 B
orderRepositoryC R
)R S
{ 	
_orderRepository 
= 
orderRepository .
;. /
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& ''
CreateOrderOrderItemCommand' B
requestC J
,J K
CancellationTokenL ]
cancellationToken^ o
)o p
{ 	
var 
order 
= 
await 
_orderRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
OrderIdE L
,L M
cancellationTokenN _
)_ `
;` a
if 
( 
order 
is 
null 
) 
{   
throw!! 
new!! 
NotFoundException!! +
(!!+ ,
$"!!, .
$str!!. H
{!!H I
request!!I P
.!!P Q
OrderId!!Q X
}!!X Y
$str!!Y Z
"!!Z [
)!![ \
;!!\ ]
}"" 
var## 
	orderItem## 
=## 
new## 
	OrderItem##  )
{$$ 
Quantity%% 
=%% 
request%% "
.%%" #
Quantity%%# +
,%%+ ,
Units&& 
=&& 
request&& 
.&&  
Units&&  %
,&&% &
	UnitPrice'' 
='' 
request'' #
.''# $
Amount''$ *
,''* +
OrderId(( 
=(( 
request(( !
.((! "
OrderId((" )
,(() *
	ProductId)) 
=)) 
request)) #
.))# $
	ProductId))$ -
}** 
;** 
order,, 
.,, 

OrderItems,, 
.,, 
Add,,  
(,,  !
	orderItem,,! *
),,* +
;,,+ ,
await-- 
_orderRepository-- "
.--" #

UnitOfWork--# -
.--- .
SaveChangesAsync--. >
(--> ?
cancellationToken--? P
)--P Q
;--Q R
return.. 
	orderItem.. 
... 
Id.. 
;..  
}// 	
}00 
}11 ú
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrderOrderItem\CreateOrderOrderItemCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Orders		= C
.		C D 
CreateOrderOrderItem		D X
{

 
public 

class '
CreateOrderOrderItemCommand ,
:- .
IRequest/ 7
<7 8
Guid8 <
>< =
,= >
ICommand? G
{ 
public '
CreateOrderOrderItemCommand *
(* +
Guid+ /
orderId0 7
,7 8
int9 <
quantity= E
,E F
decimalG N
amountO U
,U V
intW Z
units[ `
,` a
Guidb f
	productIdg p
)p q
{ 	
OrderId 
= 
orderId 
; 
Quantity 
= 
quantity 
;  
Amount 
= 
amount 
; 
Units 
= 
units 
; 
	ProductId 
= 
	productId !
;! "
} 	
public 
Guid 
OrderId 
{ 
get !
;! "
set# &
;& '
}( )
public 
int 
Quantity 
{ 
get !
;! "
set# &
;& '
}( )
public 
decimal 
Amount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
Units 
{ 
get 
; 
set  #
;# $
}% &
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} Ÿ
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Orders\CreateOrderCommandOrderItemsDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Orders= C
{ 
public		 

class		 +
CreateOrderCommandOrderItemsDto		 0
{

 
public +
CreateOrderCommandOrderItemsDto .
(. /
)/ 0
{ 	
} 	
public 
int 
Quantity 
{ 
get !
;! "
set# &
;& '
}( )
public 
decimal 
Amount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static +
CreateOrderCommandOrderItemsDto 5
Create6 <
(< =
int= @
quantityA I
,I J
decimalK R
amountS Y
,Y Z
Guid[ _
	productId` i
)i j
{ 	
return 
new +
CreateOrderCommandOrderItemsDto 6
{ 
Quantity 
= 
quantity #
,# $
Amount 
= 
amount 
,  
	ProductId 
= 
	productId %
} 
; 
} 	
} 
} ¯
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\OptionalDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
	Optionals

= F
{ 
public 

static 
class (
OptionalDtoMappingExtensions 4
{ 
public 
static 
OptionalDto !
MapToOptionalDto" 2
(2 3
this3 7
Optional8 @
projectFromA L
,L M
IMapperN U
mapperV \
)\ ]
=> 
mapper 
. 
Map 
< 
OptionalDto %
>% &
(& '
projectFrom' 2
)2 3
;3 4
public 
static 
List 
< 
OptionalDto &
>& ' 
MapToOptionalDtoList( <
(< =
this= A
IEnumerableB M
<M N
OptionalN V
>V W
projectFromX c
,c d
IMappere l
mapperm s
)s t
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToOptionalDto) 9
(9 :
mapper: @
)@ A
)A B
.B C
ToListC I
(I J
)J K
;K L
} 
} À
íD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\OptionalDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
	Optionals

= F
{ 
public 

class 
OptionalDto 
: 
IMapFrom '
<' (
Optional( 0
>0 1
{ 
public 
OptionalDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
static 
OptionalDto !
Create" (
(( )
Guid) -
id. 0
,0 1
string2 8
name9 =
)= >
{ 	
return 
new 
OptionalDto "
{ 
Id 
= 
id 
, 
Name 
= 
name 
} 
; 
} 	
public 
void 
Mapping 
( 
Profile #
profile$ +
)+ ,
{   	
profile!! 
.!! 
	CreateMap!! 
<!! 
Optional!! &
,!!& '
OptionalDto!!( 3
>!!3 4
(!!4 5
)!!5 6
;!!6 7
}"" 	
}## 
}$$ Ç
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\GetOptionalById\GetOptionalByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Optionals= F
.F G
GetOptionalByIdG V
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 )
GetOptionalByIdQueryValidator

 .
:

/ 0
AbstractValidator

1 B
<

B C 
GetOptionalByIdQuery

C W
>

W X
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
GetOptionalByIdQueryValidator ,
(, -
)- .
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ê
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\GetOptionalById\GetOptionalByIdQueryHandler.cs
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
 C
,

C D
Version

E L
=

M N
$str

O T
)

T U
]

U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Optionals= F
.F G
GetOptionalByIdG V
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
class '
GetOptionalByIdQueryHandler ,
:- .
IRequestHandler/ >
<> ? 
GetOptionalByIdQuery? S
,S T
OptionalDtoU `
?` a
>a b
{ 
private 
readonly 
IOptionalRepository ,
_optionalRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
GetOptionalByIdQueryHandler *
(* +
IOptionalRepository+ >
optionalRepository? Q
,Q R
IMapperS Z
mapper[ a
)a b
{ 	
_optionalRepository 
=  !
optionalRepository" 4
;4 5
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
OptionalDto %
?% &
>& '
Handle( .
(. / 
GetOptionalByIdQuery/ C
requestD K
,K L
CancellationTokenM ^
cancellationToken_ p
)p q
{ 	
var 
optional 
= 
await  
_optionalRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
return 
optional 
? 
. 
MapToOptionalDto -
(- .
_mapper. 5
)5 6
;6 7
}   	
}!! 
}"" •
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\GetOptionalById\GetOptionalByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Optionals		= F
.		F G
GetOptionalById		G V
{

 
public 

class  
GetOptionalByIdQuery %
:& '
IRequest( 0
<0 1
OptionalDto1 <
?< =
>= >
,> ?
IQuery@ F
{ 
public  
GetOptionalByIdQuery #
(# $
Guid$ (
id) +
)+ ,
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ◊
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\CreateOptional\CreateOptionalCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Optionals= F
.F G
CreateOptionalG U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
CreateOptionalCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
CreateOptionalCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
CreateOptionalCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} †
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\CreateOptional\CreateOptionalCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Optionals= F
.F G
CreateOptionalG U
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
class (
CreateOptionalCommandHandler -
:. /
IRequestHandler0 ?
<? @!
CreateOptionalCommand@ U
,U V
GuidW [
>[ \
{ 
private 
readonly 
IOptionalRepository ,
_optionalRepository- @
;@ A
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
CreateOptionalCommandHandler +
(+ ,
IOptionalRepository, ?
optionalRepository@ R
)R S
{ 	
_optionalRepository 
=  !
optionalRepository" 4
;4 5
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '!
CreateOptionalCommand' <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
var 
optional 
= 
new 
Optional '
{ 
Name 
= 
request 
. 
Name #
} 
; 
_optionalRepository!! 
.!!  
Add!!  #
(!!# $
optional!!$ ,
)!!, -
;!!- .
await"" 
_optionalRepository"" %
.""% &

UnitOfWork""& 0
.""0 1
SaveChangesAsync""1 A
(""A B
cancellationToken""B S
)""S T
;""T U
return## 
optional## 
.## 
Id## 
;## 
}$$ 	
}%% 
}&& û
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Optionals\CreateOptional\CreateOptionalCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Optionals		= F
.		F G
CreateOptional		G U
{

 
public 

class !
CreateOptionalCommand &
:' (
IRequest) 1
<1 2
Guid2 6
>6 7
,7 8
ICommand9 A
{ 
public !
CreateOptionalCommand $
($ %
string% +
name, 0
)0 1
{ 	
Name 
= 
name 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
} 
} ˇ
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\UpdateNestingParent\UpdateNestingParentCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
UpdateNestingParentL _
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#UpdateNestingParentCommandValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
UpdateNestingParentCommand

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#UpdateNestingParentCommandValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} ‘
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\UpdateNestingParent\UpdateNestingParentCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
UpdateNestingParentL _
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
class -
!UpdateNestingParentCommandHandler 2
:3 4
IRequestHandler5 D
<D E&
UpdateNestingParentCommandE _
>_ `
{ 
private 
readonly $
INestingParentRepository 1$
_nestingParentRepository2 J
;J K
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!UpdateNestingParentCommandHandler 0
(0 1$
INestingParentRepository1 I#
nestingParentRepositoryJ a
)a b
{ 	$
_nestingParentRepository $
=% &#
nestingParentRepository' >
;> ?
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !&
UpdateNestingParentCommand! ;
request< C
,C D
CancellationTokenE V
cancellationTokenW h
)h i
{ 	
var 
nestingParent 
= 
await  %$
_nestingParentRepository& >
.> ?
FindByIdAsync? L
(L M
requestM T
.T U
IdU W
,W X
cancellationTokenY j
)j k
;k l
if 
( 
nestingParent 
is  
null! %
)% &
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. L
{L M
requestM T
.T U
IdU W
}W X
$strX Y
"Y Z
)Z [
;[ \
}   
nestingParent"" 
."" 
Name"" 
=""  
request""! (
.""( )
Name"") -
;""- .
}## 	
}$$ 
}%% î
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\UpdateNestingParent\UpdateNestingParentCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
NestingParents		= K
.		K L
UpdateNestingParent		L _
{

 
public 

class &
UpdateNestingParentCommand +
:, -
IRequest. 6
,6 7
ICommand8 @
{ 
public &
UpdateNestingParentCommand )
() *
string* 0
name1 5
,5 6
Guid7 ;
id< >
)> ?
{ 	
Name 
= 
name 
; 
Id 
= 
id 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ∑
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\NestingParentDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
NestingParents

= K
{ 
public 

static 
class -
!NestingParentDtoMappingExtensions 9
{ 
public 
static 
NestingParentDto &!
MapToNestingParentDto' <
(< =
this= A
NestingParentB O
projectFromP [
,[ \
IMapper] d
mappere k
)k l
=> 
mapper 
. 
Map 
< 
NestingParentDto *
>* +
(+ ,
projectFrom, 7
)7 8
;8 9
public 
static 
List 
< 
NestingParentDto +
>+ ,%
MapToNestingParentDtoList- F
(F G
thisG K
IEnumerableL W
<W X
NestingParentX e
>e f
projectFromg r
,r s
IMappert {
mapper	| Ç
)
Ç É
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )!
MapToNestingParentDto) >
(> ?
mapper? E
)E F
)F G
.G H
ToListH N
(N O
)O P
;P Q
} 
} ˝
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\NestingParentDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
NestingParents

= K
{ 
public 

class 
NestingParentDto !
:" #
IMapFrom$ ,
<, -
NestingParent- :
>: ;
{ 
public 
NestingParentDto 
(  
)  !
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
static 
NestingParentDto &
Create' -
(- .
Guid. 2
id3 5
,5 6
string7 =
name> B
)B C
{ 	
return 
new 
NestingParentDto '
{ 
Id 
= 
id 
, 
Name 
= 
name 
} 
; 
} 	
public 
void 
Mapping 
( 
Profile #
profile$ +
)+ ,
{   	
profile!! 
.!! 
	CreateMap!! 
<!! 
NestingParent!! +
,!!+ ,
NestingParentDto!!- =
>!!= >
(!!> ?
)!!? @
;!!@ A
}"" 	
}## 
}$$ ü
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\ManualChildChildDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 (
ManualChildChildDtoValidator

 -
:

. /
AbstractValidator

0 A
<

A B
ManualChildChildDto

B U
>

U V
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
ManualChildChildDtoValidator +
(+ ,
), -
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} º
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\ManualChildChildDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
{ 
public 

class 
ManualChildChildDto $
{		 
public

 
ManualChildChildDto

 "
(

" #
)

# $
{ 	
Name 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
static 
ManualChildChildDto )
Create* 0
(0 1
string1 7
name8 <
)< =
{ 	
return 
new 
ManualChildChildDto *
{ 
Name 
= 
name 
} 
; 
} 	
} 
} ò
ΩD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\GetNestingParents\GetNestingParentsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
GetNestingParentsL ]
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 +
GetNestingParentsQueryValidator

 0
:

1 2
AbstractValidator

3 D
<

D E"
GetNestingParentsQuery

E [
>

[ \
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
GetNestingParentsQueryValidator .
(. /
)/ 0
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ø
ªD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\GetNestingParents\GetNestingParentsQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
GetNestingParentsL ]
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class )
GetNestingParentsQueryHandler .
:/ 0
IRequestHandler1 @
<@ A"
GetNestingParentsQueryA W
,W X
ListY ]
<] ^
NestingParentDto^ n
>n o
>o p
{ 
private 
readonly $
INestingParentRepository 1$
_nestingParentRepository2 J
;J K
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
GetNestingParentsQueryHandler ,
(, -$
INestingParentRepository- E#
nestingParentRepositoryF ]
,] ^
IMapper_ f
mapperg m
)m n
{ 	$
_nestingParentRepository $
=% &#
nestingParentRepository' >
;> ?
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
NestingParentDto /
>/ 0
>0 1
Handle2 8
(8 9"
GetNestingParentsQuery "
request# *
,* +
CancellationToken 
cancellationToken /
)/ 0
{   	
var!! 
nestingParents!! 
=!!  
await!!! &$
_nestingParentRepository!!' ?
.!!? @
FindAllAsync!!@ L
(!!L M
cancellationToken!!M ^
)!!^ _
;!!_ `
return"" 
nestingParents"" !
.""! "%
MapToNestingParentDtoList""" ;
(""; <
_mapper""< C
)""C D
;""D E
}## 	
}$$ 
}%% Î	
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\GetNestingParents\GetNestingParentsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
NestingParents		= K
.		K L
GetNestingParents		L ]
{

 
public 

class "
GetNestingParentsQuery '
:( )
IRequest* 2
<2 3
List3 7
<7 8
NestingParentDto8 H
>H I
>I J
,J K
IQueryL R
{ 
public "
GetNestingParentsQuery %
(% &
)& '
{ 	
} 	
} 
} ™
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\GetNestingParentById\GetNestingParentByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L 
GetNestingParentByIdL `
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 .
"GetNestingParentByIdQueryValidator

 3
:

4 5
AbstractValidator

6 G
<

G H%
GetNestingParentByIdQuery

H a
>

a b
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"GetNestingParentByIdQueryValidator 1
(1 2
)2 3
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ∞
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\GetNestingParentById\GetNestingParentByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L 
GetNestingParentByIdL `
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class ,
 GetNestingParentByIdQueryHandler 1
:2 3
IRequestHandler4 C
<C D%
GetNestingParentByIdQueryD ]
,] ^
NestingParentDto_ o
>o p
{ 
private 
readonly $
INestingParentRepository 1$
_nestingParentRepository2 J
;J K
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 GetNestingParentByIdQueryHandler /
(/ 0$
INestingParentRepository0 H#
nestingParentRepositoryI `
,` a
IMapperb i
mapperj p
)p q
{ 	$
_nestingParentRepository $
=% &#
nestingParentRepository' >
;> ?
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
NestingParentDto *
>* +
Handle, 2
(2 3%
GetNestingParentByIdQuery3 L
requestM T
,T U
CancellationTokenV g
cancellationTokenh y
)y z
{ 	
var 
nestingParent 
= 
await  %$
_nestingParentRepository& >
.> ?
FindByIdAsync? L
(L M
requestM T
.T U
IdU W
,W X
cancellationTokenY j
)j k
;k l
if   
(   
nestingParent   
is    
null  ! %
)  % &
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". L
{""L M
request""M T
.""T U
Id""U W
}""W X
$str""X Y
"""Y Z
)""Z [
;""[ \
}## 
return$$ 
nestingParent$$  
.$$  !!
MapToNestingParentDto$$! 6
($$6 7
_mapper$$7 >
)$$> ?
;$$? @
}%% 	
}&& 
}'' æ
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\GetNestingParentById\GetNestingParentByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
NestingParents		= K
.		K L 
GetNestingParentById		L `
{

 
public 

class %
GetNestingParentByIdQuery *
:+ ,
IRequest- 5
<5 6
NestingParentDto6 F
>F G
,G H
IQueryI O
{ 
public %
GetNestingParentByIdQuery (
(( )
Guid) -
id. 0
)0 1
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ¨
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\DeleteNestingParent\DeleteNestingParentCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
DeleteNestingParentL _
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#DeleteNestingParentCommandValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
DeleteNestingParentCommand

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#DeleteNestingParentCommandValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ’
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\DeleteNestingParent\DeleteNestingParentCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
DeleteNestingParentL _
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
class -
!DeleteNestingParentCommandHandler 2
:3 4
IRequestHandler5 D
<D E&
DeleteNestingParentCommandE _
>_ `
{ 
private 
readonly $
INestingParentRepository 1$
_nestingParentRepository2 J
;J K
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!DeleteNestingParentCommandHandler 0
(0 1$
INestingParentRepository1 I#
nestingParentRepositoryJ a
)a b
{ 	$
_nestingParentRepository $
=% &#
nestingParentRepository' >
;> ?
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !&
DeleteNestingParentCommand! ;
request< C
,C D
CancellationTokenE V
cancellationTokenW h
)h i
{ 	
var 
nestingParent 
= 
await  %$
_nestingParentRepository& >
.> ?
FindByIdAsync? L
(L M
requestM T
.T U
IdU W
,W X
cancellationTokenY j
)j k
;k l
if 
( 
nestingParent 
is  
null! %
)% &
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. L
{L M
requestM T
.T U
IdU W
}W X
$strX Y
"Y Z
)Z [
;[ \
}   $
_nestingParentRepository"" $
.""$ %
Remove""% +
(""+ ,
nestingParent"", 9
)""9 :
;"": ;
}## 	
}$$ 
}%% Ö
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\DeleteNestingParent\DeleteNestingParentCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
NestingParents		= K
.		K L
DeleteNestingParent		L _
{

 
public 

class &
DeleteNestingParentCommand +
:, -
IRequest. 6
,6 7
ICommand8 @
{ 
public &
DeleteNestingParentCommand )
() *
Guid* .
id/ 1
)1 2
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ⁄
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\CreateNestingParent\CreateNestingParentCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
CreateNestingParentL _
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class /
#CreateNestingParentCommandValidator 4
:5 6
AbstractValidator7 H
<H I&
CreateNestingParentCommandI c
>c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#CreateNestingParentCommandValidator 2
(2 3
IValidatorProvider3 E
providerF N
)N O
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
NestingChildren *
)* +
. 
NotNull 
( 
) 
. 
ForEach 
( 
x 
=> 
x 
.  
SetValidator  ,
(, -
provider- 5
.5 6
GetValidator6 B
<B C8
,CreateNestingParentCommandNestingChildrenDtoC o
>o p
(p q
)q r
!r s
)s t
)t u
;u v
} 	
} 
} ‘
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\CreateNestingParent\CreateNestingParentCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
.K L
CreateNestingParentL _
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class -
!CreateNestingParentCommandHandler 2
:3 4
IRequestHandler5 D
<D E&
CreateNestingParentCommandE _
,_ `
Guida e
>e f
{ 
private 
readonly $
INestingParentRepository 1$
_nestingParentRepository2 J
;J K
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!CreateNestingParentCommandHandler 0
(0 1$
INestingParentRepository1 I#
nestingParentRepositoryJ a
)a b
{ 	$
_nestingParentRepository $
=% &#
nestingParentRepository' >
;> ?
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '&
CreateNestingParentCommand' A
requestB I
,I J
CancellationTokenK \
cancellationToken] n
)n o
{ 	
var 
nestingParent 
= 
new  #
NestingParent$ 1
{ 
Name 
= 
request 
. 
Name #
,# $
NestingChildren   
=    !
request  " )
.  ) *
NestingChildren  * 9
.!! 
Select!! 
(!! 
nc!! 
=>!! !
new!!" %
NestingChild!!& 2
{"" 
Description## #
=##$ %
nc##& (
.##( )
Description##) 4
,##4 5
NestingChildChild$$ )
=$$* +
new$$, /
NestingChildChild$$0 A
{%% 
Name&&  
=&&! "
nc&&# %
.&&% &

ChildChild&&& 0
.&&0 1
Name&&1 5
}'' 
}(( 
)(( 
.)) 
ToList)) 
()) 
))) 
}** 
;** $
_nestingParentRepository,, $
.,,$ %
Add,,% (
(,,( )
nestingParent,,) 6
),,6 7
;,,7 8
await-- $
_nestingParentRepository-- *
.--* +

UnitOfWork--+ 5
.--5 6
SaveChangesAsync--6 F
(--F G
cancellationToken--G X
)--X Y
;--Y Z
return.. 
nestingParent..  
...  !
Id..! #
;..# $
}// 	
}00 
}11 ®
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\CreateNestingParent\CreateNestingParentCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
NestingParents

= K
.

K L
CreateNestingParent

L _
{ 
public 

class &
CreateNestingParentCommand +
:, -
IRequest. 6
<6 7
Guid7 ;
>; <
,< =
ICommand> F
{ 
public &
CreateNestingParentCommand )
() *
string* 0
name1 5
,5 6
List7 ;
<; <8
,CreateNestingParentCommandNestingChildrenDto< h
>h i
nestingChildrenj y
)y z
{ 	
Name 
= 
name 
; 
NestingChildren 
= 
nestingChildren -
;- .
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
List 
< 8
,CreateNestingParentCommandNestingChildrenDto @
>@ A
NestingChildrenB Q
{R S
getT W
;W X
setY \
;\ ]
}^ _
} 
} ⁄
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\CreateNestingParentCommandNestingChildrenDtoValidator.cs
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
$str Q
,Q R
VersionS Z
=[ \
$str] b
)b c
]c d
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class A
5CreateNestingParentCommandNestingChildrenDtoValidator F
:G H
AbstractValidatorI Z
<Z [9
,CreateNestingParentCommandNestingChildrenDto	[ á
>
á à
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public A
5CreateNestingParentCommandNestingChildrenDtoValidator D
(D E
IValidatorProviderE W
providerX `
)` a
{ 	$
ConfigureValidationRules $
($ %
provider% -
)- .
;. /
} 	
private 
void $
ConfigureValidationRules -
(- .
IValidatorProvider. @
providerA I
)I J
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Description &
)& '
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 

ChildChild %
)% &
. 
NotNull 
( 
) 
. 
SetValidator 
( 
provider &
.& '
GetValidator' 3
<3 4
ManualChildChildDto4 G
>G H
(H I
)I J
!J K
)K L
;L M
} 	
} 
} Ù
∏D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\NestingParents\CreateNestingParentCommandNestingChildrenDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
NestingParents= K
{ 
public 

class 8
,CreateNestingParentCommandNestingChildrenDto =
{		 
public

 8
,CreateNestingParentCommandNestingChildrenDto

 ;
(

; <
)

< =
{ 	
Description 
= 
null 
! 
;  

ChildChild 
= 
null 
! 
; 
} 	
public 
string 
Description !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
ManualChildChildDto "

ChildChild# -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
public 
static 8
,CreateNestingParentCommandNestingChildrenDto B
CreateC I
(I J
string 
description 
, 
ManualChildChildDto 

childChild  *
)* +
{ 	
return 
new 8
,CreateNestingParentCommandNestingChildrenDto C
{ 
Description 
= 
description )
,) *

ChildChild 
= 

childChild '
} 
; 
} 	
} 
} ¶
ûD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Interfaces\IUploadDownloadService.cs
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
.0 1
Application1 <
.< =

Interfaces= G
{ 
public 

	interface "
IUploadDownloadService +
{ 
Task 
< 
Guid 
> 
Upload 
( 
Stream  
content! (
,( )
string* 0
?0 1
filename2 :
,: ;
string< B
?B C
contentTypeD O
,O P
longQ U
?U V
contentLengthW d
,d e
CancellationTokenf w
cancellationToken	x â
=
ä ã
default
å ì
)
ì î
;
î ï
Task 
< 
FileDownloadDto 
> 
Download &
(& '
Guid' +
id, .
,. /
CancellationToken0 A
cancellationTokenB S
=T U
defaultV ]
)] ^
;^ _
} 
} ¥
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Interfaces\IProductsService.cs
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
 H
,

H I
Version

J Q
=

R S
$str

T Y
)

Y Z
]

Z [
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =

Interfaces= G
{ 
public 

	interface 
IProductsService %
{ 
Task 
< 
Guid 
> 
CreateProduct  
(  !
ProductCreateDto! 1
dto2 5
,5 6
CancellationToken7 H
cancellationTokenI Z
=[ \
default] d
)d e
;e f
Task 
< 

ProductDto 
> 
FindProductById (
(( )
Guid) -
id. 0
,0 1
CancellationToken2 C
cancellationTokenD U
=V W
defaultX _
)_ `
;` a
Task 
< 
List 
< 

ProductDto 
> 
> 
FindProducts +
(+ ,
CancellationToken, =
cancellationToken> O
=P Q
defaultR Y
)Y Z
;Z [
Task 
UpdateProduct 
( 
Guid 
id  "
," #
ProductUpdateDto$ 4
dto5 8
,8 9
CancellationToken: K
cancellationTokenL ]
=^ _
default` g
)g h
;h i
Task 
DeleteProduct 
( 
Guid 
id  "
," #
CancellationToken$ 5
cancellationToken6 G
=H I
defaultJ Q
)Q R
;R S
Task 
< 
decimal 
> 
GetProductPrice %
(% &
Guid& *
	productId+ 4
,4 5
decimal6 =
prices> D
,D E
CancellationTokenF W
cancellationTokenX i
=j k
defaultl s
)s t
;t u
Task 
< 
PagedResult 
< 

ProductDto #
># $
>$ %
FindProductsPaged& 7
(7 8
int8 ;
pageNo< B
,B C
intD G
pageSizeH P
,P Q
stringR X
orderByY `
,` a
CancellationTokenb s
cancellationToken	t Ö
=
Ü á
default
à è
)
è ê
;
ê ë
} 
} ƒ
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Interfaces\IPagingTSService.cs
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
.0 1
Application1 <
.< =

Interfaces= G
{ 
public 

	interface 
IPagingTSService %
{ 
Task 
< 
Guid 
> 
CreatePagingTS !
(! "
PagingTSCreateDto" 3
dto4 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
;g h
Task 
< 
PagingTSDto 
> 
FindPagingTSById *
(* +
Guid+ /
id0 2
,2 3
CancellationToken4 E
cancellationTokenF W
=X Y
defaultZ a
)a b
;b c
Task 
< 
PagedResult 
< 
PagingTSDto $
>$ %
>% &
FindPagingTS' 3
(3 4
int4 7
pageNo8 >
,> ?
int@ C
pageSizeD L
,L M
stringN T
?T U
orderByV ]
,] ^
CancellationToken_ p
cancellationToken	q Ç
=
É Ñ
default
Ö å
)
å ç
;
ç é
Task 
UpdatePagingTS 
( 
Guid  
id! #
,# $
PagingTSUpdateDto% 6
dto7 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
;j k
Task 
DeletePagingTS 
( 
Guid  
id! #
,# $
CancellationToken% 6
cancellationToken7 H
=I J
defaultK R
)R S
;S T
} 
} »
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Interfaces\IClassicDomainServiceTestsService.cs
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
.0 1
Application1 <
.< =

Interfaces= G
{ 
public 

	interface -
!IClassicDomainServiceTestsService 6
{ 
Task 
< 
Guid 
> *
CreateClassicDomainServiceTest 1
(1 2-
!ClassicDomainServiceTestCreateDto2 S
dtoT W
,W X
CancellationTokenY j
cancellationTokenk |
=} ~
default	 Ü
)
Ü á
;
á à
Task 
< '
ClassicDomainServiceTestDto (
>( ),
 FindClassicDomainServiceTestById* J
(J K
GuidK O
idP R
,R S
CancellationTokenT e
cancellationTokenf w
=x y
default	z Å
)
Å Ç
;
Ç É
Task 
< 
List 
< '
ClassicDomainServiceTestDto -
>- .
>. /)
FindClassicDomainServiceTests0 M
(M N
CancellationTokenN _
cancellationToken` q
=r s
defaultt {
){ |
;| }
Task *
UpdateClassicDomainServiceTest +
(+ ,
Guid, 0
id1 3
,3 4-
!ClassicDomainServiceTestUpdateDto5 V
dtoW Z
,Z [
CancellationToken\ m
cancellationTokenn 
=
Ä Å
default
Ç â
)
â ä
;
ä ã
Task *
DeleteClassicDomainServiceTest +
(+ ,
Guid, 0
id1 3
,3 4
CancellationToken5 F
cancellationTokenG X
=Y Z
default[ b
)b c
;c d
} 
} «	
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Interfaces\Customers\IPersonService.cs
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
0 1
Application

1 <
.

< =

Interfaces

= G
.

G H
	Customers

H Q
{ 
public 

	interface 
IPersonService #
{ 
Task 
< 
	PersonDto 
> 
GetPersonById %
(% &
Guid& *
personId+ 3
,3 4
CancellationToken5 F
cancellationTokenG X
=Y Z
default[ b
)b c
;c d
} 
} Ñ
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\IProductServiceProxy.cs
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
 
DefaultIntentManaged		 
(		  
Mode		  $
.		$ %
Fully		% *
,		* +
Targets		, 3
=		4 5
Targets		6 =
.		= >
Usings		> D
)		D E
]		E F
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
 P
,

P Q
Version

R Y
=

Z [
$str

\ a
)

a b
]

b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
{ 
public 

	interface  
IProductServiceProxy )
:* +
IDisposable, 7
{ 
Task 
< 
Guid 
> 
CreateProductAsync %
(% & 
CreateProductCommand& :
command; B
,B C
CancellationTokenD U
cancellationTokenV g
=h i
defaultj q
)q r
;r s
Task 
DeleteProductAsync 
(  
Guid  $
id% '
,' (
CancellationToken) :
cancellationToken; L
=M N
defaultO V
)V W
;W X
Task 
UpdateProductAsync 
(  
Guid  $
id% '
,' ( 
UpdateProductCommand) =
command> E
,E F
CancellationTokenG X
cancellationTokenY j
=k l
defaultm t
)t u
;u v
Task 
< 

ProductDto 
> 
GetProductByIdAsync ,
(, -
Guid- 1
id2 4
,4 5
CancellationToken6 G
cancellationTokenH Y
=Z [
default\ c
)c d
;d e
Task 
< 
List 
< 

ProductDto 
> 
> 
GetProductsAsync /
(/ 0
CancellationToken0 A
cancellationTokenB S
=T U
defaultV ]
)] ^
;^ _
} 
} œ
§D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\IFileUploadsService.cs
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
 
DefaultIntentManaged		 
(		  
Mode		  $
.		$ %
Fully		% *
,		* +
Targets		, 3
=		4 5
Targets		6 =
.		= >
Usings		> D
)		D E
]		E F
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
 P
,

P Q
Version

R Y
=

Z [
$str

\ a
)

a b
]

b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
{ 
public 

	interface 
IFileUploadsService (
:) *
IDisposable+ 6
{ 
Task 
< 
Guid 
> 
UploadFileAsync "
(" #
string# )
?) *
contentType+ 6
,6 7
long8 <
?< =
contentLength> K
,K L
UploadFileCommandM ^
command_ f
,f g
CancellationTokenh y
cancellationToken	z ã
=
å ç
default
é ï
)
ï ñ
;
ñ ó
Task 
< 
FileDownloadDto 
> 
DownloadFileAsync /
(/ 0
Guid0 4
id5 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
;g h
} 
} ë
ßD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\ICustomersServiceProxy.cs
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
 
DefaultIntentManaged		 
(		  
Mode		  $
.		$ %
Fully		% *
,		* +
Targets		, 3
=		4 5
Targets		6 =
.		= >
Usings		> D
)		D E
]		E F
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
 P
,

P Q
Version

R Y
=

Z [
$str

\ a
)

a b
]

b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
{ 
public 

	interface "
ICustomersServiceProxy +
:, -
IDisposable. 9
{ 
Task 
< 
Guid 
> 
CreateCustomerAsync &
(& '!
CreateCustomerCommand' <
command= D
,D E
CancellationTokenF W
cancellationTokenX i
=j k
defaultl s
)s t
;t u
Task 
DeleteCustomerAsync  
(  !
Guid! %
id& (
,( )
CancellationToken* ;
cancellationToken< M
=N O
defaultP W
)W X
;X Y
Task 
UpdateCustomerAsync  
(  !
Guid! %
id& (
,( )!
UpdateCustomerCommand* ?
command@ G
,G H
CancellationTokenI Z
cancellationToken[ l
=m n
defaulto v
)v w
;w x
Task 
< 
CustomerDto 
>  
GetCustomerByIdAsync .
(. /
Guid/ 3
id4 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
;f g
Task 
< 
List 
< 
CustomerDto 
> 
> 
GetCustomersAsync  1
(1 2
CancellationToken2 C
cancellationTokenD U
=V W
defaultX _
)_ `
;` a
} 
} –
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\Services\FileUploads\UploadFileCommand.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
Services[ c
.c d
FileUploadsd o
{		 
public

 

class

 
UploadFileCommand

 "
{ 
public 
UploadFileCommand  
(  !
)! "
{ 	
Content 
= 
null 
! 
; 
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Filename 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
ContentType "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
long 
? 
ContentLength "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
static 
UploadFileCommand '
Create( .
(. /
Stream/ 5
content6 =
,= >
string? E
?E F
filenameG O
,O P
stringQ W
?W X
contentTypeY d
,d e
longf j
?j k
contentLengthl y
)y z
{ 	
return 
new 
UploadFileCommand (
{ 
Content 
= 
content !
,! "
Filename 
= 
filename #
,# $
ContentType 
= 
contentType )
,) *
ContentLength 
= 
contentLength  -
} 
; 
} 	
}   
}!! Ô
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\Services\Common\FileDownloadDto.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
Services[ c
.c d
Commond j
{		 
public

 

class

 
FileDownloadDto

  
{ 
public 
FileDownloadDto 
( 
)  
{ 	
Content 
= 
null 
! 
; 
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Filename 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
ContentType "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
static 
FileDownloadDto %
Create& ,
(, -
Stream- 3
content4 ;
,; <
string= C
?C D
filenameE M
,M N
stringO U
?U V
contentTypeW b
)b c
{ 	
return 
new 
FileDownloadDto &
{ 
Content 
= 
content !
,! "
Filename 
= 
filename #
,# $
ContentType 
= 
contentType )
} 
; 
} 	
} 
} Á
—D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\DbContext\Tests\Services\Products\UpdateProductCommand.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
	DbContext[ d
.d e
Testse j
.j k
Servicesk s
.s t
Productst |
{		 
public

 

class

  
UpdateProductCommand

 %
{ 
public  
UpdateProductCommand #
(# $
)$ %
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
decimal 
Price 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static  
UpdateProductCommand *
Create+ 1
(1 2
Guid2 6
id7 9
,9 :
string; A
nameB F
,F G
decimalH O
priceP U
)U V
{ 	
return 
new  
UpdateProductCommand +
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Price 
= 
price 
} 
; 
} 	
} 
} µ
«D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\DbContext\Tests\Services\Products\ProductDto.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
	DbContext[ d
.d e
Testse j
.j k
Servicesk s
.s t
Productst |
{		 
public

 

class

 

ProductDto

 
{ 
public 

ProductDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
decimal 
Price 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static 

ProductDto  
Create! '
(' (
Guid( ,
id- /
,/ 0
string1 7
name8 <
,< =
decimal> E
priceF K
)K L
{ 	
return 
new 

ProductDto !
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Price 
= 
price 
} 
; 
} 	
} 
} ‰
—D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\DbContext\Tests\Services\Products\CreateProductCommand.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
	DbContext[ d
.d e
Testse j
.j k
Servicesk s
.s t
Productst |
{ 
public		 

class		  
CreateProductCommand		 %
{

 
public  
CreateProductCommand #
(# $
)$ %
{ 	
Name 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
decimal 
Price 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static  
CreateProductCommand *
Create+ 1
(1 2
string2 8
name9 =
,= >
decimal? F
priceG L
)L M
{ 	
return 
new  
CreateProductCommand +
{ 
Name 
= 
name 
, 
Price 
= 
price 
} 
; 
} 	
} 
} „
”D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\DbContext\Tests\Services\Customers\UpdateCustomerCommand.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
	DbContext[ d
.d e
Testse j
.j k
Servicesk s
.s t
	Customerst }
{		 
public

 

class

 !
UpdateCustomerCommand

 &
{ 
public !
UpdateCustomerCommand $
($ %
)% &
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static !
UpdateCustomerCommand +
Create, 2
(2 3
Guid3 7
id8 :
,: ;
string< B
nameC G
,G H
stringI O
surnameP W
,W X
boolY ]
isActive^ f
)f g
{ 	
return 
new !
UpdateCustomerCommand ,
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Surname 
= 
surname !
,! "
IsActive 
= 
isActive #
} 
; 
}   	
}!! 
}"" ±
…D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\DbContext\Tests\Services\Customers\CustomerDto.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
	DbContext[ d
.d e
Testse j
.j k
Servicesk s
.s t
	Customerst }
{		 
public

 

class

 
CustomerDto

 
{ 
public 
CustomerDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static 
CustomerDto !
Create" (
(( )
Guid) -
id. 0
,0 1
string2 8
name9 =
,= >
string? E
surnameF M
,M N
boolO S
isActiveT \
)\ ]
{ 	
return 
new 
CustomerDto "
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Surname 
= 
surname !
,! "
IsActive 
= 
isActive #
} 
; 
}   	
}!! 
}"" ‡
”D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationServices\Contracts\DbContext\Tests\Services\Customers\CreateCustomerCommand.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str L
,L M
VersionN U
=V W
$strX ]
)] ^
]^ _
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationServices= P
.P Q
	ContractsQ Z
.Z [
	DbContext[ d
.d e
Testse j
.j k
Servicesk s
.s t
	Customerst }
{ 
public		 

class		 !
CreateCustomerCommand		 &
{

 
public !
CreateCustomerCommand $
($ %
)% &
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static !
CreateCustomerCommand +
Create, 2
(2 3
string3 9
name: >
,> ?
string@ F
surnameG N
,N O
boolP T
isActiveU ]
)] ^
{ 	
return 
new !
CreateCustomerCommand ,
{ 
Name 
= 
name 
, 
Surname 
= 
surname !
,! "
IsActive 
= 
isActive #
} 
; 
} 	
} 
} •
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationEvents\SampleType.cs
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
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationEvents= N
{ 
public 

enum 

SampleType 
{		 
TypeA

 
,

 
TypeB 
} 
} Ê
∏D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationEvents\QuoteCreatedIntegrationEventQuoteLinesDto.cs
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
$str I
,I J
VersionK R
=S T
$strU Z
)Z [
][ \
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Eventing1 9
.9 :
Messages: B
{ 
public		 

class		 5
)QuoteCreatedIntegrationEventQuoteLinesDto		 :
{

 
public 5
)QuoteCreatedIntegrationEventQuoteLinesDto 8
(8 9
)9 :
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static 5
)QuoteCreatedIntegrationEventQuoteLinesDto ?
Create@ F
(F G
GuidG K
idL N
,N O
GuidP T
	productIdU ^
)^ _
{ 	
return 
new 5
)QuoteCreatedIntegrationEventQuoteLinesDto @
{ 
Id 
= 
id 
, 
	ProductId 
= 
	productId %
} 
; 
} 	
} 
} ò
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationEvents\QuoteCreatedIntegrationEvent.cs
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
$str M
,M N
VersionO V
=W X
$strY ^
)^ _
]_ `
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Eventing1 9
.9 :
Messages: B
{		 
public

 

record

 (
QuoteCreatedIntegrationEvent

 .
{ 
public (
QuoteCreatedIntegrationEvent +
(+ ,
), -
{ 	
RefNo 
= 
null 
! 
; 

QuoteLines 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
init "
;" #
}$ %
public 
string 
RefNo 
{ 
get !
;! "
init# '
;' (
}) *
public 
Guid 
PersonId 
{ 
get "
;" #
init$ (
;( )
}* +
public 
string 
? 
PersonEmail "
{# $
get% (
;( )
init* .
;. /
}0 1
public 
List 
< 5
)QuoteCreatedIntegrationEventQuoteLinesDto =
>= >

QuoteLines? I
{J K
getL O
;O P
initQ U
;U V
}W X
} 
} ‰
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationEvents\EventHandlers\EnumMessage\EnumSampleHandler.cs
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
$str		 O
,		O P
Version		Q X
=		Y Z
$str		[ `
)		` a
]		a b
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationEvents= N
.N O
EventHandlersO \
.\ ]
EnumMessage] h
{ 
[ 
IntentManaged 
( 
Mode 
. 
Fully 
, 
Body #
=$ %
Mode& *
.* +
Merge+ 0
)0 1
]1 2
public 

class 
EnumSampleHandler "
:# $$
IIntegrationEventHandler% =
<= >
EnumSampleEvent> M
>M N
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 
EnumSampleHandler  
(  !
)! "
{ 	
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
HandleAsync %
(% &
EnumSampleEvent& 5
message6 =
,= >
CancellationToken? P
cancellationTokenQ b
=c d
defaulte l
)l m
{ 	
} 	
} 
} ∞
 D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationEvents\EventHandlers\Customers\QuoteCreatedIntegrationEventHandler.cs
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
$str		 O
,		O P
Version		Q X
=		Y Z
$str		[ `
)		` a
]		a b
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
IntegrationEvents= N
.N O
EventHandlersO \
.\ ]
	Customers] f
{ 
[ 
IntentManaged 
( 
Mode 
. 
Fully 
, 
Body #
=$ %
Mode& *
.* +
Merge+ 0
)0 1
]1 2
public 

class /
#QuoteCreatedIntegrationEventHandler 4
:5 6$
IIntegrationEventHandler7 O
<O P(
QuoteCreatedIntegrationEventP l
>l m
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#QuoteCreatedIntegrationEventHandler 2
(2 3
)3 4
{ 	
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
HandleAsync %
(% &(
QuoteCreatedIntegrationEvent& B
messageC J
,J K
CancellationTokenL ]
cancellationToken^ o
=p q
defaultr y
)y z
{ 	
} 	
} 
} Í

ûD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\IntegrationEvents\EnumSampleEvent.cs
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
.0 1
Eventing1 9
.9 :
Messages: B
{ 
public		 

record		 
EnumSampleEvent		 !
{

 
public 
EnumSampleEvent 
( 
)  
{ 	
Name 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
init" &
;& '
}( )
public 

SampleType 

SampleType $
{% &
get' *
;* +
init, 0
;0 1
}2 3
} 
} ¬)
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Implementation\UploadDownloadService.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str [
,[ \
Version] d
=e f
$strg l
)l m
]m n
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Implementation= K
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
) 
] 
public 

class !
UploadDownloadService &
:' ("
IUploadDownloadService) ?
{ 
private 
readonly !
IFileUploadRepository .!
_fileUploadRepository/ D
;D E
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public !
UploadDownloadService $
($ %!
IFileUploadRepository% : 
fileUploadRepository; O
)O P
{ 	!
_fileUploadRepository !
=" # 
fileUploadRepository$ 8
;8 9
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
async 
Task 
< 
Guid 
> 
Upload  &
(& '
Stream   
content   
,   
string!! 
?!! 
filename!! 
,!! 
string"" 
?"" 
contentType"" 
,""  
long## 
?## 
contentLength## 
,##  
CancellationToken$$ 
cancellationToken$$ /
=$$0 1
default$$2 9
)$$9 :
{%% 	
var&& 
entity&& 
=&& 
new&& 

FileUpload&& '
(&&' (
)&&( )
{'' 
Filename(( 
=(( 
filename(( #
??(($ &
$str((' .
,((. /
ContentType)) 
=)) 
contentType)) )
??))* ,
$str))- G
}** 
;** 
using++ 
(++ 
MemoryStream++ 
ms++  "
=++# $
new++% (
(++( )
)++) *
)++* +
{,, 
await-- 
content-- 
.-- 
CopyToAsync-- )
(--) *
ms--* ,
)--, -
;--- .
entity.. 
... 
Content.. 
=..  
ms..! #
...# $
ToArray..$ +
(..+ ,
).., -
;..- .
}// !
_fileUploadRepository00 !
.00! "
Add00" %
(00% &
entity00& ,
)00, -
;00- .
await11 !
_fileUploadRepository11 '
.11' (

UnitOfWork11( 2
.112 3
SaveChangesAsync113 C
(11C D
)11D E
;11E F
return22 
entity22 
.22 
Id22 
;22 
}33 	
[55 	
IntentManaged55	 
(55 
Mode55 
.55 
Fully55 !
,55! "
Body55# '
=55( )
Mode55* .
.55. /
Ignore55/ 5
)555 6
]556 7
public66 
async66 
Task66 
<66 
FileDownloadDto66 )
>66) *
Download66+ 3
(663 4
Guid664 8
id669 ;
,66; <
CancellationToken66= N
cancellationToken66O `
=66a b
default66c j
)66j k
{77 	
var88 
file88 
=88 
await88 !
_fileUploadRepository88 2
.882 3
FindByIdAsync883 @
(88@ A
id88A C
,88C D
cancellationToken88E V
)88V W
;88W X
if99 
(99 
file99 
is99 
null99 
)99 
{:: 
throw;; 
new;; 
NotFoundException;; +
(;;+ ,
$";;, .
$str;;. I
{;;I J
id;;J L
};;L M
$str;;M N
";;N O
);;O P
;;;P Q
}<< 
return== 
new== 
FileDownloadDto== &
(==& '
)==' (
{==) *
Content==+ 2
===3 4
new==5 8
MemoryStream==9 E
(==E F
file==F J
.==J K
Content==K R
)==R S
,==S T
ContentType==U `
===a b
file==c g
.==g h
ContentType==h s
}==t u
;==u v
}>> 	
}?? 
}@@ êg
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Implementation\ProductsService.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str [
,[ \
Version] d
=e f
$strg l
)l m
]m n
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Implementation= K
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
) 
] 
public 

class 
ProductsService  
:! "
IProductsService# 3
{ 
private 
readonly 
IProductRepository +
_productRepository, >
;> ?
private 
readonly 
IMapper  
_mapper! (
;( )
private 
readonly 
IPricingService (
_pricingService) 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public   
ProductsService   
(   
IProductRepository   1
productRepository  2 C
,  C D
IMapper  E L
mapper  M S
,  S T
IPricingService  U d
pricingService  e s
)  s t
{!! 	
_productRepository"" 
=""  
productRepository""! 2
;""2 3
_mapper## 
=## 
mapper## 
;## 
_pricingService$$ 
=$$ 
pricingService$$ ,
;$$, -
}%% 	
['' 	
IntentManaged''	 
('' 
Mode'' 
.'' 
Fully'' !
,''! "
Body''# '
=''( )
Mode''* .
.''. /
Fully''/ 4
)''4 5
]''5 6
public(( 
async(( 
Task(( 
<(( 
Guid(( 
>(( 
CreateProduct((  -
(((- .
ProductCreateDto((. >
dto((? B
,((B C
CancellationToken((D U
cancellationToken((V g
=((h i
default((j q
)((q r
{)) 	
var** 
product** 
=** 
new** 
Product** %
{++ 
Name,, 
=,, 
dto,, 
.,, 
Name,, 
,,,  
Tags-- 
=-- 
dto-- 
.-- 
Tags-- 
... 
Select.. 
(.. 
t.. 
=>..  
new..! $
Tag..% (
(..( )
name// 
:// 
t// 
.//  
Name//  $
,//$ %
value00 
:00 
t00  
.00  !
Value00! &
)00& '
)00' (
.11 
ToList11 
(11 
)11 
}22 
;22 
_productRepository44 
.44 
Add44 "
(44" #
product44# *
)44* +
;44+ ,
await55 
_productRepository55 $
.55$ %

UnitOfWork55% /
.55/ 0
SaveChangesAsync550 @
(55@ A
cancellationToken55A R
)55R S
;55S T
return66 
product66 
.66 
Id66 
;66 
}77 	
[99 	
IntentManaged99	 
(99 
Mode99 
.99 
Fully99 !
,99! "
Body99# '
=99( )
Mode99* .
.99. /
Fully99/ 4
)994 5
]995 6
public:: 
async:: 
Task:: 
<:: 

ProductDto:: $
>::$ %
FindProductById::& 5
(::5 6
Guid::6 :
id::; =
,::= >
CancellationToken::? P
cancellationToken::Q b
=::c d
default::e l
)::l m
{;; 	
var<< 
product<< 
=<< 
await<< 
_productRepository<<  2
.<<2 3
FindByIdAsync<<3 @
(<<@ A
id<<A C
,<<C D
cancellationToken<<E V
)<<V W
;<<W X
if== 
(== 
product== 
is== 
null== 
)==  
{>> 
throw?? 
new?? 
NotFoundException?? +
(??+ ,
$"??, .
$str??. F
{??F G
id??G I
}??I J
$str??J K
"??K L
)??L M
;??M N
}@@ 
returnAA 
productAA 
.AA 
MapToProductDtoAA *
(AA* +
_mapperAA+ 2
)AA2 3
;AA3 4
}BB 	
[DD 	
IntentManagedDD	 
(DD 
ModeDD 
.DD 
FullyDD !
,DD! "
BodyDD# '
=DD( )
ModeDD* .
.DD. /
FullyDD/ 4
)DD4 5
]DD5 6
publicEE 
asyncEE 
TaskEE 
<EE 
ListEE 
<EE 

ProductDtoEE )
>EE) *
>EE* +
FindProductsEE, 8
(EE8 9
CancellationTokenEE9 J
cancellationTokenEEK \
=EE] ^
defaultEE_ f
)EEf g
{FF 	
varGG 
productsGG 
=GG 
awaitGG  
_productRepositoryGG! 3
.GG3 4
FindAllAsyncGG4 @
(GG@ A
cancellationTokenGGA R
)GGR S
;GGS T
returnHH 
productsHH 
.HH 
MapToProductDtoListHH /
(HH/ 0
_mapperHH0 7
)HH7 8
;HH8 9
}II 	
[KK 	
IntentManagedKK	 
(KK 
ModeKK 
.KK 
FullyKK !
,KK! "
BodyKK# '
=KK( )
ModeKK* .
.KK. /
FullyKK/ 4
)KK4 5
]KK5 6
publicLL 
asyncLL 
TaskLL 
UpdateProductLL '
(LL' (
GuidLL( ,
idLL- /
,LL/ 0
ProductUpdateDtoLL1 A
dtoLLB E
,LLE F
CancellationTokenLLG X
cancellationTokenLLY j
=LLk l
defaultLLm t
)LLt u
{MM 	
varNN 
productNN 
=NN 
awaitNN 
_productRepositoryNN  2
.NN2 3
FindByIdAsyncNN3 @
(NN@ A
idNNA C
,NNC D
cancellationTokenNNE V
)NNV W
;NNW X
ifOO 
(OO 
productOO 
isOO 
nullOO 
)OO  
{PP 
throwQQ 
newQQ 
NotFoundExceptionQQ +
(QQ+ ,
$"QQ, .
$strQQ. F
{QQF G
idQQG I
}QQI J
$strQQJ K
"QQK L
)QQL M
;QQM N
}RR 
productTT 
.TT 
NameTT 
=TT 
dtoTT 
.TT 
NameTT #
;TT# $
productUU 
.UU 
TagsUU 
=UU 
UpdateHelperUU '
.UU' ($
CreateOrUpdateCollectionUU( @
(UU@ A
productUUA H
.UUH I
TagsUUI M
,UUM N
dtoUUO R
.UUR S
TagsUUS W
,UUW X
(UUY Z
eUUZ [
,UU[ \
dUU] ^
)UU^ _
=>UU` b
eUUc d
.UUd e
EqualsUUe k
(UUk l
newUUl o
TagUUp s
(UUs t
nameVV 
:VV 	
dVV
 
.VV 
NameVV 
,VV 
valueWW 	
:WW	 

dWW 
.WW 
ValueWW 
)WW 
)WW 
,WW 
CreateOrUpdateTagWW '
)WW' (
;WW( )
}XX 	
[ZZ 	
IntentManagedZZ	 
(ZZ 
ModeZZ 
.ZZ 
FullyZZ !
,ZZ! "
BodyZZ# '
=ZZ( )
ModeZZ* .
.ZZ. /
FullyZZ/ 4
)ZZ4 5
]ZZ5 6
public[[ 
async[[ 
Task[[ 
DeleteProduct[[ '
([[' (
Guid[[( ,
id[[- /
,[[/ 0
CancellationToken[[1 B
cancellationToken[[C T
=[[U V
default[[W ^
)[[^ _
{\\ 	
var]] 
product]] 
=]] 
await]] 
_productRepository]]  2
.]]2 3
FindByIdAsync]]3 @
(]]@ A
id]]A C
,]]C D
cancellationToken]]E V
)]]V W
;]]W X
if^^ 
(^^ 
product^^ 
is^^ 
null^^ 
)^^  
{__ 
throw`` 
new`` 
NotFoundException`` +
(``+ ,
$"``, .
$str``. F
{``F G
id``G I
}``I J
$str``J K
"``K L
)``L M
;``M N
}aa 
_productRepositorycc 
.cc 
Removecc %
(cc% &
productcc& -
)cc- .
;cc. /
}dd 	
[ff 	
IntentManagedff	 
(ff 
Modeff 
.ff 
Fullyff !
,ff! "
Bodyff# '
=ff( )
Modeff* .
.ff. /
Fullyff/ 4
)ff4 5
]ff5 6
publicgg 
asyncgg 
Taskgg 
<gg 
decimalgg !
>gg! "
GetProductPricegg# 2
(gg2 3
Guidhh 
	productIdhh 
,hh 
decimalii 
pricesii 
,ii 
CancellationTokenjj 
cancellationTokenjj /
=jj0 1
defaultjj2 9
)jj9 :
{kk 	
varll 
resultll 
=ll 
awaitll 
_pricingServicell .
.ll. / 
GetProductPriceAsyncll/ C
(llC D
	productIdllD M
,llM N
cancellationTokenllO `
)ll` a
;lla b
varmm 
sumPricemm 
=mm 
_pricingServicemm *
.mm* +
	SumPricesmm+ 4
(mm4 5
pricesmm5 ;
)mm; <
;mm< =
returnnn 
resultnn 
;nn 
}oo 	
[qq 	
IntentManagedqq	 
(qq 
Modeqq 
.qq 
Fullyqq !
,qq! "
Bodyqq# '
=qq( )
Modeqq* .
.qq. /
Fullyqq/ 4
)qq4 5
]qq5 6
publicrr 
asyncrr 
Taskrr 
<rr 
Commonrr  
.rr  !

Paginationrr! +
.rr+ ,
PagedResultrr, 7
<rr7 8

ProductDtorr8 B
>rrB C
>rrC D
FindProductsPagedrrE V
(rrV W
intss 
pageNoss 
,ss 
inttt 
pageSizett 
,tt 
stringuu 
orderByuu 
,uu 
CancellationTokenvv 
cancellationTokenvv /
=vv0 1
defaultvv2 9
)vv9 :
{ww 	
varxx 
productsxx 
=xx 
awaitxx  
_productRepositoryxx! 3
.xx3 4
FindAllAsyncxx4 @
(xx@ A
pageNoxxA G
,xxG H
pageSizexxI Q
,xxQ R
queryOptionsxxS _
=>xx` b
queryOptionsxxc o
.xxo p
OrderByxxp w
(xxw x
orderByxxx 
)	xx Ä
,
xxÄ Å
cancellationToken
xxÇ ì
)
xxì î
;
xxî ï
returnyy 
productsyy 
.yy 
MapToPagedResultyy ,
(yy, -
xyy- .
=>yy/ 1
xyy2 3
.yy3 4
MapToProductDtoyy4 C
(yyC D
_mapperyyD K
)yyK L
)yyL M
;yyM N
}zz 	
[|| 	
IntentManaged||	 
(|| 
Mode|| 
.|| 
Fully|| !
)||! "
]||" #
private}} 
static}} 
Tag}} 
CreateOrUpdateTag}} ,
(}}, -
Tag}}- 0
?}}0 1
valueObject}}2 =
,}}= >
UpdateProductTagDto}}? R
dto}}S V
)}}V W
{~~ 	
if 
( 
valueObject 
is 
null #
)# $
{
ÄÄ 
return
ÅÅ 
new
ÅÅ 
Tag
ÅÅ 
(
ÅÅ 
name
ÇÇ 
:
ÇÇ 	
dto
ÇÇ
 
.
ÇÇ 
Name
ÇÇ 
,
ÇÇ 
value
ÉÉ 	
:
ÉÉ	 

dto
ÉÉ 
.
ÉÉ 
Value
ÉÉ 
)
ÉÉ 
;
ÉÉ 
}
ÑÑ 
return
ÖÖ 
valueObject
ÖÖ 
;
ÖÖ 
}
ÜÜ 	
}
áá 
}àà ´D
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Implementation\PagingTSService.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str [
,[ \
Version] d
=e f
$strg l
)l m
]m n
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Implementation= K
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
) 
] 
public 

class 
PagingTSService  
:! "
IPagingTSService# 3
{ 
private 
readonly 
IPagingTSRepository ,
_pagingTSRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 
PagingTSService 
( 
IPagingTSRepository 2
pagingTSRepository3 E
,E F
IMapperG N
mapperO U
)U V
{ 	
_pagingTSRepository 
=  !
pagingTSRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
."". /
Fully""/ 4
)""4 5
]""5 6
public## 
async## 
Task## 
<## 
Guid## 
>## 
CreatePagingTS##  .
(##. /
PagingTSCreateDto##/ @
dto##A D
,##D E
CancellationToken##F W
cancellationToken##X i
=##j k
default##l s
)##s t
{$$ 	
var%% 
pagingTS%% 
=%% 
new%% 
Domain%% %
.%%% &
Entities%%& .
.%%. /
PagingTS%%/ 7
{&& 
Name'' 
='' 
dto'' 
.'' 
Name'' 
}(( 
;(( 
_pagingTSRepository** 
.**  
Add**  #
(**# $
pagingTS**$ ,
)**, -
;**- .
await++ 
_pagingTSRepository++ %
.++% &

UnitOfWork++& 0
.++0 1
SaveChangesAsync++1 A
(++A B
cancellationToken++B S
)++S T
;++T U
return,, 
pagingTS,, 
.,, 
Id,, 
;,, 
}-- 	
[// 	
IntentManaged//	 
(// 
Mode// 
.// 
Fully// !
,//! "
Body//# '
=//( )
Mode//* .
.//. /
Fully/// 4
)//4 5
]//5 6
public00 
async00 
Task00 
<00 
PagingTSDto00 %
>00% &
FindPagingTSById00' 7
(007 8
Guid008 <
id00= ?
,00? @
CancellationToken00A R
cancellationToken00S d
=00e f
default00g n
)00n o
{11 	
var22 
pagingTS22 
=22 
await22  
_pagingTSRepository22! 4
.224 5
FindByIdAsync225 B
(22B C
id22C E
,22E F
cancellationToken22G X
)22X Y
;22Y Z
if33 
(33 
pagingTS33 
is33 
null33  
)33  !
{44 
throw55 
new55 
NotFoundException55 +
(55+ ,
$"55, .
$str55. G
{55G H
id55H J
}55J K
$str55K L
"55L M
)55M N
;55N O
}66 
return77 
pagingTS77 
.77 
MapToPagingTSDto77 ,
(77, -
_mapper77- 4
)774 5
;775 6
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
.::. /
Fully::/ 4
)::4 5
]::5 6
public;; 
async;; 
Task;; 
<;; 
Common;;  
.;;  !

Pagination;;! +
.;;+ ,
PagedResult;;, 7
<;;7 8
PagingTSDto;;8 C
>;;C D
>;;D E
FindPagingTS;;F R
(;;R S
int<< 
pageNo<< 
,<< 
int== 
pageSize== 
,== 
string>> 
?>> 
orderBy>> 
,>> 
CancellationToken?? 
cancellationToken?? /
=??0 1
default??2 9
)??9 :
{@@ 	
varAA 
pagingTSAA 
=AA 
awaitAA  
_pagingTSRepositoryAA! 4
.AA4 5
FindAllAsyncAA5 A
(AAA B
pageNoAAB H
,AAH I
pageSizeAAJ R
,AAR S
queryOptionsAAT `
=>AAa c
queryOptionsAAd p
.AAp q
OrderByAAq x
(AAx y
orderBy	AAy Ä
??
AAÅ É
$str
AAÑ à
)
AAà â
,
AAâ ä
cancellationToken
AAã ú
)
AAú ù
;
AAù û
returnBB 
pagingTSBB 
.BB 
MapToPagedResultBB ,
(BB, -
xBB- .
=>BB/ 1
xBB2 3
.BB3 4
MapToPagingTSDtoBB4 D
(BBD E
_mapperBBE L
)BBL M
)BBM N
;BBN O
}CC 	
[EE 	
IntentManagedEE	 
(EE 
ModeEE 
.EE 
FullyEE !
,EE! "
BodyEE# '
=EE( )
ModeEE* .
.EE. /
FullyEE/ 4
)EE4 5
]EE5 6
publicFF 
asyncFF 
TaskFF 
UpdatePagingTSFF (
(FF( )
GuidFF) -
idFF. 0
,FF0 1
PagingTSUpdateDtoFF2 C
dtoFFD G
,FFG H
CancellationTokenFFI Z
cancellationTokenFF[ l
=FFm n
defaultFFo v
)FFv w
{GG 	
varHH 
pagingTSHH 
=HH 
awaitHH  
_pagingTSRepositoryHH! 4
.HH4 5
FindByIdAsyncHH5 B
(HHB C
idHHC E
,HHE F
cancellationTokenHHG X
)HHX Y
;HHY Z
ifII 
(II 
pagingTSII 
isII 
nullII  
)II  !
{JJ 
throwKK 
newKK 
NotFoundExceptionKK +
(KK+ ,
$"KK, .
$strKK. G
{KKG H
idKKH J
}KKJ K
$strKKK L
"KKL M
)KKM N
;KKN O
}LL 
pagingTSNN 
.NN 
NameNN 
=NN 
dtoNN 
.NN  
NameNN  $
;NN$ %
}OO 	
[QQ 	
IntentManagedQQ	 
(QQ 
ModeQQ 
.QQ 
FullyQQ !
,QQ! "
BodyQQ# '
=QQ( )
ModeQQ* .
.QQ. /
FullyQQ/ 4
)QQ4 5
]QQ5 6
publicRR 
asyncRR 
TaskRR 
DeletePagingTSRR (
(RR( )
GuidRR) -
idRR. 0
,RR0 1
CancellationTokenRR2 C
cancellationTokenRRD U
=RRV W
defaultRRX _
)RR_ `
{SS 	
varTT 
pagingTSTT 
=TT 
awaitTT  
_pagingTSRepositoryTT! 4
.TT4 5
FindByIdAsyncTT5 B
(TTB C
idTTC E
,TTE F
cancellationTokenTTG X
)TTX Y
;TTY Z
ifUU 
(UU 
pagingTSUU 
isUU 
nullUU  
)UU  !
{VV 
throwWW 
newWW 
NotFoundExceptionWW +
(WW+ ,
$"WW, .
$strWW. G
{WWG H
idWWH J
}WWJ K
$strWWK L
"WWL M
)WWM N
;WWN O
}XX 
_pagingTSRepositoryZZ 
.ZZ  
RemoveZZ  &
(ZZ& '
pagingTSZZ' /
)ZZ/ 0
;ZZ0 1
}[[ 	
}\\ 
}]] £
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Implementation\Customers\PersonService.cs
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
 [
,

[ \
Version

] d
=

e f
$str

g l
)

l m
]

m n
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Implementation= K
.K L
	CustomersL U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
) 
] 
public 

class 
PersonService 
:  
IPersonService! /
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 
PersonService 
( 
) 
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
public 
async 
Task 
< 
	PersonDto #
># $
GetPersonById% 2
(2 3
Guid3 7
personId8 @
,@ A
CancellationTokenB S
cancellationTokenT e
=f g
defaulth o
)o p
{ 	
throw 
new #
NotImplementedException -
(- .
$str. b
)b c
;c d
} 	
} 
} çF
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Implementation\ClassicDomainServiceTestsService.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str [
,[ \
Version] d
=e f
$strg l
)l m
]m n
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Implementation= K
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
) 
] 
public 

class ,
 ClassicDomainServiceTestsService 1
:2 3-
!IClassicDomainServiceTestsService4 U
{ 
private 
readonly /
#IClassicDomainServiceTestRepository </
#_classicDomainServiceTestRepository= `
;` a
private 
readonly 
IMyDomainService )
_myDomainService* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 ClassicDomainServiceTestsService /
(/ 0/
#IClassicDomainServiceTestRepository0 S.
"classicDomainServiceTestRepositoryT v
,v w
IMyDomainService 
myDomainService ,
,, -
IMapper 
mapper 
) 
{ 	/
#_classicDomainServiceTestRepository   /
=  0 1.
"classicDomainServiceTestRepository  2 T
;  T U
_myDomainService!! 
=!! 
myDomainService!! .
;!!. /
_mapper"" 
="" 
mapper"" 
;"" 
}## 	
[%% 	
IntentManaged%%	 
(%% 
Mode%% 
.%% 
Fully%% !
,%%! "
Body%%# '
=%%( )
Mode%%* .
.%%. /
Fully%%/ 4
)%%4 5
]%%5 6
public&& 
async&& 
Task&& 
<&& 
Guid&& 
>&& *
CreateClassicDomainServiceTest&&  >
(&&> ?-
!ClassicDomainServiceTestCreateDto'' -
dto''. 1
,''1 2
CancellationToken(( 
cancellationToken(( /
=((0 1
default((2 9
)((9 :
{)) 	
var** $
classicDomainServiceTest** (
=**) *
new**+ .$
ClassicDomainServiceTest**/ G
(**G H
service++ 
:++ 
_myDomainService++ )
)++) *
;++* +/
#_classicDomainServiceTestRepository-- /
.--/ 0
Add--0 3
(--3 4$
classicDomainServiceTest--4 L
)--L M
;--M N
await.. /
#_classicDomainServiceTestRepository.. 5
...5 6

UnitOfWork..6 @
...@ A
SaveChangesAsync..A Q
(..Q R
cancellationToken..R c
)..c d
;..d e
return// $
classicDomainServiceTest// +
.//+ ,
Id//, .
;//. /
}00 	
[22 	
IntentManaged22	 
(22 
Mode22 
.22 
Fully22 !
,22! "
Body22# '
=22( )
Mode22* .
.22. /
Fully22/ 4
)224 5
]225 6
public33 
async33 
Task33 
<33 '
ClassicDomainServiceTestDto33 5
>335 6,
 FindClassicDomainServiceTestById337 W
(33W X
Guid44 
id44 
,44 
CancellationToken55 
cancellationToken55 /
=550 1
default552 9
)559 :
{66 	
var77 $
classicDomainServiceTest77 (
=77) *
await77+ 0/
#_classicDomainServiceTestRepository771 T
.77T U
FindByIdAsync77U b
(77b c
id77c e
,77e f
cancellationToken77g x
)77x y
;77y z
if88 
(88 $
classicDomainServiceTest88 (
is88) +
null88, 0
)880 1
{99 
throw:: 
new:: 
NotFoundException:: +
(::+ ,
$"::, .
$str::. W
{::W X
id::X Z
}::Z [
$str::[ \
"::\ ]
)::] ^
;::^ _
};; 
return<< $
classicDomainServiceTest<< +
.<<+ ,,
 MapToClassicDomainServiceTestDto<<, L
(<<L M
_mapper<<M T
)<<T U
;<<U V
}== 	
[?? 	
IntentManaged??	 
(?? 
Mode?? 
.?? 
Fully?? !
,??! "
Body??# '
=??( )
Mode??* .
.??. /
Fully??/ 4
)??4 5
]??5 6
public@@ 
async@@ 
Task@@ 
<@@ 
List@@ 
<@@ '
ClassicDomainServiceTestDto@@ :
>@@: ;
>@@; <)
FindClassicDomainServiceTests@@= Z
(@@Z [
CancellationToken@@[ l
cancellationToken@@m ~
=	@@ Ä
default
@@Å à
)
@@à â
{AA 	
varBB %
classicDomainServiceTestsBB )
=BB* +
awaitBB, 1/
#_classicDomainServiceTestRepositoryBB2 U
.BBU V
FindAllAsyncBBV b
(BBb c
cancellationTokenBBc t
)BBt u
;BBu v
returnCC %
classicDomainServiceTestsCC ,
.CC, -0
$MapToClassicDomainServiceTestDtoListCC- Q
(CCQ R
_mapperCCR Y
)CCY Z
;CCZ [
}DD 	
[FF 	
IntentManagedFF	 
(FF 
ModeFF 
.FF 
FullyFF !
,FF! "
BodyFF# '
=FF( )
ModeFF* .
.FF. /
FullyFF/ 4
)FF4 5
]FF5 6
publicGG 
asyncGG 
TaskGG *
UpdateClassicDomainServiceTestGG 8
(GG8 9
GuidHH 
idHH 
,HH -
!ClassicDomainServiceTestUpdateDtoII -
dtoII. 1
,II1 2
CancellationTokenJJ 
cancellationTokenJJ /
=JJ0 1
defaultJJ2 9
)JJ9 :
{KK 	
varLL $
classicDomainServiceTestLL (
=LL) *
awaitLL+ 0/
#_classicDomainServiceTestRepositoryLL1 T
.LLT U
FindByIdAsyncLLU b
(LLb c
idLLc e
,LLe f
cancellationTokenLLg x
)LLx y
;LLy z
ifMM 
(MM $
classicDomainServiceTestMM (
isMM) +
nullMM, 0
)MM0 1
{NN 
throwOO 
newOO 
NotFoundExceptionOO +
(OO+ ,
$"OO, .
$strOO. W
{OOW X
idOOX Z
}OOZ [
$strOO[ \
"OO\ ]
)OO] ^
;OO^ _
}PP $
classicDomainServiceTestRR $
.RR$ %
	ClassicOpRR% .
(RR. /
_myDomainServiceRR/ ?
)RR? @
;RR@ A
}SS 	
[UU 	
IntentManagedUU	 
(UU 
ModeUU 
.UU 
FullyUU !
,UU! "
BodyUU# '
=UU( )
ModeUU* .
.UU. /
FullyUU/ 4
)UU4 5
]UU5 6
publicVV 
asyncVV 
TaskVV *
DeleteClassicDomainServiceTestVV 8
(VV8 9
GuidVV9 =
idVV> @
,VV@ A
CancellationTokenVVB S
cancellationTokenVVT e
=VVf g
defaultVVh o
)VVo p
{WW 	
varXX $
classicDomainServiceTestXX (
=XX) *
awaitXX+ 0/
#_classicDomainServiceTestRepositoryXX1 T
.XXT U
FindByIdAsyncXXU b
(XXb c
idXXc e
,XXe f
cancellationTokenXXg x
)XXx y
;XXy z
ifYY 
(YY $
classicDomainServiceTestYY (
isYY) +
nullYY, 0
)YY0 1
{ZZ 
throw[[ 
new[[ 
NotFoundException[[ +
([[+ ,
$"[[, .
$str[[. W
{[[W X
id[[X Z
}[[Z [
$str[[[ \
"[[\ ]
)[[] ^
;[[^ _
}\\ /
#_classicDomainServiceTestRepository^^ /
.^^/ 0
Remove^^0 6
(^^6 7$
classicDomainServiceTest^^7 O
)^^O P
;^^P Q
}__ 	
}`` 
}aa  
èD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\GlobalSuppressions.cs
[ 
assembly 	
:	 

SuppressMessage 
( 
$str -
,- .
$str/ V
,V W
JustificationX e
=f g
$strh s
,s t
Scopeu z
={ |
$str	} Ö
,
Ö Ü
Target
á ç
=
é è
$str
ê ¬
)
¬ √
]
√ ƒ
[		 
assembly		 	
:			 

SuppressMessage		 
(		 
$str		 -
,		- .
$str		/ V
,		V W
Justification		X e
=		f g
$str		h s
,		s t
Scope		u z
=		{ |
$str			} Ö
,
		Ö Ü
Target
		á ç
=
		é è
$str
		ê ¬
)
		¬ √
]
		√ ƒ
[

 
assembly

 	
:

	 

SuppressMessage

 
(

 
$str

 -
,

- .
$str

/ V
,

V W
Justification

X e
=

f g
$str

h s
,

s t
Scope

u z
=

{ |
$str	

} Ö
,


Ö Ü
Target


á ç
=


é è
$str


ê “
)


“ ”
]


” ‘
[ 
assembly 	
:	 

SuppressMessage 
( 
$str -
,- .
$str/ V
,V W
JustificationX e
=f g
$strh s
,s t
Scopeu z
={ |
$str	} Ö
,
Ö Ü
Target
á ç
=
é è
$str
ê ∆
)
∆ «
]
« »∆
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\UploadFile\UploadFileCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I

UploadFileI S
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
UploadFileCommandValidator

 +
:

, -
AbstractValidator

. ?
<

? @
UploadFileCommand

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
UploadFileCommandValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Content "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} †
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\UploadFile\UploadFileCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I

UploadFileI S
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class $
UploadFileCommandHandler )
:* +
IRequestHandler, ;
<; <
UploadFileCommand< M
,M N
GuidO S
>S T
{ 
private 
readonly !
IFileUploadRepository .!
_fileUploadRepository/ D
;D E
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
UploadFileCommandHandler '
(' (!
IFileUploadRepository( = 
fileUploadRepository> R
)R S
{ 	!
_fileUploadRepository !
=" # 
fileUploadRepository$ 8
;8 9
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
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '
UploadFileCommand' 8
request9 @
,@ A
CancellationTokenB S
cancellationTokenT e
)e f
{ 	
var 
entity 
= 
new 

FileUpload '
(' (
)( )
{ 
Filename 
= 
request "
." #
Filename# +
,+ ,
ContentType 
= 
request %
.% &
ContentType& 1
}   
;   
using!! 
(!! 
MemoryStream!! 
ms!!  "
=!!# $
new!!% (
(!!( )
)!!) *
)!!* +
{"" 
await## 
request## 
.## 
Content## %
.##% &
CopyToAsync##& 1
(##1 2
ms##2 4
)##4 5
;##5 6
entity$$ 
.$$ 
Content$$ 
=$$  
ms$$! #
.$$# $
ToArray$$$ +
($$+ ,
)$$, -
;$$- .
}%% !
_fileUploadRepository&& !
.&&! "
Add&&" %
(&&% &
entity&&& ,
)&&, -
;&&- .
await'' !
_fileUploadRepository'' '
.''' (

UnitOfWork''( 2
.''2 3
SaveChangesAsync''3 C
(''C D
)''D E
;''E F
return(( 
entity(( 
.(( 
Id(( 
;(( 
})) 	
}** 
}++ Ì
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\UploadFile\UploadFileCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
FileUploads

= H
.

H I

UploadFile

I S
{ 
public 

class 
UploadFileCommand "
:# $
IRequest% -
<- .
Guid. 2
>2 3
,3 4
ICommand5 =
{ 
public 
UploadFileCommand  
(  !
Stream! '
content( /
,/ 0
string1 7
?7 8
filename9 A
,A B
stringC I
?I J
contentTypeK V
,V W
longX \
?\ ]
contentLength^ k
)k l
{ 	
Content 
= 
content 
; 
Filename 
= 
filename 
;  
ContentType 
= 
contentType %
;% &
ContentLength 
= 
contentLength )
;) *
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Filename 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
ContentType "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
long 
? 
ContentLength "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} “
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\SimpleUpload\SimpleUploadCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
SimpleUploadI U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 (
SimpleUploadCommandValidator

 -
:

. /
AbstractValidator

0 A
<

A B
SimpleUploadCommand

B U
>

U V
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
SimpleUploadCommandValidator +
(+ ,
), -
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Content "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} ‡
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\SimpleUpload\SimpleUploadCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
SimpleUploadI U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class &
SimpleUploadCommandHandler +
:, -
IRequestHandler. =
<= >
SimpleUploadCommand> Q
,Q R
GuidS W
>W X
{ 
private 
readonly !
IFileUploadRepository .!
_fileUploadRepository/ D
;D E
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
SimpleUploadCommandHandler )
() *!
IFileUploadRepository* ? 
fileUploadRepository@ T
)T U
{ 	!
_fileUploadRepository !
=" # 
fileUploadRepository$ 8
;8 9
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
Task 
< 
Guid 
> 
Handle  &
(& '
SimpleUploadCommand' :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 	
var 
entity 
= 
new 

FileUpload '
(' (
)( )
{ 
Filename 
= 
Guid 
.  
NewGuid  '
(' (
)( )
.) *
ToString* 2
(2 3
)3 4
,4 5
ContentType   
=   
$str   8
}!! 
;!! 
using"" 
("" 
MemoryStream"" 
ms""  "
=""# $
new""% (
(""( )
)"") *
)""* +
{## 
await$$ 
request$$ 
.$$ 
Content$$ %
.$$% &
CopyToAsync$$& 1
($$1 2
ms$$2 4
)$$4 5
;$$5 6
entity%% 
.%% 
Content%% 
=%%  
ms%%! #
.%%# $
ToArray%%$ +
(%%+ ,
)%%, -
;%%- .
}&& !
_fileUploadRepository'' !
.''! "
Add''" %
(''% &
entity''& ,
)'', -
;''- .
await(( !
_fileUploadRepository(( '
.((' (

UnitOfWork((( 2
.((2 3
SaveChangesAsync((3 C
(((C D
)((D E
;((E F
return)) 
entity)) 
.)) 
Id)) 
;)) 
}** 	
}++ 
},, §
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\SimpleUpload\SimpleUploadCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
FileUploads

= H
.

H I
SimpleUpload

I U
{ 
public 

class 
SimpleUploadCommand $
:% &
IRequest' /
</ 0
Guid0 4
>4 5
,5 6
ICommand7 ?
{ 
public 
SimpleUploadCommand "
(" #
Stream# )
content* 1
)1 2
{ 	
Content 
= 
content 
; 
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} Ä
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\SimpleDownload\SimpleDownloadQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
SimpleDownloadI W
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 (
SimpleDownloadQueryValidator

 -
:

. /
AbstractValidator

0 A
<

A B
SimpleDownloadQuery

B U
>

U V
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
SimpleDownloadQueryValidator +
(+ ,
), -
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} î
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\SimpleDownload\SimpleDownloadQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
SimpleDownloadI W
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class &
SimpleDownloadQueryHandler +
:, -
IRequestHandler. =
<= >
SimpleDownloadQuery> Q
,Q R!
SimpleFileDownloadDtoS h
>h i
{ 
private 
readonly !
IFileUploadRepository .!
_fileUploadRepository/ D
;D E
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
SimpleDownloadQueryHandler )
() *!
IFileUploadRepository* ? 
fileUploadRepository@ T
)T U
{ 	!
_fileUploadRepository !
=" # 
fileUploadRepository$ 8
;8 9
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Ignore/ 5
)5 6
]6 7
public 
async 
Task 
< !
SimpleFileDownloadDto /
>/ 0
Handle1 7
(7 8
SimpleDownloadQuery8 K
requestL S
,S T
CancellationTokenU f
cancellationTokeng x
)x y
{ 	
var 
file 
= 
await !
_fileUploadRepository 2
.2 3
FindByIdAsync3 @
(@ A
requestA H
.H I
IdI K
,K L
cancellationTokenM ^
)^ _
;_ `
if 
( 
file 
is 
null 
) 
{   
throw!! 
new!! 
NotFoundException!! +
(!!+ ,
$"!!, .
$str!!. I
{!!I J
request!!J Q
.!!Q R
Id!!R T
}!!T U
$str!!U V
"!!V W
)!!W X
;!!X Y
}"" 
return$$ 
new$$ !
SimpleFileDownloadDto$$ ,
($$, -
)$$- .
{$$/ 0
Content$$1 8
=$$9 :
new$$; >
MemoryStream$$? K
($$K L
file$$L P
.$$P Q
Content$$Q X
)$$X Y
}$$Z [
;$$[ \
}%% 	
}&& 
}'' ü
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\SimpleDownload\SimpleDownloadQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
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
0 1
Application

1 <
.

< =
FileUploads

= H
.

H I
SimpleDownload

I W
{ 
public 

class 
SimpleDownloadQuery $
:% &
IRequest' /
</ 0!
SimpleFileDownloadDto0 E
>E F
,F G
IQueryH N
{ 
public 
SimpleDownloadQuery "
(" #
Guid# '
id( *
)* +
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Í
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\RestrictedUpload\RestrictedUploadCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
RestrictedUploadI Y
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 ,
 RestrictedUploadCommandValidator

 1
:

2 3
AbstractValidator

4 E
<

E F#
RestrictedUploadCommand

F ]
>

] ^
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 RestrictedUploadCommandValidator /
(/ 0
)0 1
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Content "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} Ω
∏D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\RestrictedUpload\RestrictedUploadCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
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
0 1
Application

1 <
.

< =
FileUploads

= H
.

H I
RestrictedUpload

I Y
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
class *
RestrictedUploadCommandHandler /
:0 1
IRequestHandler2 A
<A B#
RestrictedUploadCommandB Y
>Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
RestrictedUploadCommandHandler -
(- .
). /
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
public 
async 
Task 
Handle  
(  !#
RestrictedUploadCommand! 8
request9 @
,@ A
CancellationTokenB S
cancellationTokenT e
)e f
{ 	
throw 
new #
NotImplementedException -
(- .
$str. K
)K L
;L M
} 	
} 
} €
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\RestrictedUpload\RestrictedUploadCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
FileUploads		= H
.		H I
RestrictedUpload		I Y
{

 
public 

class #
RestrictedUploadCommand (
:) *
IRequest+ 3
,3 4
ICommand5 =
{ 
public #
RestrictedUploadCommand &
(& '
Stream' -
content. 5
,5 6
string7 =
?= >
filename? G
,G H
stringI O
?O P
contentTypeQ \
,\ ]
long^ b
?b c
contentLengthd q
)q r
{ 	
Content 
= 
content 
; 
Filename 
= 
filename 
;  
ContentType 
= 
contentType %
;% &
ContentLength 
= 
contentLength )
;) *
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Filename 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
ContentType "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
long 
? 
ContentLength "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} Ù
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\DownloadFile\DownloadFileQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
DownloadFileI U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
DownloadFileQueryValidator

 +
:

, -
AbstractValidator

. ?
<

? @
DownloadFileQuery

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
DownloadFileQueryValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Â
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\DownloadFile\DownloadFileQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
FileUploads= H
.H I
DownloadFileI U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class $
DownloadFileQueryHandler )
:* +
IRequestHandler, ;
<; <
DownloadFileQuery< M
,M N
FileDownloadDtoO ^
>^ _
{ 
private 
readonly !
IFileUploadRepository .!
_fileUploadRepository/ D
;D E
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
DownloadFileQueryHandler '
(' (!
IFileUploadRepository( = 
fileUploadRepository> R
)R S
{ 	!
_fileUploadRepository !
=" # 
fileUploadRepository$ 8
;8 9
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
< 
FileDownloadDto )
>) *
Handle+ 1
(1 2
DownloadFileQuery2 C
requestD K
,K L
CancellationTokenM ^
cancellationToken_ p
)p q
{ 	
var 
file 
= 
await !
_fileUploadRepository 2
.2 3
FindByIdAsync3 @
(@ A
requestA H
.H I
IdI K
,K L
cancellationTokenM ^
)^ _
;_ `
if   
(   
file   
is   
null   
)   
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". I
{""I J
request""J Q
.""Q R
Id""R T
}""T U
$str""U V
"""V W
)""W X
;""X Y
}## 
return$$ 
new$$ 
FileDownloadDto$$ &
($$& '
)$$' (
{$$) *
Content$$+ 2
=$$3 4
new$$5 8
MemoryStream$$9 E
($$E F
file$$F J
.$$J K
Content$$K R
)$$R S
,$$S T
ContentType$$U `
=$$a b
file$$c g
.$$g h
ContentType$$h s
}$$t u
;$$u v
}&& 	
}'' 
}(( è
ßD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\FileUploads\DownloadFile\DownloadFileQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
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
0 1
Application

1 <
.

< =
FileUploads

= H
.

H I
DownloadFile

I U
{ 
public 

class 
DownloadFileQuery "
:# $
IRequest% -
<- .
FileDownloadDto. =
>= >
,> ?
IQuery@ F
{ 
public 
DownloadFileQuery  
(  !
Guid! %
id& (
)( )
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ˚
ƒD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformValueObj\PerformValueObjCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformValueObjU d
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 +
PerformValueObjCommandValidator

 0
:

1 2
AbstractValidator

3 D
<

D E"
PerformValueObjCommand

E [
>

[ \
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
PerformValueObjCommandValidator .
(. /
)/ 0
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Value1 !
)! "
. 
NotNull 
( 
) 
; 
} 	
} 
} Å
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformValueObj\PerformValueObjCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformValueObjU d
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
class )
PerformValueObjCommandHandler .
:/ 0
IRequestHandler1 @
<@ A"
PerformValueObjCommandA W
>W X
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
PerformValueObjCommandHandler ,
(, -#
IExtensiveDomainService- D"
extensiveDomainServiceE [
)[ \
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !"
PerformValueObjCommand! 7
request8 ?
,? @
CancellationTokenA R
cancellationTokenS d
)d e
{ 	#
_extensiveDomainService #
.# $
PerformValueObj$ 3
(3 4
new4 7
SimpleVO8 @
(@ A
value1 
: 
request 
.  
Value1  &
,& '
value2 
: 
request 
.  
Value2  &
)& '
)' (
;( )
} 	
}   
}!! ®
ªD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformValueObj\PerformValueObjCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformValueObjU d
{		 
public

 

class

 "
PerformValueObjCommand

 '
:

( )
IRequest

* 2
,

2 3
ICommand

4 <
{ 
public "
PerformValueObjCommand %
(% &
string& ,
value1- 3
,3 4
int5 8
value29 ?
)? @
{ 	
Value1 
= 
value1 
; 
Value2 
= 
value2 
; 
} 	
public 
string 
Value1 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
Value2 
{ 
get 
;  
set! $
;$ %
}& '
} 
} ô
ŒD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformValueObjAsync\PerformValueObjAsyncCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U 
PerformValueObjAsyncU i
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 0
$PerformValueObjAsyncCommandValidator

 5
:

6 7
AbstractValidator

8 I
<

I J'
PerformValueObjAsyncCommand

J e
>

e f
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 0
$PerformValueObjAsyncCommandValidator 3
(3 4
)4 5
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Value1 !
)! "
. 
NotNull 
( 
) 
; 
} 	
} 
} Í
ÃD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformValueObjAsync\PerformValueObjAsyncCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U 
PerformValueObjAsyncU i
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
class .
"PerformValueObjAsyncCommandHandler 3
:4 5
IRequestHandler6 E
<E F'
PerformValueObjAsyncCommandF a
>a b
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"PerformValueObjAsyncCommandHandler 1
(1 2#
IExtensiveDomainService2 I"
extensiveDomainServiceJ `
)` a
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !'
PerformValueObjAsyncCommand! <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
await #
_extensiveDomainService )
.) * 
PerformValueObjAsync* >
(> ?
new? B
SimpleVOC K
(K L
value1 
: 
request 
.  
Value1  &
,& '
value2 
: 
request 
.  
Value2  &
)& '
,' (
cancellationToken) :
): ;
;; <
} 	
}   
}!! ¡
≈D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformValueObjAsync\PerformValueObjAsyncCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U 
PerformValueObjAsyncU i
{		 
public

 

class

 '
PerformValueObjAsyncCommand

 ,
:

- .
IRequest

/ 7
,

7 8
ICommand

9 A
{ 
public '
PerformValueObjAsyncCommand *
(* +
string+ 1
value12 8
,8 9
int: =
value2> D
)D E
{ 	
Value1 
= 
value1 
; 
Value2 
= 
value2 
; 
} 	
public 
string 
Value1 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
Value2 
{ 
get 
;  
set! $
;$ %
}& '
} 
} Í
 D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformPassthrough\PerformPassthroughCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformPassthroughU g
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 .
"PerformPassthroughCommandValidator

 3
:

4 5
AbstractValidator

6 G
<

G H%
PerformPassthroughCommand

H a
>

a b
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"PerformPassthroughCommandValidator 1
(1 2
)2 3
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
ConcreteAttr '
)' (
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
BaseAttr #
)# $
. 
NotNull 
( 
) 
; 
} 	
} 
} Ø
»D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformPassthrough\PerformPassthroughCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformPassthroughU g
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
class ,
 PerformPassthroughCommandHandler 1
:2 3
IRequestHandler4 C
<C D%
PerformPassthroughCommandD ]
>] ^
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public ,
 PerformPassthroughCommandHandler /
(/ 0#
IExtensiveDomainService0 G"
extensiveDomainServiceH ^
)^ _
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !%
PerformPassthroughCommand! :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 	#
_extensiveDomainService #
.# $
PerformPassthrough$ 6
(6 7
new7 :
PassthroughObj; I
(I J
baseAttr 
: 
request !
.! "
BaseAttr" *
,* +
concreteAttr 
: 
request %
.% &
ConcreteAttr& 2
)2 3
)3 4
;4 5
} 	
}   
}!! ›
¡D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformPassthrough\PerformPassthroughCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformPassthroughU g
{		 
public

 

class

 %
PerformPassthroughCommand

 *
:

+ ,
IRequest

- 5
,

5 6
ICommand

7 ?
{ 
public %
PerformPassthroughCommand (
(( )
string) /
concreteAttr0 <
,< =
string> D
baseAttrE M
)M N
{ 	
ConcreteAttr 
= 
concreteAttr '
;' (
BaseAttr 
= 
baseAttr 
;  
} 	
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} à
‘D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformPassthroughAsync\PerformPassthroughAsyncCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U#
PerformPassthroughAsyncU l
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 3
'PerformPassthroughAsyncCommandValidator

 8
:

9 :
AbstractValidator

; L
<

L M*
PerformPassthroughAsyncCommand

M k
>

k l
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 3
'PerformPassthroughAsyncCommandValidator 6
(6 7
)7 8
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
ConcreteAttr '
)' (
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
BaseAttr #
)# $
. 
NotNull 
( 
) 
; 
} 	
} 
} ò
“D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformPassthroughAsync\PerformPassthroughAsyncCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U#
PerformPassthroughAsyncU l
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
class 1
%PerformPassthroughAsyncCommandHandler 6
:7 8
IRequestHandler9 H
<H I*
PerformPassthroughAsyncCommandI g
>g h
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 1
%PerformPassthroughAsyncCommandHandler 4
(4 5#
IExtensiveDomainService5 L"
extensiveDomainServiceM c
)c d
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !*
PerformPassthroughAsyncCommand! ?
request@ G
,G H
CancellationTokenI Z
cancellationToken[ l
)l m
{ 	
await #
_extensiveDomainService )
.) *#
PerformPassthroughAsync* A
(A B
newB E
PassthroughObjF T
(T U
baseAttr 
: 
request !
.! "
BaseAttr" *
,* +
concreteAttr 
: 
request %
.% &
ConcreteAttr& 2
)2 3
,3 4
cancellationToken5 F
)F G
;G H
} 	
}   
}!! ˆ
ÀD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformPassthroughAsync\PerformPassthroughAsyncCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U#
PerformPassthroughAsyncU l
{		 
public

 

class

 *
PerformPassthroughAsyncCommand

 /
:

0 1
IRequest

2 :
,

: ;
ICommand

< D
{ 
public *
PerformPassthroughAsyncCommand -
(- .
string. 4
concreteAttr5 A
,A B
stringC I
baseAttrJ R
)R S
{ 	
ConcreteAttr 
= 
concreteAttr '
;' (
BaseAttr 
= 
baseAttr 
;  
} 	
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} “
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityB\PerformEntityBCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityBU c
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
PerformEntityBCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
PerformEntityBCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
PerformEntityBCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
BaseAttr #
)# $
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ConcreteAttr '
)' (
. 
NotNull 
( 
) 
; 
} 	
} 
} ê
¿D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityB\PerformEntityBCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityBU c
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
class (
PerformEntityBCommandHandler -
:. /
IRequestHandler0 ?
<? @!
PerformEntityBCommand@ U
>U V
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
PerformEntityBCommandHandler +
(+ ,#
IExtensiveDomainService, C"
extensiveDomainServiceD Z
)Z [
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !!
PerformEntityBCommand! 6
request7 >
,> ?
CancellationToken@ Q
cancellationTokenR c
)c d
{ 	#
_extensiveDomainService #
.# $
PerformEntityB$ 2
(2 3
new3 6
ConcreteEntityB7 F
(F G
baseAttr 
: 
request 
. 
BaseAttr 
, 
concreteAttr 
: 
request 
. 
ConcreteAttr &
)& '
)' (
;( )
} 	
}   
}!! …
πD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityB\PerformEntityBCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityBU c
{		 
public

 

class

 !
PerformEntityBCommand

 &
:

' (
IRequest

) 1
,

1 2
ICommand

3 ;
{ 
public !
PerformEntityBCommand $
($ %
string% +
baseAttr, 4
,4 5
string6 <
concreteAttr= I
)I J
{ 	
BaseAttr 
= 
baseAttr 
;  
ConcreteAttr 
= 
concreteAttr '
;' (
} 	
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} 
ÃD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityBAsync\PerformEntityBAsyncCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityBAsyncU h
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#PerformEntityBAsyncCommandValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
PerformEntityBAsyncCommand

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#PerformEntityBAsyncCommandValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
BaseAttr #
)# $
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
ConcreteAttr '
)' (
. 
NotNull 
( 
) 
; 
} 	
} 
} ı
 D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityBAsync\PerformEntityBAsyncCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityBAsyncU h
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
class -
!PerformEntityBAsyncCommandHandler 2
:3 4
IRequestHandler5 D
<D E&
PerformEntityBAsyncCommandE _
>_ `
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!PerformEntityBAsyncCommandHandler 0
(0 1#
IExtensiveDomainService1 H"
extensiveDomainServiceI _
)_ `
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !&
PerformEntityBAsyncCommand! ;
request< C
,C D
CancellationTokenE V
cancellationTokenW h
)h i
{ 	
await #
_extensiveDomainService )
.) *
PerformEntityBAsync* =
(= >
new> A
ConcreteEntityBB Q
(Q R
baseAttr 
: 	
request
 
. 
BaseAttr 
, 
concreteAttr 
: 
request 
. 
ConcreteAttr "
)" #
,# $
cancellationToken% 6
)6 7
;7 8
} 	
}   
}!! ‚
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityBAsync\PerformEntityBAsyncCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityBAsyncU h
{		 
public

 

class

 &
PerformEntityBAsyncCommand

 +
:

, -
IRequest

. 6
,

6 7
ICommand

8 @
{ 
public &
PerformEntityBAsyncCommand )
() *
string* 0
baseAttr1 9
,9 :
string; A
concreteAttrB N
)N O
{ 	
BaseAttr 
= 
baseAttr 
;  
ConcreteAttr 
= 
concreteAttr '
;' (
} 	
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} “
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityA\PerformEntityACommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityAU c
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
PerformEntityACommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
PerformEntityACommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
PerformEntityACommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
ConcreteAttr '
)' (
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
BaseAttr #
)# $
. 
NotNull 
( 
) 
; 
} 	
} 
} ê
¿D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityA\PerformEntityACommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityAU c
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
class (
PerformEntityACommandHandler -
:. /
IRequestHandler0 ?
<? @!
PerformEntityACommand@ U
>U V
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
PerformEntityACommandHandler +
(+ ,#
IExtensiveDomainService, C"
extensiveDomainServiceD Z
)Z [
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !!
PerformEntityACommand! 6
request7 >
,> ?
CancellationToken@ Q
cancellationTokenR c
)c d
{ 	#
_extensiveDomainService #
.# $
PerformEntityA$ 2
(2 3
new3 6
ConcreteEntityA7 F
{ 
ConcreteAttr 
= 
request &
.& '
ConcreteAttr' 3
,3 4
BaseAttr 
= 
request "
." #
BaseAttr# +
}   
)   
;   
}!! 	
}"" 
}## …
πD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityA\PerformEntityACommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityAU c
{		 
public

 

class

 !
PerformEntityACommand

 &
:

' (
IRequest

) 1
,

1 2
ICommand

3 ;
{ 
public !
PerformEntityACommand $
($ %
string% +
concreteAttr, 8
,8 9
string: @
baseAttrA I
)I J
{ 	
ConcreteAttr 
= 
concreteAttr '
;' (
BaseAttr 
= 
baseAttr 
;  
} 	
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} 
ÃD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityAAsync\PerformEntityAAsyncCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityAAsyncU h
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#PerformEntityAAsyncCommandValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
PerformEntityAAsyncCommand

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#PerformEntityAAsyncCommandValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
ConcreteAttr '
)' (
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
BaseAttr #
)# $
. 
NotNull 
( 
) 
; 
} 	
} 
} ˘
 D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityAAsync\PerformEntityAAsyncCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityAAsyncU h
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
class -
!PerformEntityAAsyncCommandHandler 2
:3 4
IRequestHandler5 D
<D E&
PerformEntityAAsyncCommandE _
>_ `
{ 
private 
readonly #
IExtensiveDomainService 0#
_extensiveDomainService1 H
;H I
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!PerformEntityAAsyncCommandHandler 0
(0 1#
IExtensiveDomainService1 H"
extensiveDomainServiceI _
)_ `
{ 	#
_extensiveDomainService #
=$ %"
extensiveDomainService& <
;< =
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !&
PerformEntityAAsyncCommand! ;
request< C
,C D
CancellationTokenE V
cancellationTokenW h
)h i
{ 	
await #
_extensiveDomainService )
.) *
PerformEntityAAsync* =
(= >
new> A
ConcreteEntityAB Q
{ 
ConcreteAttr 
= 
request &
.& '
ConcreteAttr' 3
,3 4
BaseAttr 
= 
request "
." #
BaseAttr# +
}   
,   
cancellationToken    
)    !
;  ! "
}!! 	
}"" 
}## ‚
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ExtensiveDomainServices\PerformEntityAAsync\PerformEntityAAsyncCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =#
ExtensiveDomainServices= T
.T U
PerformEntityAAsyncU h
{		 
public

 

class

 &
PerformEntityAAsyncCommand

 +
:

, -
IRequest

. 6
,

6 7
ICommand

8 @
{ 
public &
PerformEntityAAsyncCommand )
() *
string* 0
concreteAttr1 =
,= >
string? E
baseAttrF N
)N O
{ 	
ConcreteAttr 
= 
concreteAttr '
;' (
BaseAttr 
= 
baseAttr 
;  
} 	
public 
string 
ConcreteAttr "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
BaseAttr 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} ó1
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\EventHandlers\Customers\NewQuoteCreatedHandler.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
EventHandlers= J
.J K
	CustomersK T
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class "
NewQuoteCreatedHandler '
:( ) 
INotificationHandler* >
<> ?#
DomainEventNotification? V
<V W
NewQuoteCreatedW f
>f g
>g h
{ 
private 
readonly 
IUserRepository (
_userRepository) 8
;8 9
private 
readonly 
IPersonService '
_personService( 6
;6 7
private 
readonly 
	IEventBus "
	_eventBus# ,
;, -
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public "
NewQuoteCreatedHandler %
(% &
IUserRepository& 5
userRepository6 D
,D E
IPersonServiceF T
personServiceU b
,b c
	IEventBusd m
eventBusn v
)v w
{ 	
_userRepository 
= 
userRepository ,
;, -
_personService 
= 
personService *
;* +
	_eventBus   
=   
eventBus    
;    !
}!! 	
[## 	
IntentManaged##	 
(## 
Mode## 
.## 
Fully## !
,##! "
Body### '
=##( )
Mode##* .
.##. /
Fully##/ 4
)##4 5
]##5 6
public$$ 
async$$ 
Task$$ 
Handle$$  
($$  !#
DomainEventNotification%% #
<%%# $
NewQuoteCreated%%$ 3
>%%3 4
notification%%5 A
,%%A B
CancellationToken&& 
cancellationToken&& /
)&&/ 0
{'' 	
var(( 
user(( 
=(( 
await(( 
_userRepository(( ,
.((, -
FindByIdAsync((- :
(((: ;
notification((; G
.((G H
DomainEvent((H S
.((S T
Quote((T Y
.((Y Z
PersonId((Z b
,((b c
cancellationToken((d u
)((u v
;((v w
if)) 
()) 
user)) 
is)) 
null)) 
))) 
{** 
throw++ 
new++ 
NotFoundException++ +
(+++ ,
$"++, .
$str++. C
{++C D
notification++D P
.++P Q
DomainEvent++Q \
.++\ ]
Quote++] b
.++b c
PersonId++c k
}++k l
$str++l m
"++m n
)++n o
;++o p
},, 
var-- 
result-- 
=-- 
await-- 
_personService-- -
.--- .
GetPersonById--. ;
(--; <
notification--< H
.--H I
DomainEvent--I T
.--T U
Quote--U Z
.--Z [
PersonId--[ c
,--c d
cancellationToken--e v
)--v w
;--w x
user// 
.// 
Email// 
=// 
result// 
.//  
Email//  %
;//% &
user00 
.00 
Name00 
=00 
result00 
.00 
Name00 #
;00# $
user11 
.11 
Surname11 
=11 
result11 !
.11! "
Surname11" )
;11) *
user22 
.22 
QuoteId22 
=22 
notification22 '
.22' (
DomainEvent22( 3
.223 4
Quote224 9
.229 :
Id22: <
;22< =
var33 
domainEvent33 
=33 
notification33 *
.33* +
DomainEvent33+ 6
;336 7
	_eventBus44 
.44 
Publish44 
(44 
new44 !(
QuoteCreatedIntegrationEvent44" >
{55 
Id66 
=66 
domainEvent66  
.66  !
Quote66! &
.66& '
Id66' )
,66) *
RefNo77 
=77 
domainEvent77 #
.77# $
Quote77$ )
.77) *
RefNo77* /
,77/ 0
PersonId88 
=88 
domainEvent88 &
.88& '
Quote88' ,
.88, -
PersonId88- 5
,885 6
PersonEmail99 
=99 
domainEvent99 )
.99) *
Quote99* /
.99/ 0
PersonEmail990 ;
,99; <

QuoteLines:: 
=:: 
domainEvent:: (
.::( )
Quote::) .
.::. /

QuoteLines::/ 9
.;; 
Select;; 
(;; 
ql;; 
=>;; !
new;;" %5
)QuoteCreatedIntegrationEventQuoteLinesDto;;& O
{<< 
Id== 
=== 
ql== 
.==  
Id==  "
,==" #
	ProductId>> !
=>>" #
ql>>$ &
.>>& '
	ProductId>>' 0
}?? 
)?? 
.@@ 
ToList@@ 
(@@ 
)@@ 
}AA 
)AA 
;AA 
}BB 	
}CC 
}DD Ã
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\UpdateDomainServiceTest\UpdateDomainServiceTestCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P#
UpdateDomainServiceTestP g
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 3
'UpdateDomainServiceTestCommandValidator

 8
:

9 :
AbstractValidator

; L
<

L M*
UpdateDomainServiceTestCommand

M k
>

k l
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 3
'UpdateDomainServiceTestCommandValidator 6
(6 7
)7 8
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ú
ÕD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\UpdateDomainServiceTest\UpdateDomainServiceTestCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P#
UpdateDomainServiceTestP g
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 1
%UpdateDomainServiceTestCommandHandler 6
:7 8
IRequestHandler9 H
<H I*
UpdateDomainServiceTestCommandI g
>g h
{ 
private 
readonly (
IDomainServiceTestRepository 5(
_domainServiceTestRepository6 R
;R S
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 1
%UpdateDomainServiceTestCommandHandler 4
(4 5(
IDomainServiceTestRepository5 Q'
domainServiceTestRepositoryR m
)m n
{ 	(
_domainServiceTestRepository (
=) *'
domainServiceTestRepository+ F
;F G
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !*
UpdateDomainServiceTestCommand! ?
request@ G
,G H
CancellationTokenI Z
cancellationToken[ l
)l m
{ 	
var %
existingDomainServiceTest )
=* +
await, 1(
_domainServiceTestRepository2 N
.N O
FindByIdAsyncO \
(\ ]
request] d
.d e
Ide g
,g h
cancellationTokeni z
)z {
;{ |
if 
( %
existingDomainServiceTest )
is* ,
null- 1
)1 2
{ 
throw   
new   
NotFoundException   +
(  + ,
$"  , .
$str  . P
{  P Q
request  Q X
.  X Y
Id  Y [
}  [ \
$str  \ ]
"  ] ^
)  ^ _
;  _ `
}!! 
}## 	
}$$ 
}%% °
∆D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\UpdateDomainServiceTest\UpdateDomainServiceTestCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
DomainServiceTests		= O
.		O P#
UpdateDomainServiceTest		P g
{

 
public 

class *
UpdateDomainServiceTestCommand /
:0 1
IRequest2 :
,: ;
ICommand< D
{ 
public *
UpdateDomainServiceTestCommand -
(- .
Guid. 2
id3 5
)5 6
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ¿
ÀD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\MyOpDomainServiceTest\MyOpDomainServiceTestCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P!
MyOpDomainServiceTestP e
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 1
%MyOpDomainServiceTestCommandValidator

 6
:

7 8
AbstractValidator

9 J
<

J K(
MyOpDomainServiceTestCommand

K g
>

g h
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 1
%MyOpDomainServiceTestCommandValidator 4
(4 5
)5 6
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ∞
…D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\MyOpDomainServiceTest\MyOpDomainServiceTestCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P!
MyOpDomainServiceTestP e
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class /
#MyOpDomainServiceTestCommandHandler 4
:5 6
IRequestHandler7 F
<F G(
MyOpDomainServiceTestCommandG c
>c d
{ 
private 
readonly (
IDomainServiceTestRepository 5(
_domainServiceTestRepository6 R
;R S
private 
readonly 
IMyDomainService )
_domainService* 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#MyOpDomainServiceTestCommandHandler 2
(2 3(
IDomainServiceTestRepository3 O'
domainServiceTestRepositoryP k
,k l
IMyDomainService 
domainService *
)* +
{ 	(
_domainServiceTestRepository (
=) *'
domainServiceTestRepository+ F
;F G
_domainService 
= 
domainService *
;* +
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !(
MyOpDomainServiceTestCommand! =
request> E
,E F
CancellationTokenG X
cancellationTokenY j
)j k
{   	
var!! %
existingDomainServiceTest!! )
=!!* +
await!!, 1(
_domainServiceTestRepository!!2 N
.!!N O
FindByIdAsync!!O \
(!!\ ]
request!!] d
.!!d e
Id!!e g
,!!g h
cancellationToken!!i z
)!!z {
;!!{ |
if"" 
("" %
existingDomainServiceTest"" )
is""* ,
null""- 1
)""1 2
{## 
throw$$ 
new$$ 
NotFoundException$$ +
($$+ ,
$"$$, .
$str$$. P
{$$P Q
request$$Q X
.$$X Y
Id$$Y [
}$$[ \
$str$$\ ]
"$$] ^
)$$^ _
;$$_ `
}%% %
existingDomainServiceTest'' %
.''% &
MyOp''& *
(''* +
_domainService''+ 9
)''9 :
;'': ;
}(( 	
})) 
}** ó
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\MyOpDomainServiceTest\MyOpDomainServiceTestCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
DomainServiceTests		= O
.		O P!
MyOpDomainServiceTest		P e
{

 
public 

class (
MyOpDomainServiceTestCommand -
:. /
IRequest0 8
,8 9
ICommand: B
{ 
public (
MyOpDomainServiceTestCommand +
(+ ,
Guid, 0
id1 3
)3 4
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ∏
…D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\GetDomainServiceTests\GetDomainServiceTestsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P!
GetDomainServiceTestsP e
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#GetDomainServiceTestsQueryValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
GetDomainServiceTestsQuery

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#GetDomainServiceTestsQueryValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} É
«D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\GetDomainServiceTests\GetDomainServiceTestsQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P!
GetDomainServiceTestsP e
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class -
!GetDomainServiceTestsQueryHandler 2
:3 4
IRequestHandler5 D
<D E&
GetDomainServiceTestsQueryE _
,_ `
Lista e
<e f 
DomainServiceTestDtof z
>z {
>{ |
{ 
private 
readonly (
IDomainServiceTestRepository 5(
_domainServiceTestRepository6 R
;R S
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!GetDomainServiceTestsQueryHandler 0
(0 1(
IDomainServiceTestRepository1 M'
domainServiceTestRepositoryN i
,i j
IMapperk r
mappers y
)y z
{ 	(
_domainServiceTestRepository (
=) *'
domainServiceTestRepository+ F
;F G
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
<  
DomainServiceTestDto 3
>3 4
>4 5
Handle6 <
(< =&
GetDomainServiceTestsQuery &
request' .
,. /
CancellationToken 
cancellationToken /
)/ 0
{   	
var!! 
domainServiceTests!! "
=!!# $
await!!% *(
_domainServiceTestRepository!!+ G
.!!G H
FindAllAsync!!H T
(!!T U
cancellationToken!!U f
)!!f g
;!!g h
return"" 
domainServiceTests"" %
.""% &)
MapToDomainServiceTestDtoList""& C
(""C D
_mapper""D K
)""K L
;""L M
}## 	
}$$ 
}%% ã

¿D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\GetDomainServiceTests\GetDomainServiceTestsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
DomainServiceTests		= O
.		O P!
GetDomainServiceTests		P e
{

 
public 

class &
GetDomainServiceTestsQuery +
:, -
IRequest. 6
<6 7
List7 ;
<; < 
DomainServiceTestDto< P
>P Q
>Q R
,R S
IQueryT Z
{ 
public &
GetDomainServiceTestsQuery )
() *
)* +
{ 	
} 	
} 
}  
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\GetDomainServiceTestById\GetDomainServiceTestByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P$
GetDomainServiceTestByIdP h
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 2
&GetDomainServiceTestByIdQueryValidator

 7
:

8 9
AbstractValidator

: K
<

K L)
GetDomainServiceTestByIdQuery

L i
>

i j
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 2
&GetDomainServiceTestByIdQueryValidator 5
(5 6
)6 7
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} à
ÕD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\GetDomainServiceTestById\GetDomainServiceTestByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P$
GetDomainServiceTestByIdP h
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 0
$GetDomainServiceTestByIdQueryHandler 5
:6 7
IRequestHandler8 G
<G H)
GetDomainServiceTestByIdQueryH e
,e f 
DomainServiceTestDtog {
>{ |
{ 
private 
readonly (
IDomainServiceTestRepository 5(
_domainServiceTestRepository6 R
;R S
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 0
$GetDomainServiceTestByIdQueryHandler 3
(3 4(
IDomainServiceTestRepository4 P'
domainServiceTestRepositoryQ l
,l m
IMapper 
mapper 
) 
{ 	(
_domainServiceTestRepository (
=) *'
domainServiceTestRepository+ F
;F G
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
<  
DomainServiceTestDto .
>. /
Handle0 6
(6 7)
GetDomainServiceTestByIdQuery )
request* 1
,1 2
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" 
domainServiceTest"" !
=""" #
await""$ )(
_domainServiceTestRepository""* F
.""F G
FindByIdAsync""G T
(""T U
request""U \
.""\ ]
Id""] _
,""_ `
cancellationToken""a r
)""r s
;""s t
if## 
(## 
domainServiceTest## !
is##" $
null##% )
)##) *
{$$ 
throw%% 
new%% 
NotFoundException%% +
(%%+ ,
$"%%, .
$str%%. P
{%%P Q
request%%Q X
.%%X Y
Id%%Y [
}%%[ \
$str%%\ ]
"%%] ^
)%%^ _
;%%_ `
}&& 
return(( 
domainServiceTest(( $
.(($ %%
MapToDomainServiceTestDto((% >
(((> ?
_mapper((? F
)((F G
;((G H
})) 	
}** 
}++ ﬁ
∆D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\GetDomainServiceTestById\GetDomainServiceTestByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
DomainServiceTests		= O
.		O P$
GetDomainServiceTestById		P h
{

 
public 

class )
GetDomainServiceTestByIdQuery .
:/ 0
IRequest1 9
<9 : 
DomainServiceTestDto: N
>N O
,O P
IQueryQ W
{ 
public )
GetDomainServiceTestByIdQuery ,
(, -
Guid- 1
id2 4
)4 5
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Í
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\DomainServiceTestDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
DomainServiceTests

= O
{ 
public 

static 
class 1
%DomainServiceTestDtoMappingExtensions =
{ 
public 
static  
DomainServiceTestDto *%
MapToDomainServiceTestDto+ D
(D E
thisE I
DomainServiceTestJ [
projectFrom\ g
,g h
IMapperi p
mapperq w
)w x
=> 
mapper 
. 
Map 
<  
DomainServiceTestDto .
>. /
(/ 0
projectFrom0 ;
); <
;< =
public 
static 
List 
<  
DomainServiceTestDto /
>/ 0)
MapToDomainServiceTestDtoList1 N
(N O
thisO S
IEnumerableT _
<_ `
DomainServiceTest` q
>q r
projectFroms ~
,~ 
IMapper
Ä á
mapper
à é
)
é è
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )%
MapToDomainServiceTestDto) B
(B C
mapperC I
)I J
)J K
.K L
ToListL R
(R S
)S T
;T U
} 
} ≈
§D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\DomainServiceTestDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
DomainServiceTests

= O
{ 
public 

class  
DomainServiceTestDto %
:& '
IMapFrom( 0
<0 1
DomainServiceTest1 B
>B C
{ 
public  
DomainServiceTestDto #
(# $
)$ %
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
static  
DomainServiceTestDto *
Create+ 1
(1 2
Guid2 6
id7 9
)9 :
{ 	
return 
new  
DomainServiceTestDto +
{ 
Id 
= 
id 
} 
; 
} 	
public 
void 
Mapping 
( 
Profile #
profile$ +
)+ ,
{ 	
profile 
. 
	CreateMap 
< 
DomainServiceTest /
,/ 0 
DomainServiceTestDto1 E
>E F
(F G
)G H
;H I
} 	
}   
}!! Ã
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\DeleteDomainServiceTest\DeleteDomainServiceTestCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P#
DeleteDomainServiceTestP g
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 3
'DeleteDomainServiceTestCommandValidator

 8
:

9 :
AbstractValidator

; L
<

L M*
DeleteDomainServiceTestCommand

M k
>

k l
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 3
'DeleteDomainServiceTestCommandValidator 6
(6 7
)7 8
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ω
ÕD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\DeleteDomainServiceTest\DeleteDomainServiceTestCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P#
DeleteDomainServiceTestP g
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
class 1
%DeleteDomainServiceTestCommandHandler 6
:7 8
IRequestHandler9 H
<H I*
DeleteDomainServiceTestCommandI g
>g h
{ 
private 
readonly (
IDomainServiceTestRepository 5(
_domainServiceTestRepository6 R
;R S
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 1
%DeleteDomainServiceTestCommandHandler 4
(4 5(
IDomainServiceTestRepository5 Q'
domainServiceTestRepositoryR m
)m n
{ 	(
_domainServiceTestRepository (
=) *'
domainServiceTestRepository+ F
;F G
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !*
DeleteDomainServiceTestCommand! ?
request@ G
,G H
CancellationTokenI Z
cancellationToken[ l
)l m
{ 	
var %
existingDomainServiceTest )
=* +
await, 1(
_domainServiceTestRepository2 N
.N O
FindByIdAsyncO \
(\ ]
request] d
.d e
Ide g
,g h
cancellationTokeni z
)z {
;{ |
if 
( %
existingDomainServiceTest )
is* ,
null- 1
)1 2
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. P
{P Q
requestQ X
.X Y
IdY [
}[ \
$str\ ]
"] ^
)^ _
;_ `
}   (
_domainServiceTestRepository"" (
.""( )
Remove"") /
(""/ 0%
existingDomainServiceTest""0 I
)""I J
;""J K
}## 	
}$$ 
}%% °
∆D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\DeleteDomainServiceTest\DeleteDomainServiceTestCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
DomainServiceTests		= O
.		O P#
DeleteDomainServiceTest		P g
{

 
public 

class *
DeleteDomainServiceTestCommand /
:0 1
IRequest2 :
,: ;
ICommand< D
{ 
public *
DeleteDomainServiceTestCommand -
(- .
Guid. 2
id3 5
)5 6
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ã
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\CreateDomainServiceTest\CreateDomainServiceTestCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P#
CreateDomainServiceTestP g
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 3
'CreateDomainServiceTestCommandValidator

 8
:

9 :
AbstractValidator

; L
<

L M*
CreateDomainServiceTestCommand

M k
>

k l
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 3
'CreateDomainServiceTestCommandValidator 6
(6 7
)7 8
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} º
ÕD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\CreateDomainServiceTest\CreateDomainServiceTestCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
DomainServiceTests= O
.O P#
CreateDomainServiceTestP g
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 1
%CreateDomainServiceTestCommandHandler 6
:7 8
IRequestHandler9 H
<H I*
CreateDomainServiceTestCommandI g
,g h
Guidi m
>m n
{ 
private 
readonly (
IDomainServiceTestRepository 5(
_domainServiceTestRepository6 R
;R S
private 
readonly 
IMyDomainService )
_domainService* 8
;8 9
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 1
%CreateDomainServiceTestCommandHandler 4
(4 5(
IDomainServiceTestRepository5 Q'
domainServiceTestRepositoryR m
,m n
IMyDomainService 
domainService *
)* +
{ 	(
_domainServiceTestRepository (
=) *'
domainServiceTestRepository+ F
;F G
_domainService 
= 
domainService *
;* +
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '*
CreateDomainServiceTestCommand' E
requestF M
,M N
CancellationTokenO `
cancellationTokena r
)r s
{   	
var!!  
newDomainServiceTest!! $
=!!% &
new!!' *
DomainServiceTest!!+ <
(!!< =
_domainService!!= K
)!!K L
;!!L M(
_domainServiceTestRepository## (
.##( )
Add##) ,
(##, - 
newDomainServiceTest##- A
)##A B
;##B C
await$$ (
_domainServiceTestRepository$$ .
.$$. /

UnitOfWork$$/ 9
.$$9 :
SaveChangesAsync$$: J
($$J K
cancellationToken$$K \
)$$\ ]
;$$] ^
return%%  
newDomainServiceTest%% '
.%%' (
Id%%( *
;%%* +
}&& 	
}'' 
}(( ›	
∆D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DomainServiceTests\CreateDomainServiceTest\CreateDomainServiceTestCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
DomainServiceTests		= O
.		O P#
CreateDomainServiceTest		P g
{

 
public 

class *
CreateDomainServiceTestCommand /
:0 1
IRequest2 :
<: ;
Guid; ?
>? @
,@ A
ICommandB J
{ 
public *
CreateDomainServiceTestCommand -
(- .
). /
{ 	
} 	
} 
} ¢3
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\DependencyInjection.cs
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

IntentTemplate 
( 
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
{ 
public 

static 
class 
DependencyInjection +
{ 
public 
static 
IServiceCollection (
AddApplication) 7
(7 8
this8 <
IServiceCollection= O
servicesP X
,X Y
IConfigurationZ h
configurationi v
)v w
{ 	
services 
. %
AddValidatorsFromAssembly .
(. /
Assembly/ 7
.7 8 
GetExecutingAssembly8 L
(L M
)M N
,N O
lifetimeP X
:X Y
ServiceLifetimeZ i
.i j
	Transientj s
)s t
;t u
services   
.   

AddMediatR   
(    
cfg    #
=>  $ &
{!! 
cfg"" 
."" (
RegisterServicesFromAssembly"" 0
(""0 1
Assembly""1 9
.""9 : 
GetExecutingAssembly"": N
(""N O
)""O P
)""P Q
;""Q R
cfg## 
.## 
AddOpenBehavior## #
(### $
typeof##$ *
(##* +'
UnhandledExceptionBehaviour##+ F
<##F G
,##G H
>##H I
)##I J
)##J K
;##K L
cfg$$ 
.$$ 
AddOpenBehavior$$ #
($$# $
typeof$$$ *
($$* + 
PerformanceBehaviour$$+ ?
<$$? @
,$$@ A
>$$A B
)$$B C
)$$C D
;$$D E
cfg%% 
.%% 
AddOpenBehavior%% #
(%%# $
typeof%%$ *
(%%* +"
AuthorizationBehaviour%%+ A
<%%A B
,%%B C
>%%C D
)%%D E
)%%E F
;%%F G
cfg&& 
.&& 
AddOpenBehavior&& #
(&&# $
typeof&&$ *
(&&* +$
EventBusPublishBehaviour&&+ C
<&&C D
,&&D E
>&&E F
)&&F G
)&&G H
;&&H I
cfg'' 
.'' 
AddOpenBehavior'' #
(''# $
typeof''$ *
(''* +
ValidationBehaviour''+ >
<''> ?
,''? @
>''@ A
)''A B
)''B C
;''C D
cfg(( 
.(( 
AddOpenBehavior(( #
(((# $
typeof(($ *
(((* +
UnitOfWorkBehaviour((+ >
<((> ?
,((? @
>((@ A
)((A B
)((B C
;((C D
})) 
))) 
;)) 
services** 
.** 
AddAutoMapper** "
(**" #
Assembly**# +
.**+ , 
GetExecutingAssembly**, @
(**@ A
)**A B
)**B C
;**C D
services++ 
.++ 
	AddScoped++ 
<++ 
IValidatorProvider++ 1
,++1 2
ValidatorProvider++3 D
>++D E
(++E F
)++F G
;++G H
services,, 
.,, 
AddTransient,, !
<,,! "
IValidationService,," 4
,,,4 5
ValidationService,,6 G
>,,G H
(,,H I
),,I J
;,,J K
services-- 
.-- 
AddTransient-- !
<--! "
ICustomerManager--" 2
,--2 3
CustomerManager--4 C
>--C D
(--D E
)--E F
;--F G
services.. 
... 
AddTransient.. !
<..! "
IPricingService.." 1
,..1 2
PricingService..3 A
>..A B
(..B C
)..C D
;..D E
services// 
.// 
AddTransient// !
<//! "
IMyDomainService//" 2
,//2 3
MyDomainService//4 C
>//C D
(//D E
)//E F
;//F G
services00 
.00 
AddTransient00 !
<00! "#
IExtensiveDomainService00" 9
,009 :"
ExtensiveDomainService00; Q
>00Q R
(00R S
)00S T
;00T U
services11 
.11 
AddTransient11 !
<11! "-
!IClassicDomainServiceTestsService11" C
,11C D,
 ClassicDomainServiceTestsService11E e
>11e f
(11f g
)11g h
;11h i
services22 
.22 
AddTransient22 !
<22! "
IPagingTSService22" 2
,222 3
PagingTSService224 C
>22C D
(22D E
)22E F
;22F G
services33 
.33 
AddTransient33 !
<33! "
IProductsService33" 2
,332 3
ProductsService334 C
>33C D
(33D E
)33E F
;33F G
services44 
.44 
AddTransient44 !
<44! ""
IUploadDownloadService44" 8
,448 9!
UploadDownloadService44: O
>44O P
(44P Q
)44Q R
;44R S
services55 
.55 
AddTransient55 !
<55! "
IPersonService55" 0
,550 1
PersonService552 ?
>55? @
(55@ A
)55A B
;55B C
services66 
.66 
AddTransient66 !
<66! "$
IIntegrationEventHandler66" :
<66: ;(
QuoteCreatedIntegrationEvent66; W
>66W X
,66X Y/
#QuoteCreatedIntegrationEventHandler66Z }
>66} ~
(66~ 
)	66 Ä
;
66Ä Å
services77 
.77 
AddTransient77 !
<77! "$
IIntegrationEventHandler77" :
<77: ;
EnumSampleEvent77; J
>77J K
,77K L
EnumSampleHandler77M ^
>77^ _
(77_ `
)77` a
;77a b
return88 
services88 
;88 
}99 	
}:: 
};; –
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UserDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
	Customers

= F
{ 
public 

static 
class $
UserDtoMappingExtensions 0
{ 
public 
static 
UserDto 
MapToUserDto *
(* +
this+ /
User0 4
projectFrom5 @
,@ A
IMapperB I
mapperJ P
)P Q
=> 
mapper 
. 
Map 
< 
UserDto !
>! "
(" #
projectFrom# .
). /
;/ 0
public 
static 
List 
< 
UserDto "
>" #
MapToUserDtoList$ 4
(4 5
this5 9
IEnumerable: E
<E F
UserF J
>J K
projectFromL W
,W X
IMapperY `
mappera g
)g h
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToUserDto) 5
(5 6
mapper6 <
)< =
)= >
.> ?
ToList? E
(E F
)F G
;G H
} 
} ™
éD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UserDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
	Customers

= F
{ 
public 

class 
UserDto 
: 
IMapFrom #
<# $
User$ (
>( )
{ 
public 
UserDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 

QuoteRefNo 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 

QuoteRefNo  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
static 
UserDto 
Create $
($ %
Guid% )
id* ,
,, -
string. 4
name5 9
,9 :
string; A
surnameB I
,I J
stringK Q

quoteRefNoR \
)\ ]
{ 	
return 
new 
UserDto 
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Surname   
=   
surname   !
,  ! "

QuoteRefNo!! 
=!! 

quoteRefNo!! '
}"" 
;"" 
}## 	
public%% 
void%% 
Mapping%% 
(%% 
Profile%% #
profile%%$ +
)%%+ ,
{&& 	
profile'' 
.'' 
	CreateMap'' 
<'' 
User'' "
,''" #
UserDto''$ +
>''+ ,
('', -
)''- .
.(( 
	ForMember(( 
((( 
d(( 
=>(( 
d((  !
.((! "

QuoteRefNo((" ,
,((, -
opt((. 1
=>((2 4
opt((5 8
.((8 9
MapFrom((9 @
(((@ A
src((A D
=>((E G
src((H K
.((K L
Quote((L Q
.((Q R
RefNo((R W
)((W X
)((X Y
;((Y Z
})) 	
}** 
}++ ≠
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UpdateCustomer\UpdateCustomerCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
UpdateCustomerG U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
UpdateCustomerCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
UpdateCustomerCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
UpdateCustomerCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} Ü
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UpdateCustomer\UpdateCustomerCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
UpdateCustomerG U
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
class (
UpdateCustomerCommandHandler -
:. /
IRequestHandler0 ?
<? @!
UpdateCustomerCommand@ U
>U V
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
UpdateCustomerCommandHandler +
(+ ,
ICustomerRepository, ?
customerRepository@ R
)R S
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !!
UpdateCustomerCommand! 6
request7 >
,> ?
CancellationToken@ Q
cancellationTokenR c
)c d
{ 	
var 
customer 
= 
await  
_customerRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
if 
( 
customer 
is 
null  
)  !
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. G
{G H
requestH O
.O P
IdP R
}R S
$strS T
"T U
)U V
;V W
}   
customer"" 
."" 
Name"" 
="" 
request"" #
.""# $
Name""$ (
;""( )
customer## 
.## 
Surname## 
=## 
request## &
.##& '
Surname##' .
;##. /
}$$ 	
}%% 
}&& ß
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UpdateCustomer\UpdateCustomerCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
UpdateCustomer		G U
{

 
public 

class !
UpdateCustomerCommand &
:' (
IRequest) 1
,1 2
ICommand3 ;
{ 
public !
UpdateCustomerCommand $
($ %
Guid% )
id* ,
,, -
string. 4
name5 9
,9 :
string; A
surnameB I
,I J
boolK O
isActiveP X
)X Y
{ 	
Id 
= 
id 
; 
Name 
= 
name 
; 
Surname 
= 
surname 
; 
IsActive 
= 
isActive 
;  
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} –
ÿD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UpdateCorporateFuneralCoverQuote\UpdateCorporateFuneralCoverQuoteCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G,
 UpdateCorporateFuneralCoverQuoteG g
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 <
0UpdateCorporateFuneralCoverQuoteCommandValidator

 A
:

B C
AbstractValidator

D U
<

U V3
'UpdateCorporateFuneralCoverQuoteCommand

V }
>

} ~
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public <
0UpdateCorporateFuneralCoverQuoteCommandValidator ?
(? @
)@ A
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
RefNo  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 

QuoteLines %
)% &
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
	Corporate $
)$ %
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Registration '
)' (
. 
NotNull 
( 
) 
; 
} 	
}   
}!! ˙"
÷D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UpdateCorporateFuneralCoverQuote\UpdateCorporateFuneralCoverQuoteCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G,
 UpdateCorporateFuneralCoverQuoteG g
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class :
.UpdateCorporateFuneralCoverQuoteCommandHandler ?
:@ A
IRequestHandlerB Q
<Q R3
'UpdateCorporateFuneralCoverQuoteCommandR y
>y z
{ 
private 
readonly 1
%ICorporateFuneralCoverQuoteRepository >1
%_corporateFuneralCoverQuoteRepository? d
;d e
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public :
.UpdateCorporateFuneralCoverQuoteCommandHandler =
(= >1
%ICorporateFuneralCoverQuoteRepository> c1
$corporateFuneralCoverQuoteRepository	d à
)
à â
{ 	1
%_corporateFuneralCoverQuoteRepository 1
=2 30
$corporateFuneralCoverQuoteRepository4 X
;X Y
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !3
'UpdateCorporateFuneralCoverQuoteCommand! H
requestI P
,P Q
CancellationTokenR c
cancellationTokend u
)u v
{ 	
var &
corporateFuneralCoverQuote *
=+ ,
await- 21
%_corporateFuneralCoverQuoteRepository3 X
.X Y
FindByIdAsyncY f
(f g
requestg n
.n o
Ido q
,q r
cancellationToken	s Ñ
)
Ñ Ö
;
Ö Ü
if 
( &
corporateFuneralCoverQuote *
is+ -
null. 2
)2 3
{   
throw!! 
new!! 
NotFoundException!! +
(!!+ ,
$"!!, .
$str!!. Y
{!!Y Z
request!!Z a
.!!a b
Id!!b d
}!!d e
$str!!e f
"!!f g
)!!g h
;!!h i
}"" &
corporateFuneralCoverQuote$$ &
.$$& '
	Corporate$$' 0
=$$1 2
request$$3 :
.$$: ;
	Corporate$$; D
;$$D E&
corporateFuneralCoverQuote%% &
.%%& '
Amount%%' -
=%%. /
request%%0 7
.%%7 8
Amount%%8 >
;%%> ?&
corporateFuneralCoverQuote&& &
.&&& '
RefNo&&' ,
=&&- .
request&&/ 6
.&&6 7
RefNo&&7 <
;&&< =&
corporateFuneralCoverQuote'' &
.''& '
PersonId''' /
=''0 1
request''2 9
.''9 :
PersonId'': B
;''B C&
corporateFuneralCoverQuote(( &
.((& '
PersonEmail((' 2
=((3 4
request((5 <
.((< =
PersonEmail((= H
;((H I&
corporateFuneralCoverQuote)) &
.))& '
Registration))' 3
=))4 5
request))6 =
.))= >
Registration))> J
;))J K&
corporateFuneralCoverQuote++ &
.++& '
NotifyQuoteCreated++' 9
(++9 :
)++: ;
;++; <
},, 	
}-- 
}.. Ô
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\UpdateCorporateFuneralCoverQuote\UpdateCorporateFuneralCoverQuoteCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
	Customers

= F
.

F G,
 UpdateCorporateFuneralCoverQuote

G g
{ 
public 

class 3
'UpdateCorporateFuneralCoverQuoteCommand 8
:9 :
IRequest; C
,C D
ICommandE M
{ 
public 3
'UpdateCorporateFuneralCoverQuoteCommand 6
(6 7
string7 =
refNo> C
,C D
Guid 
personId 
, 
string 
? 
personEmail 
,  
List 
< 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto <
>< =

quoteLines> H
,H I
string 
	corporate 
, 
string 
registration 
,  
Guid 
id 
, 
decimal 
amount 
) 
{ 	
RefNo 
= 
refNo 
; 
PersonId 
= 
personId 
;  
PersonEmail 
= 
personEmail %
;% &

QuoteLines 
= 

quoteLines #
;# $
	Corporate 
= 
	corporate !
;! "
Registration 
= 
registration '
;' (
Id 
= 
id 
; 
Amount 
= 
amount 
; 
} 	
public!! 
string!! 
RefNo!! 
{!! 
get!! !
;!!! "
set!!# &
;!!& '
}!!( )
public"" 
Guid"" 
PersonId"" 
{"" 
get"" "
;""" #
set""$ '
;""' (
}"") *
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
public$$ 
List$$ 
<$$ 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto$$ ?
>$$? @

QuoteLines$$A K
{$$L M
get$$N Q
;$$Q R
set$$S V
;$$V W
}$$X Y
public%% 
string%% 
	Corporate%% 
{%%  !
get%%" %
;%%% &
set%%' *
;%%* +
}%%, -
public&& 
string&& 
Registration&& "
{&&# $
get&&% (
;&&( )
set&&* -
;&&- .
}&&/ 0
public'' 
Guid'' 
Id'' 
{'' 
get'' 
;'' 
set'' !
;''! "
}''# $
public(( 
decimal(( 
Amount(( 
{(( 
get((  #
;((# $
set((% (
;((( )
}((* +
})) 
}** ‘
êD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\PersonDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
{ 
public 

class 
	PersonDto 
{		 
public

 
	PersonDto

 
(

 
)

 
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
Email 
= 
null 
! 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
static 
	PersonDto 
Create  &
(& '
string' -
name. 2
,2 3
string4 :
surname; B
,B C
stringD J
emailK P
)P Q
{ 	
return 
new 
	PersonDto  
{ 
Name 
= 
name 
, 
Surname 
= 
surname !
,! "
Email 
= 
email 
} 
; 
} 	
} 
} 
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomers\GetCustomersQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
GetCustomersG S
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
GetCustomersQueryValidator

 +
:

, -
AbstractValidator

. ?
<

? @
GetCustomersQuery

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
GetCustomersQueryValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ®
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomers\GetCustomersQueryHandler.cs
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
 C
,

C D
Version

E L
=

M N
$str

O T
)

T U
]

U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
GetCustomersG S
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
class $
GetCustomersQueryHandler )
:* +
IRequestHandler, ;
<; <
GetCustomersQuery< M
,M N
ListO S
<S T
CustomerDtoT _
>_ `
>` a
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
GetCustomersQueryHandler '
(' (
ICustomerRepository( ;
customerRepository< N
)N O
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
CustomerDto *
>* +
>+ ,
Handle- 3
(3 4
GetCustomersQuery4 E
requestF M
,M N
CancellationTokenO `
cancellationTokena r
)r s
{ 	
return 
await 
_customerRepository ,
., -!
FindAllProjectToAsync- B
(B C
filterExpressionC S
:S T
nullU Y
,Y Z
filterProjection[ k
:k l
requestm t
.t u
	Transformu ~
,~ 
cancellationToken
Ä ë
:
ë í
cancellationToken
ì §
)
§ •
;
• ¶
} 	
} 
} €
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomers\GetCustomersQuery.cs
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
$str		 B
,		B C
Version		D K
=		L M
$str		N S
)		S T
]		T U
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
GetCustomersG S
{ 
public 

class 
GetCustomersQuery "
:# $
IRequest% -
<- .
List. 2
<2 3
CustomerDto3 >
>> ?
>? @
,@ A
IQueryB H
{ 
public 
GetCustomersQuery  
(  !
Func! %
<% &

IQueryable& 0
<0 1
CustomerDto1 <
>< =
,= >

IQueryable? I
>I J
	transformK T
)T U
{ 	
	Transform 
= 
	transform !
;! "
} 	
public 
Func 
< 

IQueryable 
< 
CustomerDto *
>* +
,+ ,

IQueryable- 7
>7 8
	Transform9 B
{C D
getE H
;H I
}J K
} 
} ¨
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersWithParams\GetCustomersWithParamsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G"
GetCustomersWithParamsG ]
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 0
$GetCustomersWithParamsQueryValidator

 5
:

6 7
AbstractValidator

8 I
<

I J'
GetCustomersWithParamsQuery

J e
>

e f
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 0
$GetCustomersWithParamsQueryValidator 3
(3 4
)4 5
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ∂%
¿D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersWithParams\GetCustomersWithParamsQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G"
GetCustomersWithParamsG ]
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class .
"GetCustomersWithParamsQueryHandler 3
:4 5
IRequestHandler6 E
<E F'
GetCustomersWithParamsQueryF a
,a b
Listc g
<g h
CustomerDtoh s
>s t
>t u
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"GetCustomersWithParamsQueryHandler 1
(1 2
ICustomerRepository2 E
customerRepositoryF X
,X Y
IMapperZ a
mapperb h
)h i
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
CustomerDto *
>* +
>+ ,
Handle- 3
(3 4'
GetCustomersWithParamsQuery   '
request  ( /
,  / 0
CancellationToken!! 
cancellationToken!! /
)!!/ 0
{"" 	

IQueryable## 
<## 
Customer## 
>##  
FilterCustomers##! 0
(##0 1

IQueryable##1 ;
<##; <
Customer##< D
>##D E
	queryable##F O
)##O P
{$$ 
	queryable%% 
=%% 
	queryable%% %
.%%% &
Where%%& +
(%%+ ,
x%%, -
=>%%. 0
x%%1 2
.%%2 3
IsActive%%3 ;
==%%< >
request%%? F
.%%F G
IsActive%%G O
)%%O P
;%%P Q
if'' 
('' 
request'' 
.'' 
Name''  
!=''! #
null''$ (
)''( )
{(( 
	queryable)) 
=)) 
	queryable))  )
.))) *
Where))* /
())/ 0
x))0 1
=>))2 4
x))5 6
.))6 7
Name))7 ;
==))< >
request))? F
.))F G
Name))G K
)))K L
;))L M
}** 
if,, 
(,, 
request,, 
.,, 
Surname,, #
!=,,$ &
null,,' +
),,+ ,
{-- 
	queryable.. 
=.. 
	queryable..  )
...) *
Where..* /
(../ 0
x..0 1
=>..2 4
x..5 6
...6 7
Surname..7 >
==..? A
request..B I
...I J
Surname..J Q
)..Q R
;..R S
}// 
return00 
	queryable00  
;00  !
}11 
var33 
	customers33 
=33 
await33 !
_customerRepository33" 5
.335 6
FindAllAsync336 B
(33B C
FilterCustomers33C R
,33R S
cancellationToken33T e
)33e f
;33f g
return44 
	customers44 
.44  
MapToCustomerDtoList44 1
(441 2
_mapper442 9
)449 :
;44: ;
}55 	
}66 
}77 Á
πD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersWithParams\GetCustomersWithParamsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G"
GetCustomersWithParams		G ]
{

 
public 

class '
GetCustomersWithParamsQuery ,
:- .
IRequest/ 7
<7 8
List8 <
<< =
CustomerDto= H
>H I
>I J
,J K
IQueryL R
{ 
public '
GetCustomersWithParamsQuery *
(* +
bool+ /
isActive0 8
,8 9
string: @
?@ A
nameB F
,F G
stringH N
?N O
surnameP W
)W X
{ 	
IsActive 
= 
isActive 
;  
Name 
= 
name 
; 
Surname 
= 
surname 
; 
} 	
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Surname 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} ¶
¿D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomerStatistics\GetCustomerStatisticsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G!
GetCustomerStatisticsG \
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#GetCustomerStatisticsQueryValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
GetCustomerStatisticsQuery

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#GetCustomerStatisticsQueryValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ó
æD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomerStatistics\GetCustomerStatisticsQueryHandler.cs
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
$str		 C
,		C D
Version		E L
=		M N
$str		O T
)		T U
]		U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G!
GetCustomerStatisticsG \
{ 
[ 
IntentManaged 
( 
Mode 
. 
Fully 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class -
!GetCustomerStatisticsQueryHandler 2
:3 4
IRequestHandler5 D
<D E&
GetCustomerStatisticsQueryE _
,_ `
inta d
>d e
{ 
private 
readonly 
ICustomerManager )
_customerManager* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
public -
!GetCustomerStatisticsQueryHandler 0
(0 1
ICustomerManager1 A
customerManagerB Q
)Q R
{ 	
_customerManager 
= 
customerManager .
;. /
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
int 
> 
Handle %
(% &&
GetCustomerStatisticsQuery& @
requestA H
,H I
CancellationTokenJ [
cancellationToken\ m
)m n
{ 	
var 
result 
= 
_customerManager )
.) *!
GetCustomerStatistics* ?
(? @
request@ G
.G H

CustomerIdH R
)R S
;S T
throw 
new #
NotImplementedException -
(- .
$str. P
)P Q
;Q R
} 	
}   
}!! Ã
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomerStatistics\GetCustomerStatisticsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G!
GetCustomerStatistics		G \
{

 
public 

class &
GetCustomerStatisticsQuery +
:, -
IRequest. 6
<6 7
int7 :
>: ;
,; <
IQuery= C
{ 
public &
GetCustomerStatisticsQuery )
() *
Guid* .

customerId/ 9
)9 :
{ 	

CustomerId 
= 

customerId #
;# $
} 	
public 
Guid 

CustomerId 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} œ
¿D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersPaginated\GetCustomersPaginatedQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G!
GetCustomersPaginatedG \
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 /
#GetCustomersPaginatedQueryValidator

 4
:

5 6
AbstractValidator

7 H
<

H I&
GetCustomersPaginatedQuery

I c
>

c d
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public /
#GetCustomersPaginatedQueryValidator 2
(2 3
)3 4
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} ˝
æD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersPaginated\GetCustomersPaginatedQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G!
GetCustomersPaginatedG \
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class -
!GetCustomersPaginatedQueryHandler 2
:3 4
IRequestHandler5 D
<D E&
GetCustomersPaginatedQueryE _
,_ `
PagedResulta l
<l m
CustomerDtom x
>x y
>y z
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public -
!GetCustomersPaginatedQueryHandler 0
(0 1
ICustomerRepository1 D
customerRepositoryE W
,W X
IMapperY `
mappera g
)g h
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
PagedResult %
<% &
CustomerDto& 1
>1 2
>2 3
Handle4 :
(: ;&
GetCustomersPaginatedQuery &
request' .
,. /
CancellationToken 
cancellationToken /
)/ 0
{   	
var!! 
	customers!! 
=!! 
await!! !
_customerRepository!!" 5
.!!5 6
FindAllAsync!!6 B
(!!B C
x!!C D
=>!!E G
x!!H I
.!!I J
Name!!J N
==!!O Q
request!!R Y
.!!Y Z
Name!!Z ^
&&!!_ a
x!!b c
.!!c d
Surname!!d k
==!!l n
request!!o v
.!!v w
Surname!!w ~
&&	!! Å
x
!!Ç É
.
!!É Ñ
IsActive
!!Ñ å
==
!!ç è
request
!!ê ó
.
!!ó ò
IsActive
!!ò †
,
!!† °
request
!!¢ ©
.
!!© ™
PageNo
!!™ ∞
,
!!∞ ±
request
!!≤ π
.
!!π ∫
PageSize
!!∫ ¬
,
!!¬ √
cancellationToken
!!ƒ ’
)
!!’ ÷
;
!!÷ ◊
return"" 
	customers"" 
."" 
MapToPagedResult"" -
(""- .
x"". /
=>""0 2
x""3 4
.""4 5
MapToCustomerDto""5 E
(""E F
_mapper""F M
)""M N
)""N O
;""O P
}## 	
}$$ 
}%% ◊
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersPaginated\GetCustomersPaginatedQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G!
GetCustomersPaginated		G \
{

 
public 

class &
GetCustomersPaginatedQuery +
:, -
IRequest. 6
<6 7
PagedResult7 B
<B C
CustomerDtoC N
>N O
>O P
,P Q
IQueryR X
{ 
public &
GetCustomersPaginatedQuery )
() *
bool* .
isActive/ 7
,7 8
string9 ?
name@ D
,D E
stringF L
surnameM T
,T U
intV Y
pageNoZ `
,` a
intb e
pageSizef n
)n o
{ 	
IsActive 
= 
isActive 
;  
Name 
= 
name 
; 
Surname 
= 
surname 
; 
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
} 	
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
PageNo 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
} 
} €
“D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersPaginatedWithOrder\GetCustomersPaginatedWithOrderQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G*
GetCustomersPaginatedWithOrderG e
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 8
,GetCustomersPaginatedWithOrderQueryValidator

 =
:

> ?
AbstractValidator

@ Q
<

Q R/
#GetCustomersPaginatedWithOrderQuery

R u
>

u v
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 8
,GetCustomersPaginatedWithOrderQueryValidator ;
(; <
)< =
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
OrderBy "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} ¡"
–D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersPaginatedWithOrder\GetCustomersPaginatedWithOrderQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G*
GetCustomersPaginatedWithOrderG e
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 6
*GetCustomersPaginatedWithOrderQueryHandler ;
:< =
IRequestHandler> M
<M N/
#GetCustomersPaginatedWithOrderQueryN q
,q r
Commons y
.y z

Pagination	z Ñ
.
Ñ Ö
PagedResult
Ö ê
<
ê ë
CustomerDto
ë ú
>
ú ù
>
ù û
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 6
*GetCustomersPaginatedWithOrderQueryHandler 9
(9 :
ICustomerRepository: M
customerRepositoryN `
,` a
IMapperb i
mapperj p
)p q
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Common  
.  !

Pagination! +
.+ ,
PagedResult, 7
<7 8
CustomerDto8 C
>C D
>D E
HandleF L
(L M/
#GetCustomersPaginatedWithOrderQuery /
request0 7
,7 8
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" 
	customers"" 
="" 
await"" !
_customerRepository""" 5
.""5 6
FindAllAsync""6 B
(""B C
x""C D
=>""E G
x""H I
.""I J
Name""J N
==""O Q
request""R Y
.""Y Z
Name""Z ^
&&""_ a
x""b c
.""c d
Surname""d k
==""l n
request""o v
.""v w
Surname""w ~
&&	"" Å
x
""Ç É
.
""É Ñ
IsActive
""Ñ å
==
""ç è
request
""ê ó
.
""ó ò
IsActive
""ò †
,
""† °
request
""¢ ©
.
""© ™
PageNo
""™ ∞
,
""∞ ±
request
""≤ π
.
""π ∫
PageSize
""∫ ¬
,
""¬ √
queryOptions
""ƒ –
=>
""— ”
queryOptions
""‘ ‡
.
""‡ ·
OrderBy
""· Ë
(
""Ë È
request
""È 
.
"" Ò
OrderBy
""Ò ¯
)
""¯ ˘
,
""˘ ˙
cancellationToken
""˚ å
)
""å ç
;
""ç é
return## 
	customers## 
.## 
MapToPagedResult## -
(##- .
x##. /
=>##0 2
x##3 4
.##4 5
MapToCustomerDto##5 E
(##E F
_mapper##F M
)##M N
)##N O
;##O P
}$$ 	
}%% 
}&& ü
…D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersPaginatedWithOrder\GetCustomersPaginatedWithOrderQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G*
GetCustomersPaginatedWithOrder		G e
{

 
public 

class /
#GetCustomersPaginatedWithOrderQuery 4
:5 6
IRequest7 ?
<? @
PagedResult@ K
<K L
CustomerDtoL W
>W X
>X Y
,Y Z
IQuery[ a
{ 
public /
#GetCustomersPaginatedWithOrderQuery 2
(2 3
bool3 7
isActive8 @
,@ A
string 
name 
, 
string 
surname 
, 
int 
pageNo 
, 
int 
pageSize 
, 
string 
orderBy 
) 
{ 	
IsActive 
= 
isActive 
;  
Name 
= 
name 
; 
Surname 
= 
surname 
; 
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
OrderBy 
= 
orderBy 
; 
} 	
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
PageNo 
{ 
get 
;  
set! $
;$ %
}& '
public   
int   
PageSize   
{   
get   !
;  ! "
set  # &
;  & '
}  ( )
public!! 
string!! 
OrderBy!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
}"" 
}## ˘
ŒD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersByNameAndSurname\GetCustomersByNameAndSurnameQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G(
GetCustomersByNameAndSurnameG c
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 6
*GetCustomersByNameAndSurnameQueryValidator

 ;
:

< =
AbstractValidator

> O
<

O P-
!GetCustomersByNameAndSurnameQuery

P q
>

q r
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 6
*GetCustomersByNameAndSurnameQueryValidator 9
(9 :
): ;
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} Ô
ÃD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersByNameAndSurname\GetCustomersByNameAndSurnameQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G(
GetCustomersByNameAndSurnameG c
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 4
(GetCustomersByNameAndSurnameQueryHandler 9
:: ;
IRequestHandler< K
<K L-
!GetCustomersByNameAndSurnameQueryL m
,m n
Listo s
<s t
CustomerDtot 
>	 Ä
>
Ä Å
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 4
(GetCustomersByNameAndSurnameQueryHandler 7
(7 8
ICustomerRepository8 K
customerRepositoryL ^
,^ _
IMapper` g
mapperh n
)n o
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
CustomerDto *
>* +
>+ ,
Handle- 3
(3 4-
!GetCustomersByNameAndSurnameQuery -
request. 5
,5 6
CancellationToken 
cancellationToken /
)/ 0
{   	
var!! 
	customers!! 
=!! 
await!! !
_customerRepository!!" 5
.!!5 6
FindAllAsync!!6 B
(!!B C
x!!C D
=>!!E G
x!!H I
.!!I J
Name!!J N
==!!O Q
request!!R Y
.!!Y Z
Name!!Z ^
&&!!_ a
x!!b c
.!!c d
Surname!!d k
==!!l n
request!!o v
.!!v w
Surname!!w ~
,!!~ 
cancellationToken
!!Ä ë
)
!!ë í
;
!!í ì
return"" 
	customers"" 
.""  
MapToCustomerDtoList"" 1
(""1 2
_mapper""2 9
)""9 :
;"": ;
}## 	
}$$ 
}%% Æ
≈D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomersByNameAndSurname\GetCustomersByNameAndSurnameQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G(
GetCustomersByNameAndSurname		G c
{

 
public 

class -
!GetCustomersByNameAndSurnameQuery 2
:3 4
IRequest5 =
<= >
List> B
<B C
CustomerDtoC N
>N O
>O P
,P Q
IQueryR X
{ 
public -
!GetCustomersByNameAndSurnameQuery 0
(0 1
string1 7
name8 <
,< =
string> D
surnameE L
)L M
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} Ç
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomerById\GetCustomerByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
GetCustomerByIdG V
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 )
GetCustomerByIdQueryValidator

 .
:

/ 0
AbstractValidator

1 B
<

B C 
GetCustomerByIdQuery

C W
>

W X
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
GetCustomerByIdQueryValidator ,
(, -
)- .
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ¬
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomerById\GetCustomerByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
GetCustomerByIdG V
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class '
GetCustomerByIdQueryHandler ,
:- .
IRequestHandler/ >
<> ? 
GetCustomerByIdQuery? S
,S T
CustomerDtoU `
>` a
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
GetCustomerByIdQueryHandler *
(* +
ICustomerRepository+ >
customerRepository? Q
,Q R
IMapperS Z
mapper[ a
)a b
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
CustomerDto %
>% &
Handle' -
(- . 
GetCustomerByIdQuery. B
requestC J
,J K
CancellationTokenL ]
cancellationToken^ o
)o p
{ 	
var 
customer 
= 
await  
_customerRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
if   
(   
customer   
is   
null    
)    !
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". G
{""G H
request""H O
.""O P
Id""P R
}""R S
$str""S T
"""T U
)""U V
;""V W
}## 
return$$ 
customer$$ 
.$$ 
MapToCustomerDto$$ ,
($$, -
_mapper$$- 4
)$$4 5
;$$5 6
}%% 	
}&& 
}'' ñ
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\GetCustomerById\GetCustomerByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
GetCustomerById		G V
{

 
public 

class  
GetCustomerByIdQuery %
:& '
IRequest( 0
<0 1
CustomerDto1 <
>< =
,= >
IQuery? E
{ 
public  
GetCustomerByIdQuery #
(# $
Guid$ (
id) +
)+ ,
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ñ
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\DeleteCustomer\DeleteCustomerCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
DeleteCustomerG U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
DeleteCustomerCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
DeleteCustomerCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
DeleteCustomerCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ò
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\DeleteCustomer\DeleteCustomerCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
DeleteCustomerG U
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
class (
DeleteCustomerCommandHandler -
:. /
IRequestHandler0 ?
<? @!
DeleteCustomerCommand@ U
>U V
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
DeleteCustomerCommandHandler +
(+ ,
ICustomerRepository, ?
customerRepository@ R
)R S
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !!
DeleteCustomerCommand! 6
request7 >
,> ?
CancellationToken@ Q
cancellationTokenR c
)c d
{ 	
var 
customer 
= 
await  
_customerRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
if 
( 
customer 
is 
null  
)  !
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. G
{G H
requestH O
.O P
IdP R
}R S
$strS T
"T U
)U V
;V W
}   
_customerRepository"" 
.""  
Remove""  &
(""& '
customer""' /
)""/ 0
;""0 1
}## 	
}$$ 
}%% ‚

´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\DeleteCustomer\DeleteCustomerCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
DeleteCustomer		G U
{

 
public 

class !
DeleteCustomerCommand &
:' (
IRequest) 1
,1 2
ICommand3 ;
{ 
public !
DeleteCustomerCommand $
($ %
Guid% )
id* ,
), -
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ú
ºD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\DeactivateCustomer\DeactivateCustomerCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
DeactivateCustomerG Y
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 .
"DeactivateCustomerCommandValidator

 3
:

4 5
AbstractValidator

6 G
<

G H%
DeactivateCustomerCommand

H a
>

a b
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public .
"DeactivateCustomerCommandValidator 1
(1 2
)2 3
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ƒ
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\DeactivateCustomer\DeactivateCustomerCommandHandler.cs
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
$str		 E
,		E F
Version		G N
=		O P
$str		Q V
)		V W
]		W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
DeactivateCustomerG Y
{ 
[ 
IntentManaged 
( 
Mode 
. 
Fully 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class ,
 DeactivateCustomerCommandHandler 1
:2 3
IRequestHandler4 C
<C D%
DeactivateCustomerCommandD ]
>] ^
{ 
private 
readonly 
ICustomerManager )
_customerManager* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
)! "
]" #
public ,
 DeactivateCustomerCommandHandler /
(/ 0
ICustomerManager0 @
customerManagerA P
)P Q
{ 	
_customerManager 
= 
customerManager .
;. /
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !%
DeactivateCustomerCommand! :
request; B
,B C
CancellationTokenD U
cancellationTokenV g
)g h
{ 	
await 
_customerManager "
." ##
DeactivateCustomerAsync# :
(: ;
request; B
.B C

CustomerIdC M
,M N
cancellationTokenO `
)` a
;a b
} 	
} 
} ñ
≥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\DeactivateCustomer\DeactivateCustomerCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
DeactivateCustomer		G Y
{

 
public 

class %
DeactivateCustomerCommand *
:+ ,
IRequest- 5
,5 6
ICommand7 ?
{ 
public %
DeactivateCustomerCommand (
(( )
Guid) -

customerId. 8
)8 9
{ 	

CustomerId 
= 

customerId #
;# $
} 	
public 
Guid 

CustomerId 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} ¯
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CustomerDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
	Customers

= F
{ 
public 

static 
class (
CustomerDtoMappingExtensions 4
{ 
public 
static 
CustomerDto !
MapToCustomerDto" 2
(2 3
this3 7
Customer8 @
projectFromA L
,L M
IMapperN U
mapperV \
)\ ]
=> 
mapper 
. 
Map 
< 
CustomerDto %
>% &
(& '
projectFrom' 2
)2 3
;3 4
public 
static 
List 
< 
CustomerDto &
>& ' 
MapToCustomerDtoList( <
(< =
this= A
IEnumerableB M
<M N
CustomerN V
>V W
projectFromX c
,c d
IMappere l
mapperm s
)s t
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToCustomerDto) 9
(9 :
mapper: @
)@ A
)A B
.B C
ToListC I
(I J
)J K
;K L
} 
} —"
íD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CustomerDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
	Customers

= F
{ 
public 

class 
CustomerDto 
: 
IMapFrom '
<' (
Customer( 0
>0 1
{ 
public 
CustomerDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
bool !
PreferencesNewsletter )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 
bool 
PreferencesSpecials '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 
static 
CustomerDto !
Create" (
(( )
Guid 
id 
, 
string 
name 
, 
string 
surname 
, 
bool 
isActive 
, 
bool   !
preferencesNewsletter   &
,  & '
bool!! 
preferencesSpecials!! $
)!!$ %
{"" 	
return## 
new## 
CustomerDto## "
{$$ 
Id%% 
=%% 
id%% 
,%% 
Name&& 
=&& 
name&& 
,&& 
Surname'' 
='' 
surname'' !
,''! "
IsActive(( 
=(( 
isActive(( #
,((# $!
PreferencesNewsletter)) %
=))& '!
preferencesNewsletter))( =
,))= >
PreferencesSpecials** #
=**$ %
preferencesSpecials**& 9
}++ 
;++ 
},, 	
public.. 
void.. 
Mapping.. 
(.. 
Profile.. #
profile..$ +
)..+ ,
{// 	
profile00 
.00 
	CreateMap00 
<00 
Customer00 &
,00& '
CustomerDto00( 3
>003 4
(004 5
)005 6
.11 
ForPath11 
(11 
d11 
=>11 
d11 
.11  !
PreferencesNewsletter11  5
,115 6
opt117 :
=>11; =
opt11> A
.11A B
MapFrom11B I
(11I J
src11J M
=>11N P
src11Q T
.11T U
Preferences11U `
!11` a
.11a b

Newsletter11b l
)11l m
)11m n
.22 
ForPath22 
(22 
d22 
=>22 
d22 
.22  
PreferencesSpecials22  3
,223 4
opt225 8
=>229 ;
opt22< ?
.22? @
MapFrom22@ G
(22G H
src22H K
=>22L N
src22O R
.22R S
Preferences22S ^
!22^ _
.22_ `
Specials22` h
)22h i
)22i j
;22j k
}33 	
}44 
}55 ∆
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateQuote\CreateQuoteCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
CreateQuoteG R
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
CreateQuoteCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
CreateQuoteCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
CreateQuoteCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
RefNo  
)  !
. 
NotNull 
( 
) 
; 
} 	
} 
} √
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateQuote\CreateQuoteCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
CreateQuoteG R
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
class %
CreateQuoteCommandHandler *
:+ ,
IRequestHandler- <
<< =
CreateQuoteCommand= O
>O P
{ 
private 
readonly 
IQuoteRepository )
_quoteRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
CreateQuoteCommandHandler (
(( )
IQuoteRepository) 9
quoteRepository: I
)I J
{ 	
_quoteRepository 
= 
quoteRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
CreateQuoteCommand! 3
request4 ;
,; <
CancellationToken= N
cancellationTokenO `
)` a
{ 	
var 
quote 
= 
new 
Quote !
(! "
refNo 
: 
request 
. 
RefNo $
,$ %
personId 
: 
request !
.! "
PersonId" *
,* +
personEmail 
: 
request $
.$ %
PersonEmail% 0
)0 1
;1 2
quote!! 
.!! 
NotifyQuoteCreated!! $
(!!$ %
)!!% &
;!!& '
_quoteRepository## 
.## 
Add##  
(##  !
quote##! &
)##& '
;##' (
}$$ 	
}%% 
}&& «
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateQuote\CreateQuoteCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
CreateQuote		G R
{

 
public 

class 
CreateQuoteCommand #
:$ %
IRequest& .
,. /
ICommand0 8
{ 
public 
CreateQuoteCommand !
(! "
string" (
refNo) .
,. /
Guid0 4
personId5 =
,= >
string? E
?E F
personEmailG R
)R S
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
;% &
} 	
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
PersonId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
PersonEmail "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
} Á
∆D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateFuneralCoverQuote\CreateFuneralCoverQuoteCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G#
CreateFuneralCoverQuoteG ^
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 3
'CreateFuneralCoverQuoteCommandValidator

 8
:

9 :
AbstractValidator

; L
<

L M*
CreateFuneralCoverQuoteCommand

M k
>

k l
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 3
'CreateFuneralCoverQuoteCommandValidator 6
(6 7
)7 8
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
RefNo  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 

QuoteLines %
)% &
. 
NotNull 
( 
) 
; 
} 	
} 
} ⁄"
ƒD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateFuneralCoverQuote\CreateFuneralCoverQuoteCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G#
CreateFuneralCoverQuoteG ^
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class 1
%CreateFuneralCoverQuoteCommandHandler 6
:7 8
IRequestHandler9 H
<H I*
CreateFuneralCoverQuoteCommandI g
>g h
{ 
private 
readonly (
IFuneralCoverQuoteRepository 5(
_funeralCoverQuoteRepository6 R
;R S
private 
readonly  
IProductServiceProxy - 
_productServiceProxy. B
;B C
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public 1
%CreateFuneralCoverQuoteCommandHandler 4
(4 5(
IFuneralCoverQuoteRepository5 Q'
funeralCoverQuoteRepositoryR m
,m n!
IProductServiceProxy	o É!
productServiceProxy
Ñ ó
)
ó ò
{ 	(
_funeralCoverQuoteRepository (
=) *'
funeralCoverQuoteRepository+ F
;F G 
_productServiceProxy  
=! "
productServiceProxy# 6
;6 7
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !*
CreateFuneralCoverQuoteCommand! ?
request@ G
,G H
CancellationTokenI Z
cancellationToken[ l
)l m
{ 	
var   
funeralCoverQuote   !
=  " #
new  $ '
FuneralCoverQuote  ( 9
(  9 :
refNo!! 
:!! 
request!! 
.!! 
RefNo!! $
,!!$ %
personId"" 
:"" 
request"" !
.""! "
PersonId""" *
,""* +
personEmail## 
:## 
request## $
.##$ %
PersonEmail##% 0
)##0 1
{$$ 
RefNo%% 
=%% 
request%% 
.%%  
RefNo%%  %
,%%% &

QuoteLines&& 
=&& 
request&& $
.&&$ %

QuoteLines&&% /
.'' 
Select'' 
('' 
ql'' 
=>'' !
new''" %
	QuoteLine''& /
(''/ 0
	productId(( !
:((! "
ql((# %
.((% &
	ProductId((& /
)((/ 0
{)) 
	ProductId** !
=**" #
ql**$ &
.**& '
	ProductId**' 0
}++ 
)++ 
.,, 
ToList,, 
(,, 
),, 
}-- 
;-- 
var.. 
result.. 
=.. 
await..  
_productServiceProxy.. 3
...3 4
GetProductsAsync..4 D
(..D E
cancellationToken..E V
)..V W
;..W X
funeralCoverQuote00 
.00 
NotifyQuoteCreated00 0
(000 1
)001 2
;002 3(
_funeralCoverQuoteRepository22 (
.22( )
Add22) ,
(22, -
funeralCoverQuote22- >
)22> ?
;22? @
}33 	
}44 
}55 ‘
ΩD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateFuneralCoverQuote\CreateFuneralCoverQuoteCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
	Customers

= F
.

F G#
CreateFuneralCoverQuote

G ^
{ 
public 

class *
CreateFuneralCoverQuoteCommand /
:0 1
IRequest2 :
,: ;
ICommand< D
{ 
public *
CreateFuneralCoverQuoteCommand -
(- .
string. 4
refNo5 :
,: ;
Guid 
personId 
, 
string 
? 
personEmail 
,  
List 
< 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto <
>< =

quoteLines> H
)H I
{ 	
RefNo 
= 
refNo 
; 
PersonId 
= 
personId 
;  
PersonEmail 
= 
personEmail %
;% &

QuoteLines 
= 

quoteLines #
;# $
} 	
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
PersonId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
PersonEmail "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
List 
< 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto ?
>? @

QuoteLinesA K
{L M
getN Q
;Q R
setS V
;V W
}X Y
} 
} Ï
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateFuneralCoverQuoteCommandQuoteLinesDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
{ 
public		 

class		 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto		 <
{

 
public 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto :
(: ;
); <
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
Guid 
	ProductId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto A
CreateB H
(H I
GuidI M
idN P
,P Q
GuidR V
	productIdW `
)` a
{ 	
return 
new 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto B
{ 
Id 
= 
id 
, 
	ProductId 
= 
	productId %
} 
; 
} 	
} 
} ñ
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateCustomer\CreateCustomerCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
CreateCustomerG U
{		 
[

 
IntentManaged

 
(

 
Mode

 
.

 
Fully

 
,

 
Body

 #
=

$ %
Mode

& *
.

* +
Merge

+ 0
)

0 1
]

1 2
public 

class *
CreateCustomerCommandValidator /
:0 1
AbstractValidator2 C
<C D!
CreateCustomerCommandD Y
>Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
CreateCustomerCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
. 
Length 
( 
$num 
, 
$num 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
for 
( 
int 
x 
= 
$num 
; 
x 
< 
$num !
;! "
x# $
++$ &
)& '
{ 
Console 
. 
	WriteLine !
(! "
$str" )
)) *
;* +
} 
}   	
}!! 
}"" ∫
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateCustomer\CreateCustomerCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
CreateCustomerG U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class (
CreateCustomerCommandHandler -
:. /
IRequestHandler0 ?
<? @!
CreateCustomerCommand@ U
,U V
GuidW [
>[ \
{ 
private 
readonly 
ICustomerRepository ,
_customerRepository- @
;@ A
private 
readonly "
ICustomersServiceProxy /"
_customersServiceProxy0 F
;F G
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
CreateCustomerCommandHandler +
(+ ,
ICustomerRepository, ?
customerRepository@ R
,R S"
ICustomersServiceProxyT j"
customersServiceProxy	k Ä
)
Ä Å
{ 	
_customerRepository 
=  !
customerRepository" 4
;4 5"
_customersServiceProxy "
=# $!
customersServiceProxy% :
;: ;
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '!
CreateCustomerCommand' <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
var 
customer 
= 
new 
Customer '
{   
Name!! 
=!! 
request!! 
.!! 
Name!! #
,!!# $
Surname"" 
="" 
request"" !
.""! "
Surname""" )
,"") *
IsActive## 
=## 
request## "
.##" #
IsActive### +
}$$ 
;$$ 
_customerRepository&& 
.&&  
Add&&  #
(&&# $
customer&&$ ,
)&&, -
;&&- .
await'' 
_customerRepository'' %
.''% &

UnitOfWork''& 0
.''0 1
SaveChangesAsync''1 A
(''A B
cancellationToken''B S
)''S T
;''T U
return(( 
customer(( 
.(( 
Id(( 
;(( 
})) 	
}** 
}++ ‘
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateCustomer\CreateCustomerCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
CreateCustomer		G U
{

 
public 

class !
CreateCustomerCommand &
:' (
IRequest) 1
<1 2
Guid2 6
>6 7
,7 8
ICommand9 A
{ 
public !
CreateCustomerCommand $
($ %
string% +
name, 0
,0 1
string2 8
surname9 @
,@ A
boolB F
isActiveG O
)O P
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
IsActive 
= 
isActive 
;  
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} –
ÿD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateCorporateFuneralCoverQuote\CreateCorporateFuneralCoverQuoteCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G,
 CreateCorporateFuneralCoverQuoteG g
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 <
0CreateCorporateFuneralCoverQuoteCommandValidator

 A
:

B C
AbstractValidator

D U
<

U V3
'CreateCorporateFuneralCoverQuoteCommand

V }
>

} ~
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public <
0CreateCorporateFuneralCoverQuoteCommandValidator ?
(? @
)@ A
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
RefNo  
)  !
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 

QuoteLines %
)% &
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
	Corporate $
)$ %
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Registration '
)' (
. 
NotNull 
( 
) 
; 
} 	
}   
}!! Á
÷D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateCorporateFuneralCoverQuote\CreateCorporateFuneralCoverQuoteCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G,
 CreateCorporateFuneralCoverQuoteG g
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class :
.CreateCorporateFuneralCoverQuoteCommandHandler ?
:@ A
IRequestHandlerB Q
<Q R3
'CreateCorporateFuneralCoverQuoteCommandR y
>y z
{ 
private 
readonly 1
%ICorporateFuneralCoverQuoteRepository >1
%_corporateFuneralCoverQuoteRepository? d
;d e
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public :
.CreateCorporateFuneralCoverQuoteCommandHandler =
(= >1
%ICorporateFuneralCoverQuoteRepository> c1
$corporateFuneralCoverQuoteRepository	d à
)
à â
{ 	1
%_corporateFuneralCoverQuoteRepository 1
=2 30
$corporateFuneralCoverQuoteRepository4 X
;X Y
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !3
'CreateCorporateFuneralCoverQuoteCommand! H
requestI P
,P Q
CancellationTokenR c
cancellationTokend u
)u v
{ 	
var &
corporateFuneralCoverQuote *
=+ ,
new- 0&
CorporateFuneralCoverQuote1 K
(K L
refNo 
: 
request 
. 
RefNo $
,$ %
personId 
: 
request !
.! "
PersonId" *
,* +
personEmail   
:   
request   $
.  $ %
PersonEmail  % 0
)  0 1
{!! 
	Corporate"" 
="" 
request"" #
.""# $
	Corporate""$ -
,""- .
Registration## 
=## 
request## &
.##& '
Registration##' 3
,##3 4

QuoteLines$$ 
=$$ 
request$$ $
.$$$ %

QuoteLines$$% /
.%% 
Select%% 
(%% 
ql%% 
=>%% !
new%%" %
	QuoteLine%%& /
(%%/ 0
	productId&& !
:&&! "
ql&&# %
.&&% &
	ProductId&&& /
)&&/ 0
)&&0 1
.'' 
ToList'' 
('' 
)'' 
}(( 
;(( 1
%_corporateFuneralCoverQuoteRepository** 1
.**1 2
Add**2 5
(**5 6&
corporateFuneralCoverQuote**6 P
)**P Q
;**Q R
}++ 	
},, 
}-- ”
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\CreateCorporateFuneralCoverQuote\CreateCorporateFuneralCoverQuoteCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
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
0 1
Application

1 <
.

< =
	Customers

= F
.

F G,
 CreateCorporateFuneralCoverQuote

G g
{ 
public 

class 3
'CreateCorporateFuneralCoverQuoteCommand 8
:9 :
IRequest; C
,C D
ICommandE M
{ 
public 3
'CreateCorporateFuneralCoverQuoteCommand 6
(6 7
string7 =
refNo> C
,C D
Guid 
personId 
, 
string 
? 
personEmail 
,  
List 
< 7
+CreateFuneralCoverQuoteCommandQuoteLinesDto <
>< =

quoteLines> H
,H I
string 
	corporate 
, 
string 
registration 
)  
{ 	
RefNo 
= 
refNo 
; 
PersonId 
= 
personId 
;  
PersonEmail 
= 
personEmail %
;% &

QuoteLines 
= 

quoteLines #
;# $
	Corporate 
= 
	corporate !
;! "
Registration 
= 
registration '
;' (
} 	
public 
string 
RefNo 
{ 
get !
;! "
set# &
;& '
}( )
public 
Guid 
PersonId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
PersonEmail "
{# $
get% (
;( )
set* -
;- .
}/ 0
public   
List   
<   7
+CreateFuneralCoverQuoteCommandQuoteLinesDto   ?
>  ? @

QuoteLines  A K
{  L M
get  N Q
;  Q R
set  S V
;  V W
}  X Y
public!! 
string!! 
	Corporate!! 
{!!  !
get!!" %
;!!% &
set!!' *
;!!* +
}!!, -
public"" 
string"" 
Registration"" "
{""# $
get""% (
;""( )
set""* -
;""- .
}""/ 0
}## 
}$$ ¯
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\ApproveQuote\ApproveQuoteCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
ApproveQuoteG S
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 (
ApproveQuoteCommandValidator

 -
:

. /
AbstractValidator

0 A
<

A B
ApproveQuoteCommand

B U
>

U V
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
ApproveQuoteCommandValidator +
(+ ,
), -
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ã
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\ApproveQuote\ApproveQuoteCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Customers= F
.F G
ApproveQuoteG S
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class &
ApproveQuoteCommandHandler +
:, -
IRequestHandler. =
<= >
ApproveQuoteCommand> Q
>Q R
{ 
private 
readonly 
IQuoteRepository )
_quoteRepository* :
;: ;
private 
readonly 
IPersonService '
_personService( 6
;6 7
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
ApproveQuoteCommandHandler )
() *
IQuoteRepository* :
quoteRepository; J
,J K
IPersonServiceL Z
personService[ h
)h i
{ 	
_quoteRepository 
= 
quoteRepository .
;. /
_personService 
= 
personService *
;* +
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
ApproveQuoteCommand! 4
request5 <
,< =
CancellationToken> O
cancellationTokenP a
)a b
{ 	
var 
entity 
= 
await 
_quoteRepository /
./ 0
FindByIdAsync0 =
(= >
request> E
.E F
QuoteIdF M
,M N
cancellationTokenO `
)` a
;a b
if   
(   
entity   
is   
null   
)   
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". D
{""D E
request""E L
.""L M
QuoteId""M T
}""T U
$str""U V
"""V W
)""W X
;""X Y
}## 
var$$ 
result$$ 
=$$ 
await$$ 
_personService$$ -
.$$- .
GetPersonById$$. ;
($$; <
entity$$< B
.$$B C
PersonId$$C K
,$$K L
cancellationToken$$M ^
)$$^ _
;$$_ `
entity&& 
.&& 
PersonEmail&& 
=&&  
result&&! '
.&&' (
Email&&( -
;&&- .
}'' 	
}(( 
})) Ï

ßD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Customers\ApproveQuote\ApproveQuoteCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Customers		= F
.		F G
ApproveQuote		G S
{

 
public 

class 
ApproveQuoteCommand $
:% &
IRequest' /
,/ 0
ICommand1 9
{ 
public 
ApproveQuoteCommand "
(" #
Guid# '
quoteId( /
)/ 0
{ 	
QuoteId 
= 
quoteId 
; 
} 	
public 
Guid 
QuoteId 
{ 
get !
;! "
set# &
;& '
}( )
} 
} ◊
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\UpdateContract\UpdateContractCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
UpdateContractG U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
UpdateContractCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
UpdateContractCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
UpdateContractCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} à
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\UpdateContract\UpdateContractCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
UpdateContractG U
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
class (
UpdateContractCommandHandler -
:. /
IRequestHandler0 ?
<? @!
UpdateContractCommand@ U
>U V
{ 
private 
readonly 
IContractRepository ,
_contractRepository- @
;@ A
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
UpdateContractCommandHandler +
(+ ,
IContractRepository, ?
contractRepository@ R
)R S
{ 	
_contractRepository 
=  !
contractRepository" 4
;4 5
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !!
UpdateContractCommand! 6
request7 >
,> ?
CancellationToken@ Q
cancellationTokenR c
)c d
{ 	
var 
contract 
= 
await  
_contractRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
if 
( 
contract 
is 
null  
)  !
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. G
{G H
requestH O
.O P
IdP R
}R S
$strS T
"T U
)U V
;V W
}   
contract"" 
."" 
Name"" 
="" 
request"" #
.""# $
Name""$ (
;""( )
contract## 
.## 
IsActive## 
=## 
request##  '
.##' (
IsActive##( 0
;##0 1
}$$ 	
}%% 
}&& å
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\UpdateContract\UpdateContractCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Contracts		= F
.		F G
UpdateContract		G U
{

 
public 

class !
UpdateContractCommand &
:' (
IRequest) 1
,1 2
ICommand3 ;
{ 
public !
UpdateContractCommand $
($ %
Guid% )
id* ,
,, -
string. 4
name5 9
,9 :
bool; ?
isActive@ H
)H I
{ 	
Id 
= 
id 
; 
Name 
= 
name 
; 
IsActive 
= 
isActive 
;  
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} 
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\GetContracts\GetContractsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
GetContractsG S
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
GetContractsQueryValidator

 +
:

, -
AbstractValidator

. ?
<

? @
GetContractsQuery

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
GetContractsQueryValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ®
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\GetContracts\GetContractsQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
GetContractsG S
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class $
GetContractsQueryHandler )
:* +
IRequestHandler, ;
<; <
GetContractsQuery< M
,M N
ListO S
<S T
ContractDtoT _
>_ `
>` a
{ 
private 
readonly 
IContractRepository ,
_contractRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
GetContractsQueryHandler '
(' (
IContractRepository( ;
contractRepository< N
,N O
IMapperP W
mapperX ^
)^ _
{ 	
_contractRepository 
=  !
contractRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
List 
< 
ContractDto *
>* +
>+ ,
Handle- 3
(3 4
GetContractsQuery4 E
requestF M
,M N
CancellationTokenO `
cancellationTokena r
)r s
{ 	
var 
	contracts 
= 
await !
_contractRepository" 5
.5 6
FindAllAsync6 B
(B C
xC D
=>E G
xH I
.I J
IsActiveJ R
,R S
cancellationTokenT e
)e f
;f g
return   
	contracts   
.    
MapToContractDtoList   1
(  1 2
_mapper  2 9
)  9 :
;  : ;
}!! 	
}"" 
}## √	
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\GetContracts\GetContractsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Contracts		= F
.		F G
GetContracts		G S
{

 
public 

class 
GetContractsQuery "
:# $
IRequest% -
<- .
List. 2
<2 3
ContractDto3 >
>> ?
>? @
,@ A
IQueryB H
{ 
public 
GetContractsQuery  
(  !
)! "
{ 	
} 	
} 
} Ç
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\GetContractById\GetContractByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
GetContractByIdG V
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 )
GetContractByIdQueryValidator

 .
:

/ 0
AbstractValidator

1 B
<

B C 
GetContractByIdQuery

C W
>

W X
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
GetContractByIdQueryValidator ,
(, -
)- .
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} ¬
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\GetContractById\GetContractByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
GetContractByIdG V
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class '
GetContractByIdQueryHandler ,
:- .
IRequestHandler/ >
<> ? 
GetContractByIdQuery? S
,S T
ContractDtoU `
>` a
{ 
private 
readonly 
IContractRepository ,
_contractRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
GetContractByIdQueryHandler *
(* +
IContractRepository+ >
contractRepository? Q
,Q R
IMapperS Z
mapper[ a
)a b
{ 	
_contractRepository 
=  !
contractRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
ContractDto %
>% &
Handle' -
(- . 
GetContractByIdQuery. B
requestC J
,J K
CancellationTokenL ]
cancellationToken^ o
)o p
{ 	
var 
contract 
= 
await  
_contractRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
if   
(   
contract   
is   
null    
)    !
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". G
{""G H
request""H O
.""O P
Id""P R
}""R S
$str""S T
"""T U
)""U V
;""V W
}## 
return$$ 
contract$$ 
.$$ 
MapToContractDto$$ ,
($$, -
_mapper$$- 4
)$$4 5
;$$5 6
}%% 	
}&& 
}'' ñ
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\GetContractById\GetContractByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Contracts		= F
.		F G
GetContractById		G V
{

 
public 

class  
GetContractByIdQuery %
:& '
IRequest( 0
<0 1
ContractDto1 <
>< =
,= >
IQuery? E
{ 
public  
GetContractByIdQuery #
(# $
Guid$ (
id) +
)+ ,
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ñ
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\DeleteContract\DeleteContractCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
DeleteContractG U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
DeleteContractCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
DeleteContractCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
DeleteContractCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ò
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\DeleteContract\DeleteContractCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
DeleteContractG U
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
class (
DeleteContractCommandHandler -
:. /
IRequestHandler0 ?
<? @!
DeleteContractCommand@ U
>U V
{ 
private 
readonly 
IContractRepository ,
_contractRepository- @
;@ A
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
DeleteContractCommandHandler +
(+ ,
IContractRepository, ?
contractRepository@ R
)R S
{ 	
_contractRepository 
=  !
contractRepository" 4
;4 5
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !!
DeleteContractCommand! 6
request7 >
,> ?
CancellationToken@ Q
cancellationTokenR c
)c d
{ 	
var 
contract 
= 
await  
_contractRepository! 4
.4 5
FindByIdAsync5 B
(B C
requestC J
.J K
IdK M
,M N
cancellationTokenO `
)` a
;a b
if 
( 
contract 
is 
null  
)  !
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. G
{G H
requestH O
.O P
IdP R
}R S
$strS T
"T U
)U V
;V W
}   
_contractRepository"" 
.""  
Remove""  &
(""& '
contract""' /
)""/ 0
;""0 1
}## 	
}$$ 
}%% ‚

´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\DeleteContract\DeleteContractCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
	Contracts		= F
.		F G
DeleteContract		G U
{

 
public 

class !
DeleteContractCommand &
:' (
IRequest) 1
,1 2
ICommand3 ;
{ 
public !
DeleteContractCommand $
($ %
Guid% )
id* ,
), -
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ◊
¥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\CreateContract\CreateContractCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
CreateContractG U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 *
CreateContractCommandValidator

 /
:

0 1
AbstractValidator

2 C
<

C D!
CreateContractCommand

D Y
>

Y Z
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public *
CreateContractCommandValidator -
(- .
). /
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
} 	
} 
} ¿
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\CreateContract\CreateContractCommandHandler.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
CreateContractG U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class (
CreateContractCommandHandler -
:. /
IRequestHandler0 ?
<? @!
CreateContractCommand@ U
,U V
ContractDtoW b
>b c
{ 
private 
readonly 
IContractRepository ,
_contractRepository- @
;@ A
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public (
CreateContractCommandHandler +
(+ ,
IContractRepository, ?
contractRepository@ R
,R S
IMapperT [
mapper\ b
)b c
{ 	
_contractRepository 
=  !
contractRepository" 4
;4 5
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
ContractDto %
>% &
Handle' -
(- .!
CreateContractCommand. C
requestD K
,K L
CancellationTokenM ^
cancellationToken_ p
)p q
{ 	
var 
contract 
= 
new 
Contract '
{   
Name!! 
=!! 
request!! 
.!! 
Name!! #
,!!# $
IsActive"" 
="" 
request"" "
.""" #
IsActive""# +
}## 
;## 
_contractRepository%% 
.%%  
Add%%  #
(%%# $
contract%%$ ,
)%%, -
;%%- .
await&& 
_contractRepository&& %
.&&% &

UnitOfWork&&& 0
.&&0 1
SaveChangesAsync&&1 A
(&&A B
cancellationToken&&B S
)&&S T
;&&T U
return'' 
contract'' 
.'' 
MapToContractDto'' ,
('', -
_mapper''- 4
)''4 5
;''5 6
}(( 	
})) 
}** ¿
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\CreateContract\CreateContractCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
	Contracts= F
.F G
CreateContractG U
{		 
public

 

class

 !
CreateContractCommand

 &
:

' (
IRequest

) 1
<

1 2
ContractDto

2 =
>

= >
,

> ?
ICommand

@ H
{ 
public !
CreateContractCommand $
($ %
string% +
name, 0
,0 1
bool2 6
isActive7 ?
)? @
{ 	
Name 
= 
name 
; 
IsActive 
= 
isActive 
;  
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
} 
} ¯
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\ContractDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
	Contracts

= F
{ 
public 

static 
class (
ContractDtoMappingExtensions 4
{ 
public 
static 
ContractDto !
MapToContractDto" 2
(2 3
this3 7
Contract8 @
projectFromA L
,L M
IMapperN U
mapperV \
)\ ]
=> 
mapper 
. 
Map 
< 
ContractDto %
>% &
(& '
projectFrom' 2
)2 3
;3 4
public 
static 
List 
< 
ContractDto &
>& ' 
MapToContractDtoList( <
(< =
this= A
IEnumerableB M
<M N
ContractN V
>V W
projectFromX c
,c d
IMappere l
mapperm s
)s t
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToContractDto) 9
(9 :
mapper: @
)@ A
)A B
.B C
ToListC I
(I J
)J K
;K L
} 
} Ê
íD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Contracts\ContractDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
	Contracts

= F
{ 
public 

class 
ContractDto 
: 
IMapFrom '
<' (
Contract( 0
>0 1
{ 
public 
ContractDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
bool 
IsActive 
{ 
get "
;" #
set$ '
;' (
}) *
public 
static 
ContractDto !
Create" (
(( )
Guid) -
id. 0
,0 1
string2 8
name9 =
,= >
bool? C
isActiveD L
)L M
{ 	
return 
new 
ContractDto "
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
IsActive 
= 
isActive #
} 
; 
} 	
public!! 
void!! 
Mapping!! 
(!! 
Profile!! #
profile!!$ +
)!!+ ,
{"" 	
profile## 
.## 
	CreateMap## 
<## 
Contract## &
,##& '
ContractDto##( 3
>##3 4
(##4 5
)##5 6
;##6 7
}$$ 	
}%% 
}&& ó
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Validation\ValidatorProvider.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Common		= C
.		C D

Validation		D N
{

 
public 

class 
ValidatorProvider "
:# $
IValidatorProvider% 7
{ 
private 
readonly 
IServiceProvider )
_serviceProvider* :
;: ;
public 
ValidatorProvider  
(  !
IServiceProvider! 1
serviceProvider2 A
)A B
{ 	
_serviceProvider 
= 
serviceProvider .
;. /
} 	
public 

IValidator 
< 
T 
> 
GetValidator )
<) *
T* +
>+ ,
(, -
)- .
{ 	
return 
_serviceProvider #
.# $

GetService$ .
<. /

IValidator/ 9
<9 :
T: ;
>; <
>< =
(= >
)> ?
!? @
;@ A
} 	
} 
} ñ
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Validation\ValidationService.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

ValidationD N
{ 
public 

class 
ValidationService "
:# $
IValidationService% 7
{ 
private 
readonly 
IServiceProvider )
_serviceProvider* :
;: ;
public 
ValidationService  
(  !
IServiceProvider! 1
serviceProvider2 A
)A B
{ 	
_serviceProvider 
= 
serviceProvider .
;. /
} 	
public 
async 
Task 
Handle  
<  !
TRequest! )
>) *
(* +
TRequest+ 3
request4 ;
,; <
CancellationToken= N
cancellationTokenO `
=a b
defaultc j
)j k
{ 	
var 

validators 
= 
_serviceProvider -
.- .

GetService. 8
<8 9
IEnumerable9 D
<D E

IValidatorE O
<O P
TRequestP X
>X Y
>Y Z
>Z [
([ \
)\ ]
!] ^
;^ _
if 
( 

validators 
. 
Any 
( 
)  
)  !
{ 
var 
context 
= 
new !
ValidationContext" 3
<3 4
TRequest4 <
>< =
(= >
request> E
)E F
;F G
var 
validationResults %
=& '
await( -
Task. 2
.2 3
WhenAll3 :
(: ;

validators; E
.E F
SelectF L
(L M
vM N
=>O Q
vR S
.S T
ValidateAsyncT a
(a b
contextb i
,i j
cancellationTokenk |
)| }
)} ~
)~ 
;	 Ä
var   
failures   
=   
validationResults   0
.  0 1

SelectMany  1 ;
(  ; <
r  < =
=>  > @
r  A B
.  B C
Errors  C I
)  I J
.  J K
Where  K P
(  P Q
f  Q R
=>  S U
f  V W
!=  X Z
null  [ _
)  _ `
.  ` a
ToList  a g
(  g h
)  h i
;  i j
if"" 
("" 
failures"" 
."" 
Count"" "
!=""# %
$num""& '
)""' (
throw## 
new## 
ValidationException## 1
(##1 2
failures##2 :
)##: ;
;##; <
}$$ 
}%% 	
}&& 
}'' ⁄
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Validation\IValidatorProvider.cs
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
$str _
,_ `
Versiona h
=i j
$strk p
)p q
]q r
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

ValidationD N
{ 
public		 

	interface		 
IValidatorProvider		 '
{

 

IValidator 
< 
T 
> 
GetValidator "
<" #
T# $
>$ %
(% &
)& '
;' (
} 
} ƒ	
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Validation\IValidationService.cs
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
$str _
,_ `
Versiona h
=i j
$strk p
)p q
]q r
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

ValidationD N
{		 
public

 

	interface

 
IValidationService

 '
{ 
Task 
Handle 
< 
TRequest 
> 
( 
TRequest &
request' .
,. /
CancellationToken0 A
cancellationTokenB S
=T U
defaultV ]
)] ^
;^ _
} 
} ≈
ôD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\SimpleFileDownloadDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
{ 
public		 

class		 !
SimpleFileDownloadDto		 &
{

 
public !
SimpleFileDownloadDto $
($ %
)% &
{ 	
Content 
= 
null 
! 
; 
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static !
SimpleFileDownloadDto +
Create, 2
(2 3
Stream3 9
content: A
)A B
{ 	
return 
new !
SimpleFileDownloadDto ,
{ 
Content 
= 
content !
} 
; 
} 	
} 
} ±
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Security\AuthorizeAttribute.cs
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
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D
SecurityD L
{ 
[ 
AttributeUsage 
( 
AttributeTargets $
.$ %
Class% *
,* +
AllowMultiple, 9
=: ;
true< @
,@ A
	InheritedB K
=L M
trueN R
)R S
]S T
public 

class 
AuthorizeAttribute #
:$ %
	Attribute& /
{ 
public 
AuthorizeAttribute !
(! "
)" #
{ 	
Roles 
= 
null 
! 
; 
Policy 
= 
null 
! 
; 
} 	
public 
string 
Roles 
{ 
get !
;! "
set# &
;& '
}( )
public   
string   
Policy   
{   
get   "
;  " #
set  $ '
;  ' (
}  ) *
}!! 
}"" é
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Pagination\PagedResultMappingExtensions.cs
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
$str [
,[ \
Version] d
=e f
$strg l
)l m
]m n
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Common		= C
.		C D

Pagination		D N
{

 
public 

static 
class (
PagedResultMappingExtensions 4
{ 
public 
static 
PagedResult !
<! "
TDto" &
>& '
MapToPagedResult( 8
<8 9
TDomain9 @
,@ A
TDtoB F
>F G
(G H
this 

IPagedList 
< 
TDomain #
># $
	pagedList% .
,. /
Func 
< 
TDomain 
, 
TDto 
> 
mapFunc  '
)' (
{ 	
var 
data 
= 
	pagedList  
.  !
Select! '
(' (
mapFunc( /
)/ 0
.0 1
ToList1 7
(7 8
)8 9
;9 :
return!! 
PagedResult!! 
<!! 
TDto!! #
>!!# $
.!!$ %
Create!!% +
(!!+ ,

totalCount"" 
:"" 
	pagedList"" %
.""% &

TotalCount""& 0
,""0 1
	pageCount## 
:## 
	pagedList## $
.##$ %
	PageCount##% .
,##. /
pageSize$$ 
:$$ 
	pagedList$$ #
.$$# $
PageSize$$$ ,
,$$, -

pageNumber%% 
:%% 
	pagedList%% %
.%%% &
PageNo%%& ,
,%%, -
data&& 
:&& 
data&& 
)&& 
;&& 
}'' 	
public// 
static// 
PagedResult// !
<//! "
TDto//" &
>//& '
MapToPagedResult//( 8
<//8 9
TDto//9 =
>//= >
(//> ?
this//? C

IPagedList//D N
<//N O
TDto//O S
>//S T
	pagedList//U ^
)//^ _
{00 	
return11 
PagedResult11 
<11 
TDto11 #
>11# $
.11$ %
Create11% +
(11+ ,

totalCount22 
:22 
	pagedList22 %
.22% &

TotalCount22& 0
,220 1
	pageCount33 
:33 
	pagedList33 $
.33$ %
	PageCount33% .
,33. /
pageSize44 
:44 
	pagedList44 #
.44# $
PageSize44$ ,
,44, -

pageNumber55 
:55 
	pagedList55 %
.55% &
PageNo55& ,
,55, -
data66 
:66 
	pagedList66 
)66  
;66  !
}77 	
}88 
}:: ®
öD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Pagination\PagedResult.cs
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
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

PaginationD N
{ 
public		 

class		 
PagedResult		 
<		 
T		 
>		 
{

 
public 
PagedResult 
( 
) 
{ 	
Data 
= 
null 
! 
; 
} 	
public 
static 
PagedResult !
<! "
T" #
># $
Create% +
(+ ,
int, /

totalCount0 :
,: ;
int< ?
	pageCount@ I
,I J
intK N
pageSizeO W
,W X
intY \

pageNumber] g
,g h
IEnumerablei t
<t u
Tu v
>v w
datax |
)| }
{ 	
return 
new 
PagedResult "
<" #
T# $
>$ %
{ 

TotalCount 
= 

totalCount '
,' (
	PageCount 
= 
	pageCount %
,% &
PageSize 
= 
pageSize #
,# $

PageNumber 
= 

pageNumber '
,' (
Data 
= 
data 
, 
} 
; 
} 	
public 
int 

TotalCount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
	PageCount 
{ 
get "
;" #
set$ '
;' (
}) *
public   
int   
PageSize   
{   
get   !
;  ! "
set  # &
;  & '
}  ( )
public"" 
int"" 

PageNumber"" 
{"" 
get""  #
;""# $
set""% (
;""( )
}""* +
public$$ 
IEnumerable$$ 
<$$ 
T$$ 
>$$ 
Data$$ "
{$$# $
get$$% (
;$$( )
set$$* -
;$$- .
}$$/ 0
}&& 
}'' ®
òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Pagination\PagedList.cs
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
$str		 @
,		@ A
Version		B I
=		J K
$str		L Q
)		Q R
]		R S
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

PaginationD N
{ 
public 

class 
	PagedList 
< 
T 
> 
: 
List  $
<$ %
T% &
>& '
,' (

IPagedList) 3
<3 4
T4 5
>5 6
{ 
public 
	PagedList 
( 
int 

totalCount '
,' (
int) ,
pageNo- 3
,3 4
int5 8
pageSize9 A
,A B
IEnumerableC N
<N O
TO P
>P Q
resultsR Y
)Y Z
{ 	

TotalCount 
= 

totalCount #
;# $
	PageCount 
= 
GetPageCount $
($ %
pageSize% -
,- .

TotalCount/ 9
)9 :
;: ;
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
AddRange 
( 
results 
) 
; 
} 	
public 
int 

TotalCount 
{ 
get  #
;# $
private% ,
set- 0
;0 1
}2 3
public 
int 
	PageCount 
{ 
get "
;" #
private$ +
set, /
;/ 0
}1 2
public 
int 
PageNo 
{ 
get 
;  
private! (
set) ,
;, -
}. /
public 
int 
PageSize 
{ 
get !
;! "
private# *
set+ .
;. /
}0 1
private 
static 
int 
GetPageCount '
(' (
int( +
pageSize, 4
,4 5
int6 9

totalCount: D
)D E
{ 	
if 
( 
pageSize 
== 
$num 
) 
{   
return!! 
$num!! 
;!! 
}"" 
var## 
	remainder## 
=## 

totalCount## &
%##' (
pageSize##) 1
;##1 2
return$$ 
($$ 

totalCount$$ 
/$$  
pageSize$$! )
)$$) *
+$$+ ,
($$- .
	remainder$$. 7
==$$8 :
$num$$; <
?$$= >
$num$$? @
:$$A B
$num$$C D
)$$D E
;$$E F
}%% 	
}&& 
}'' û
ëD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Models\Result.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D
ModelsD J
{		 
public

 

class

 
Result

 
{ 
internal 
Result 
( 
bool 
	succeeded &
,& '
IEnumerable( 3
<3 4
string4 :
>: ;
errors< B
)B C
{ 	
	Succeeded 
= 
	succeeded !
;! "
Errors 
= 
errors 
. 
ToArray #
(# $
)$ %
;% &
} 	
public 
bool 
	Succeeded 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
[ 
] 
Errors 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
static 
Result 
Success $
($ %
)% &
{ 	
return 
new 
Result 
( 
true "
," #
new$ '
string( .
[. /
]/ 0
{1 2
}3 4
)4 5
;5 6
} 	
public 
static 
Result 
Failure $
($ %
IEnumerable% 0
<0 1
string1 7
>7 8
errors9 ?
)? @
{ 	
return 
new 
Result 
( 
false #
,# $
errors% +
)+ ,
;, -
} 	
} 
}   ”
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Models\DomainEventNotification.cs
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
$str O
,O P
VersionQ X
=Y Z
$str[ `
)` a
]a b
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D
ModelsD J
{		 
public

 

class

 #
DomainEventNotification

 (
<

( )
TDomainEvent

) 5
>

5 6
:

7 8
INotification

9 F
where

G L
TDomainEvent

M Y
:

Z [
DomainEvent

\ g
{ 
public #
DomainEventNotification &
(& '
TDomainEvent' 3
domainEvent4 ?
)? @
{ 	
DomainEvent 
= 
domainEvent %
;% &
} 	
public 
TDomainEvent 
DomainEvent '
{( )
get* -
;- .
}/ 0
} 
} ó
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Mappings\MappingProfile.cs
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
0 1
Application

1 <
.

< =
Common

= C
.

C D
Mappings

D L
{ 
public 

class 
MappingProfile 
:  !
Profile" )
{ 
public 
MappingProfile 
( 
) 
{ 	%
ApplyMappingsFromAssembly %
(% &
Assembly& .
.. / 
GetExecutingAssembly/ C
(C D
)D E
)E F
;F G
} 	
private 
void %
ApplyMappingsFromAssembly .
(. /
Assembly/ 7
assembly8 @
)@ A
{ 	
var 
types 
= 
assembly  
.  !
GetExportedTypes! 1
(1 2
)2 3
. 
Where 
( 
t 
=> 
Array !
.! "
Exists" (
(( )
t) *
.* +
GetInterfaces+ 8
(8 9
)9 :
,: ;
i< =
=>> @
i 
. 
IsGenericType #
&&$ &
i' (
.( )$
GetGenericTypeDefinition) A
(A B
)B C
==D F
typeofG M
(M N
IMapFromN V
<V W
>W X
)X Y
)Y Z
)Z [
. 
ToList 
( 
) 
; 
foreach 
( 
var 
type 
in  
types! &
)& '
{ 
var 
instance 
= 
	Activator (
.( )
CreateInstance) 7
(7 8
type8 <
,< =
true> B
)B C
;C D
var 

methodInfo 
=  
type! %
.% &
	GetMethod& /
(/ 0
$str0 9
)9 :
?? 
type 
. 
GetInterface (
(( )
$str) 5
)5 6
?6 7
.7 8
	GetMethod8 A
(A B
$strB K
)K L
;L M

methodInfo!! 
?!! 
.!! 
Invoke!! "
(!!" #
instance!!# +
,!!+ ,
[!!- .
this!!. 2
]!!2 3
)!!3 4
;!!4 5
}"" 
}## 	
}$$ 
}%% ∂
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Mappings\IMapFrom.cs
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
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D
MappingsD L
{ 
internal		 
	interface		 
IMapFrom		 
<		  
T		  !
>		! "
{

 
void 
Mapping 
( 
Profile 
profile $
)$ %
;% &
} 
} â
ïD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Interfaces\IQuery.cs
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
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

InterfacesD N
{ 
public 

	interface 
IQuery 
{		 
} 
} ö	
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Interfaces\IDomainEventService.cs
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
$str K
,K L
VersionM T
=U V
$strW \
)\ ]
]] ^
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Common		= C
.		C D

Interfaces		D N
{

 
public 

	interface 
IDomainEventService (
{ 
Task 
Publish 
( 
DomainEvent  
domainEvent! ,
,, -
CancellationToken. ?
cancellationToken@ Q
=R S
defaultT [
)[ \
;\ ]
} 
} Ú
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Interfaces\ICurrentUserService.cs
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
$str S
,S T
VersionU \
=] ^
$str_ d
)d e
]e f
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

InterfacesD N
{ 
public		 

	interface		 
ICurrentUserService		 (
{

 
string 
? 
UserId 
{ 
get 
; 
} 
string 
? 
UserName 
{ 
get 
; 
}  !
Task 
< 
bool 
> 
IsInRoleAsync  
(  !
string! '
role( ,
), -
;- .
Task 
< 
bool 
> 
AuthorizeAsync !
(! "
string" (
policy) /
)/ 0
;0 1
} 
} ç
óD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Interfaces\ICommand.cs
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
$str G
,G H
VersionI P
=Q R
$strS X
)X Y
]Y Z
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

InterfacesD N
{ 
public 

	interface 
ICommand 
{		 
} 
} ≠
ìD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\FileDownloadDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
{ 
public		 

class		 
FileDownloadDto		  
{

 
public 
FileDownloadDto 
( 
)  
{ 	
Content 
= 
null 
! 
; 
} 	
public 
Stream 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Filename 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
ContentType "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
static 
FileDownloadDto %
Create& ,
(, -
Stream- 3
content4 ;
,; <
string= C
?C D
filenameE M
,M N
stringO U
?U V
contentTypeW b
)b c
{ 	
return 
new 
FileDownloadDto &
{ 
Content 
= 
content !
,! "
Filename 
= 
filename #
,# $
ContentType 
= 
contentType )
} 
; 
} 	
} 
} ä
ßD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Exceptions\ProblemDetailsWithErrors.cs
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
$str S
,S T
VersionU \
=] ^
$str_ d
)d e
]e f
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

ExceptionsD N
{		 
public

 

class

 $
ProblemDetailsWithErrors

 )
{ 
public $
ProblemDetailsWithErrors '
(' (
)( )
{ 	
Type 
= 
null 
! 
; 
Title 
= 
null 
! 
; 
TraceId 
= 
null 
! 
; 
Errors 
= 
null 
! 
; 
ExtensionData 
= 
null  
!  !
;! "
} 	
[ 	
JsonPropertyName	 
( 
$str  
)  !
]! "
public 
string 
Type 
{ 
get  
;  !
set" %
;% &
}' (
[ 	
JsonPropertyName	 
( 
$str !
)! "
]" #
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
[ 	
JsonPropertyName	 
( 
$str "
)" #
]# $
public 
int 
Status 
{ 
get 
;  
set! $
;$ %
}& '
[ 	
JsonPropertyName	 
( 
$str #
)# $
]$ %
public 
string 
TraceId 
{ 
get  #
;# $
set% (
;( )
}* +
[ 	
JsonPropertyName	 
( 
$str "
)" #
]# $
public 

Dictionary 
< 
string  
,  !
string" (
[( )
]) *
>* +
Errors, 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
[ 	
JsonExtensionData	 
] 
public 

Dictionary 
< 
string  
,  !
object" (
>( )
ExtensionData* 7
{8 9
get: =
;= >
set? B
;B C
}D E
}   
}!! ÅY
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Exceptions\HttpClientRequestException.cs
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
$str U
,U V
VersionW ^
=_ `
$stra f
)f g
]g h
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

ExceptionsD N
{ 
public 

class &
HttpClientRequestException +
:, -
	Exception. 7
{ 
public &
HttpClientRequestException )
() *
)* +
{ 	

RequestUri 
= 
new 
Uri  
(  !
string! '
.' (
Empty( -
,- .
UriKind/ 6
.6 7
RelativeOrAbsolute7 I
)I J
;J K
ResponseHeaders 
= 
new !

Dictionary" ,
<, -
string- 3
,3 4
IEnumerable5 @
<@ A
stringA G
>G H
>H I
(I J
)J K
;K L
ResponseContent 
= 
string $
.$ %
Empty% *
;* +
} 	
public &
HttpClientRequestException )
() *
string* 0
message1 8
)8 9
:: ;
base< @
(@ A
messageA H
)H I
{ 	

RequestUri 
= 
new 
Uri  
(  !
string! '
.' (
Empty( -
,- .
UriKind/ 6
.6 7
RelativeOrAbsolute7 I
)I J
;J K
ResponseHeaders 
= 
new !

Dictionary" ,
<, -
string- 3
,3 4
IEnumerable5 @
<@ A
stringA G
>G H
>H I
(I J
)J K
;K L
ResponseContent 
= 
string $
.$ %
Empty% *
;* +
} 	
public   &
HttpClientRequestException   )
(  ) *
string  * 0
message  1 8
,  8 9
	Exception  : C
innerException  D R
)  R S
:  T U
base  V Z
(  Z [
message  [ b
,  b c
innerException  d r
)  r s
{!! 	

RequestUri"" 
="" 
new"" 
Uri""  
(""  !
string""! '
.""' (
Empty""( -
,""- .
UriKind""/ 6
.""6 7
RelativeOrAbsolute""7 I
)""I J
;""J K
ResponseHeaders## 
=## 
new## !

Dictionary##" ,
<##, -
string##- 3
,##3 4
IEnumerable##5 @
<##@ A
string##A G
>##G H
>##H I
(##I J
)##J K
;##K L
ResponseContent$$ 
=$$ 
string$$ $
.$$$ %
Empty$$% *
;$$* +
}%% 	
public&& &
HttpClientRequestException&& )
(&&) *
Uri&&* -

requestUri&&. 8
,&&8 9
HttpStatusCode'' 

statusCode'' %
,''% &
IReadOnlyDictionary(( 
<((  
string((  &
,((& '
IEnumerable((( 3
<((3 4
string((4 :
>((: ;
>((; <
responseHeaders((= L
,((L M
string)) 
?)) 
reasonPhrase))  
,))  !
string** 
responseContent** "
)**" #
:**$ %
base**& *
(*** +

GetMessage**+ 5
(**5 6

requestUri**6 @
,**@ A

statusCode**B L
,**L M
reasonPhrase**N Z
,**Z [
responseContent**\ k
)**k l
)**l m
{++ 	

RequestUri,, 
=,, 

requestUri,, #
;,,# $

StatusCode-- 
=-- 

statusCode-- #
;--# $
ResponseHeaders.. 
=.. 
responseHeaders.. -
;..- .
ReasonPhrase// 
=// 
reasonPhrase// '
;//' (
ResponseContent00 
=00 
responseContent00 -
;00- .
if22 
(22 
responseHeaders22 
?22  
.22  !
TryGetValue22! ,
(22, -
$str22- ;
,22; <
out22= @
var22A D
contentTypeValues22E V
)22V W
==22X Z
true22[ _
)22_ `
{33 
var44 
contentType44 
=44  !
contentTypeValues44" 3
?443 4
.444 5
FirstOrDefault445 C
(44C D
)44D E
;44E F
if66 
(66 
!66 
string66 
.66 
IsNullOrEmpty66 )
(66) *
contentType66* 5
)665 6
&&667 9
contentType66: E
.66E F

StartsWith66F P
(66P Q
$str66Q k
,66k l
StringComparison66m }
.66} ~
OrdinalIgnoreCase	66~ è
)
66è ê
)
66ê ë
{77 
ProblemDetails88 "
=88# $
JsonSerializer88% 3
.883 4
Deserialize884 ?
<88? @$
ProblemDetailsWithErrors88@ X
>88X Y
(88Y Z
responseContent88Z i
)88i j
;88j k
}99 
}:: 
};; 	
public== $
ProblemDetailsWithErrors== '
?==' (
ProblemDetails==) 7
{==8 9
get==: =
;=== >
private==? F
set==G J
;==J K
}==L M
public?? 
Uri?? 

RequestUri?? 
{?? 
get??  #
;??# $
private??% ,
set??- 0
;??0 1
}??2 3
public@@ 
HttpStatusCode@@ 

StatusCode@@ (
{@@) *
get@@+ .
;@@. /
private@@0 7
set@@8 ;
;@@; <
}@@= >
publicAA 
IReadOnlyDictionaryAA "
<AA" #
stringAA# )
,AA) *
IEnumerableAA+ 6
<AA6 7
stringAA7 =
>AA= >
>AA> ?
ResponseHeadersAA@ O
{AAP Q
getAAR U
;AAU V
privateAAW ^
setAA_ b
;AAb c
}AAd e
publicBB 
stringBB 
?BB 
ReasonPhraseBB #
{BB$ %
getBB& )
;BB) *
privateBB+ 2
setBB3 6
;BB6 7
}BB8 9
publicCC 
stringCC 
ResponseContentCC %
{CC& '
getCC( +
;CC+ ,
privateCC- 4
setCC5 8
;CC8 9
}CC: ;
publicEE 
staticEE 
asyncEE 
TaskEE  
<EE  !&
HttpClientRequestExceptionEE! ;
>EE; <
CreateEE= C
(EEC D
UriFF 
baseAddressFF 
,FF 
HttpRequestMessageGG 
requestGG &
,GG& '
HttpResponseMessageHH 
responseHH  (
,HH( )
CancellationTokenII 
cancellationTokenII /
)II/ 0
{JJ 	
varKK 
fullRequestUriKK 
=KK  
newKK! $
UriKK% (
(KK( )
baseAddressKK) 4
,KK4 5
requestKK6 =
.KK= >

RequestUriKK> H
!KKH I
)KKI J
;KKJ K
varLL 
contentLL 
=LL 
awaitLL 
responseLL  (
.LL( )
ContentLL) 0
.LL0 1
ReadAsStringAsyncLL1 B
(LLB C
cancellationTokenLLC T
)LLT U
.LLU V
ConfigureAwaitLLV d
(LLd e
falseLLe j
)LLj k
;LLk l
varNN 
headersNN 
=NN 
responseNN "
.NN" #
HeadersNN# *
.NN* +
ToDictionaryNN+ 7
(NN7 8
kNN8 9
=>NN: <
kNN= >
.NN> ?
KeyNN? B
,NNB C
vNND E
=>NNF H
vNNI J
.NNJ K
ValueNNK P
)NNP Q
;NNQ R
varOO 
contentHeadersOO 
=OO  
responseOO! )
.OO) *
ContentOO* 1
.OO1 2
HeadersOO2 9
.OO9 :
ToDictionaryOO: F
(OOF G
kOOG H
=>OOI K
kOOL M
.OOM N
KeyOON Q
,OOQ R
vOOS T
=>OOU W
vOOX Y
.OOY Z
ValueOOZ _
)OO_ `
;OO` a
varPP 

allHeadersPP 
=PP 
headersPP $
.QQ 
ConcatQQ 
(QQ 
contentHeadersQQ &
)QQ& '
.RR 
GroupByRR 
(RR 
kvpRR 
=>RR 
kvpRR  #
.RR# $
KeyRR$ '
)RR' (
.SS 
ToDictionarySS 
(SS 
groupSS #
=>SS$ &
groupSS' ,
.SS, -
KeySS- 0
,SS0 1
groupSS2 7
=>SS8 :
groupSS; @
.SS@ A
LastSSA E
(SSE F
)SSF G
.SSG H
ValueSSH M
)SSM N
;SSN O
returnUU 
newUU &
HttpClientRequestExceptionUU 1
(UU1 2
fullRequestUriUU2 @
,UU@ A
responseUUB J
.UUJ K

StatusCodeUUK U
,UUU V

allHeadersUUW a
,UUa b
responseUUc k
.UUk l
ReasonPhraseUUl x
,UUx y
content	UUz Å
)
UUÅ Ç
;
UUÇ É
}VV 	
privateXX 
staticXX 
stringXX 

GetMessageXX (
(XX( )
UriYY 

requestUriYY 
,YY 
HttpStatusCodeZZ 

statusCodeZZ %
,ZZ% &
string[[ 
?[[ 
reasonPhrase[[  
,[[  !
string\\ 
responseContent\\ "
)\\" #
{]] 	
var^^ 
message^^ 
=^^ 
$"^^ 
$str^^ '
{^^' (

requestUri^^( 2
}^^2 3
$str^^3 L
{^^L M
(^^M N
int^^N Q
)^^Q R

statusCode^^R \
}^^\ ]
$str^^] ^
{^^^ _
reasonPhrase^^_ k
}^^k l
$str^^l m
"^^m n
;^^n o
if__ 
(__ 
!__ 
string__ 
.__ 
IsNullOrWhiteSpace__ *
(__* +
responseContent__+ :
)__: ;
)__; <
{`` 
messageaa 
+=aa 
$straa :
;aa: ;
}bb 
returndd 
messagedd 
;dd 
}ee 	
}ff 
}gg ◊
ßD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Exceptions\ForbiddenAccessException.cs
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
.0 1
Application1 <
.< =
Common= C
.C D

ExceptionsD N
{ 
public		 

class		 $
ForbiddenAccessException		 )
:		* +
	Exception		, 5
{

 
public $
ForbiddenAccessException '
(' (
)( )
:* +
base, 0
(0 1
)1 2
{3 4
}5 6
public $
ForbiddenAccessException '
(' (
string( .
message/ 6
)6 7
: 
base 
( 
message 
) 
{ 	
} 	
public $
ForbiddenAccessException '
(' (
string( .
message/ 6
,6 7
	Exception8 A
innerExceptionB P
)P Q
: 
base 
( 
message 
, 
innerException *
)* +
{ 	
} 	
} 
} ¨

•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Eventing\IIntegrationEventHandler.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Common		= C
.		C D
Eventing		D L
{

 
public 

	interface $
IIntegrationEventHandler -
<- .
in. 0
TMessage1 9
>9 :
where 
TMessage 
: 
class 
{ 
Task 
HandleAsync 
( 
TMessage !
message" )
,) *
CancellationToken+ <
cancellationToken= N
=O P
defaultQ X
)X Y
;Y Z
} 
} ±
ñD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Eventing\IEventBus.cs
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
$str G
,G H
VersionI P
=Q R
$strS X
)X Y
]Y Z
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Common		= C
.		C D
Eventing		D L
{

 
public 

	interface 
	IEventBus 
{ 
void 
Publish 
< 
T 
> 
( 
T 
message !
)! "
where 
T 
: 
class 
; 
Task 
FlushAllAsync 
( 
CancellationToken ,
cancellationToken- >
=? @
defaultA H
)H I
;I J
void 
Send 
< 
T 
> 
( 
T 
message 
) 
where 
T 
: 
class 
; 
void 
Send 
< 
T 
> 
( 
T 
message 
, 
Uri  #
address$ +
)+ ,
where 
T 
: 
class 
; 
} 
} ◊
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\ValidationBehaviour.cs
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
 [
,

[ \
Version

] d
=

e f
$str

g l
)

l m
]

m n
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

BehavioursD N
{ 
public 

class 
ValidationBehaviour $
<$ %
TRequest% -
,- .
	TResponse/ 8
>8 9
:: ;
IPipelineBehavior< M
<M N
TRequestN V
,V W
	TResponseX a
>a b
where 
TRequest 
: 
notnull  
{ 
private 
readonly 
IEnumerable $
<$ %

IValidator% /
</ 0
TRequest0 8
>8 9
>9 :
_validators; F
;F G
public 
ValidationBehaviour "
(" #
IEnumerable# .
<. /

IValidator/ 9
<9 :
TRequest: B
>B C
>C D

validatorsE O
)O P
{ 	
_validators 
= 

validators $
;$ %
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest 
request 
, "
RequestHandlerDelegate "
<" #
	TResponse# ,
>, -
next. 2
,2 3
CancellationToken 
cancellationToken /
)/ 0
{ 	
if 
( 
_validators 
. 
Any 
(  
)  !
)! "
{ 
var 
context 
= 
new !
ValidationContext" 3
<3 4
TRequest4 <
>< =
(= >
request> E
)E F
;F G
var!! 
validationResults!! %
=!!& '
await!!( -
Task!!. 2
.!!2 3
WhenAll!!3 :
(!!: ;
_validators!!; F
.!!F G
Select!!G M
(!!M N
v!!N O
=>!!P R
v!!S T
.!!T U
ValidateAsync!!U b
(!!b c
context!!c j
,!!j k
cancellationToken!!l }
)!!} ~
)!!~ 
)	!! Ä
;
!!Ä Å
var"" 
failures"" 
="" 
validationResults"" 0
.""0 1

SelectMany""1 ;
(""; <
r""< =
=>""> @
r""A B
.""B C
Errors""C I
)""I J
.""J K
Where""K P
(""P Q
f""Q R
=>""S U
f""V W
!=""X Z
null""[ _
)""_ `
.""` a
ToList""a g
(""g h
)""h i
;""i j
if$$ 
($$ 
failures$$ 
.$$ 
Count$$ "
!=$$# %
$num$$& '
)$$' (
{%% 
throw&& 
new&& 
ValidationException&& 1
(&&1 2
failures&&2 :
)&&: ;
;&&; <
}'' 
}(( 
return)) 
await)) 
next)) 
()) 
))) 
;))  
}** 	
}++ 
},, ‚
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\UnitOfWorkBehaviour.cs
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
$str U
,U V
VersionW ^
=_ `
$stra f
)f g
]g h
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

BehavioursD N
{ 
public 

class 
UnitOfWorkBehaviour $
<$ %
TRequest% -
,- .
	TResponse/ 8
>8 9
:: ;
IPipelineBehavior< M
<M N
TRequestN V
,V W
	TResponseX a
>a b
where 
TRequest 
: 
notnull  
,  !
ICommand" *
{ 
private 
readonly 
IUnitOfWork $
_dataSource% 0
;0 1
public 
UnitOfWorkBehaviour "
(" #
IUnitOfWork# .

dataSource/ 9
)9 :
{ 	
_dataSource 
= 

dataSource $
??% '
throw( -
new. 1!
ArgumentNullException2 G
(G H
nameofH N
(N O

dataSourceO Y
)Y Z
)Z [
;[ \
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest   
request   
,   "
RequestHandlerDelegate!! "
<!!" #
	TResponse!!# ,
>!!, -
next!!. 2
,!!2 3
CancellationToken"" 
cancellationToken"" /
)""/ 0
{## 	
using** 
(** 
var** 
transaction** "
=**# $
new**% (
TransactionScope**) 9
(**9 :"
TransactionScopeOption**: P
.**P Q
Required**Q Y
,**Y Z
new++ 
TransactionOptions++ &
{++' (
IsolationLevel++) 7
=++8 9
IsolationLevel++: H
.++H I
ReadCommitted++I V
}++W X
,++X Y+
TransactionScopeAsyncFlowOption++Z y
.++y z
Enabled	++z Å
)
++Å Ç
)
++Ç É
{,, 
var-- 
response-- 
=-- 
await-- $
next--% )
(--) *
)--* +
;--+ ,
await22 
_dataSource22 !
.22! "
SaveChangesAsync22" 2
(222 3
cancellationToken223 D
)22D E
;22E F
transaction66 
.66 
Complete66 $
(66$ %
)66% &
;66& '
return88 
response88 
;88  
}99 
}:: 	
};; 
}<< ˛
™D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\UnhandledExceptionBehaviour.cs
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
 ]
,

] ^
Version

_ f
=

g h
$str

i n
)

n o
]

o p
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

BehavioursD N
{ 
public 

class '
UnhandledExceptionBehaviour ,
<, -
TRequest- 5
,5 6
	TResponse7 @
>@ A
:B C
IPipelineBehaviorD U
<U V
TRequestV ^
,^ _
	TResponse` i
>i j
where 
TRequest 
: 
notnull  
{ 
private 
readonly 
ILogger  
<  !'
UnhandledExceptionBehaviour! <
<< =
TRequest= E
,E F
	TResponseG P
>P Q
>Q R
_loggerS Z
;Z [
public '
UnhandledExceptionBehaviour *
(* +
ILogger+ 2
<2 3'
UnhandledExceptionBehaviour3 N
<N O
TRequestO W
,W X
	TResponseY b
>b c
>c d
loggere k
)k l
{ 	
_logger 
= 
logger 
; 
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest 
request 
, "
RequestHandlerDelegate "
<" #
	TResponse# ,
>, -
next. 2
,2 3
CancellationToken 
cancellationToken /
)/ 0
{ 	
try 
{ 
return 
await 
next !
(! "
)" #
;# $
}   
catch!! 
(!! 
ValidationException!! &
)!!& '
{"" 
throw$$ 
;$$ 
}%% 
catch&& 
(&& 
	Exception&& 
ex&& 
)&&  
{'' 
var(( 
requestName(( 
=((  !
typeof((" (
(((( )
TRequest(() 1
)((1 2
.((2 3
Name((3 7
;((7 8
_logger)) 
.)) 
LogError))  
())  !
ex))! #
,))# $
$str	))% à
,
))à â
requestName
))ä ï
,
))ï ñ
request
))ó û
)
))û ü
;
))ü †
throw** 
;** 
}++ 
},, 	
}-- 
}.. Ø!
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\PerformanceBehaviour.cs
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
 V
,

V W
Version

X _
=

` a
$str

b g
)

g h
]

h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

BehavioursD N
{ 
public 

class  
PerformanceBehaviour %
<% &
TRequest& .
,. /
	TResponse0 9
>9 :
:; <
IPipelineBehavior= N
<N O
TRequestO W
,W X
	TResponseY b
>b c
where 
TRequest 
: 
notnull  
{ 
private 
readonly 
	Stopwatch "
_timer# )
;) *
private 
readonly 
ILogger  
<  ! 
PerformanceBehaviour! 5
<5 6
TRequest6 >
,> ?
	TResponse@ I
>I J
>J K
_loggerL S
;S T
private 
readonly 
ICurrentUserService ,
_currentUserService- @
;@ A
public  
PerformanceBehaviour #
(# $
ILogger$ +
<+ , 
PerformanceBehaviour, @
<@ A
TRequestA I
,I J
	TResponseK T
>T U
>U V
loggerW ]
,] ^
ICurrentUserService 
currentUserService  2
)2 3
{ 	
_timer 
= 
new 
	Stopwatch "
(" #
)# $
;$ %
_logger 
= 
logger 
; 
_currentUserService 
=  !
currentUserService" 4
;4 5
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest 
request 
, "
RequestHandlerDelegate   "
<  " #
	TResponse  # ,
>  , -
next  . 2
,  2 3
CancellationToken!! 
cancellationToken!! /
)!!/ 0
{"" 	
_timer## 
.## 
Start## 
(## 
)## 
;## 
var%% 
response%% 
=%% 
await%%  
next%%! %
(%%% &
)%%& '
;%%' (
_timer'' 
.'' 
Stop'' 
('' 
)'' 
;'' 
var)) 
elapsedMilliseconds)) #
=))$ %
_timer))& ,
.)), -
ElapsedMilliseconds))- @
;))@ A
if++ 
(++ 
elapsedMilliseconds++ #
>++$ %
$num++& )
)++) *
{,, 
var-- 
requestName-- 
=--  !
typeof--" (
(--( )
TRequest--) 1
)--1 2
.--2 3
Name--3 7
;--7 8
var.. 
userId.. 
=.. 
_currentUserService.. 0
...0 1
UserId..1 7
;..7 8
var// 
userName// 
=// 
_currentUserService// 2
.//2 3
UserName//3 ;
;//; <
_logger11 
.11 

LogWarning11 "
(11" #
$str	11# Æ
,
11Æ Ø
requestName22 
,22  
elapsedMilliseconds22! 4
,224 5
userId226 <
,22< =
userName22> F
,22F G
request22H O
)22O P
;22P Q
}33 
return55 
response55 
;55 
}66 	
}77 
}88 
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\LoggingBehaviour.cs
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
$str		 R
,		R S
Version		T [
=		\ ]
$str		^ c
)		c d
]		d e
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Common= C
.C D

BehavioursD N
{ 
public 

class 
LoggingBehaviour !
<! "
TRequest" *
>* +
:, - 
IRequestPreProcessor. B
<B C
TRequestC K
>K L
where 
TRequest 
: 
notnull  
{ 
private 
readonly 
ILogger  
<  !
LoggingBehaviour! 1
<1 2
TRequest2 :
>: ;
>; <
_logger= D
;D E
private 
readonly 
ICurrentUserService ,
_currentUserService- @
;@ A
public 
LoggingBehaviour 
(  
ILogger  '
<' (
LoggingBehaviour( 8
<8 9
TRequest9 A
>A B
>B C
loggerD J
,J K
ICurrentUserServiceL _
currentUserService` r
)r s
{ 	
_logger 
= 
logger 
; 
_currentUserService 
=  !
currentUserService" 4
;4 5
} 	
public 
Task 
Process 
( 
TRequest $
request% ,
,, -
CancellationToken. ?
cancellationToken@ Q
)Q R
{ 	
var 
requestName 
= 
typeof $
($ %
TRequest% -
)- .
.. /
Name/ 3
;3 4
var 
userId 
= 
_currentUserService ,
., -
UserId- 3
;3 4
var 
userName 
= 
_currentUserService .
.. /
UserName/ 7
;7 8
_logger 
. 
LogInformation "
(" #
$str# |
,| }
requestName   
,   
userId   #
,  # $
userName  % -
,  - .
request  / 6
)  6 7
;  7 8
return!! 
Task!! 
.!! 
CompletedTask!! %
;!!% &
}"" 	
}## 
}$$ Ø
ßD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\EventBusPublishBehaviour.cs
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
$str Z
,Z [
Version\ c
=d e
$strf k
)k l
]l m
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
0 1
Application

1 <
.

< =
Common

= C
.

C D

Behaviours

D N
;

N O
public 
class $
EventBusPublishBehaviour %
<% &
TRequest& .
,. /
	TResponse0 9
>9 :
:; <
IPipelineBehavior= N
<N O
TRequestO W
,W X
	TResponseY b
>b c
where 
TRequest 
: 
notnull 
{ 
private 
readonly 
	IEventBus 
	_eventBus (
;( )
public 
$
EventBusPublishBehaviour #
(# $
	IEventBus$ -
eventBus. 6
)6 7
{ 
	_eventBus 
= 
eventBus 
; 
} 
public 

async 
Task 
< 
	TResponse 
>  
Handle! '
(' (
TRequest( 0
request1 8
,8 9"
RequestHandlerDelegate: P
<P Q
	TResponseQ Z
>Z [
next\ `
,` a
CancellationTokenb s
cancellationToken	t Ö
)
Ö Ü
{ 
var 
response 
= 
await 
next !
(! "
)" #
;# $
await 
	_eventBus 
. 
FlushAllAsync %
(% &
cancellationToken& 7
)7 8
;8 9
return 
response 
; 
} 
} à2
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Common\Behaviours\AuthorizationBehaviour.cs
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
.0 1
Application1 <
.< =
Common= C
.C D

BehavioursD N
{ 
public 

class "
AuthorizationBehaviour '
<' (
TRequest( 0
,0 1
	TResponse2 ;
>; <
:= >
IPipelineBehavior? P
<P Q
TRequestQ Y
,Y Z
	TResponse[ d
>d e
where 
TRequest 
: 
notnull  
{ 
private 
readonly 
ICurrentUserService ,
_currentUserService- @
;@ A
public "
AuthorizationBehaviour %
(% &
ICurrentUserService 
currentUserService  2
)2 3
{ 	
_currentUserService 
=  !
currentUserService" 4
;4 5
} 	
public 
async 
Task 
< 
	TResponse #
># $
Handle% +
(+ ,
TRequest, 4
request5 <
,< ="
RequestHandlerDelegate> T
<T U
	TResponseU ^
>^ _
next` d
,d e
CancellationTokenf w
cancellationToken	x â
)
â ä
{ 	
var 
authorizeAttributes #
=$ %
request& -
.- .
GetType. 5
(5 6
)6 7
.7 8
GetCustomAttributes8 K
<K L
AuthorizeAttributeL ^
>^ _
(_ `
)` a
.a b
ToListb h
(h i
)i j
;j k
if   
(   
authorizeAttributes   #
.  # $
Any  $ '
(  ' (
)  ( )
)  ) *
{!! 
if## 
(## 
_currentUserService## '
.##' (
UserId##( .
==##/ 1
null##2 6
)##6 7
{$$ 
throw%% 
new%% '
UnauthorizedAccessException%% 9
(%%9 :
)%%: ;
;%%; <
}&& 
var)) (
authorizeAttributesWithRoles)) 0
=))1 2
authorizeAttributes))3 F
.))F G
Where))G L
())L M
a))M N
=>))O Q
!))R S
string))S Y
.))Y Z
IsNullOrWhiteSpace))Z l
())l m
a))m n
.))n o
Roles))o t
)))t u
)))u v
.))v w
ToList))w }
())} ~
)))~ 
;	)) Ä
if++ 
(++ (
authorizeAttributesWithRoles++ 0
.++0 1
Any++1 4
(++4 5
)++5 6
)++6 7
{,, 
foreach-- 
(-- 
var--  
roles--! &
in--' )(
authorizeAttributesWithRoles--* F
.--F G
Select--G M
(--M N
a--N O
=>--P R
a--S T
.--T U
Roles--U Z
.--Z [
Split--[ `
(--` a
$char--a d
)--d e
)--e f
)--f g
{.. 
var// 

authorized// &
=//' (
false//) .
;//. /
foreach00 
(00  !
var00! $
role00% )
in00* ,
roles00- 2
)002 3
{11 
var22 
isInRole22  (
=22) *
await22+ 0
_currentUserService221 D
.22D E
IsInRoleAsync22E R
(22R S
role22S W
.22W X
Trim22X \
(22\ ]
)22] ^
)22^ _
;22_ `
if33 
(33  
isInRole33  (
)33( )
{44 

authorized55  *
=55+ ,
true55- 1
;551 2
break66  %
;66% &
}77 
}88 
if;; 
(;; 
!;; 

authorized;; '
);;' (
{<< 
throw== !
new==" %$
ForbiddenAccessException==& >
(==> ?
)==? @
;==@ A
}>> 
}?? 
}@@ 
varCC +
authorizeAttributesWithPoliciesCC 3
=CC4 5
authorizeAttributesCC6 I
.CCI J
WhereCCJ O
(CCO P
aCCP Q
=>CCR T
!CCU V
stringCCV \
.CC\ ]
IsNullOrWhiteSpaceCC] o
(CCo p
aCCp q
.CCq r
PolicyCCr x
)CCx y
)CCy z
.CCz {
ToList	CC{ Å
(
CCÅ Ç
)
CCÇ É
;
CCÉ Ñ
ifDD 
(DD +
authorizeAttributesWithPoliciesDD 3
.DD3 4
AnyDD4 7
(DD7 8
)DD8 9
)DD9 :
{EE 
foreachFF 
(FF 
varFF  
policyFF! '
inFF( *+
authorizeAttributesWithPoliciesFF+ J
.FFJ K
SelectFFK Q
(FFQ R
aFFR S
=>FFT V
aFFW X
.FFX Y
PolicyFFY _
)FF_ `
)FF` a
{GG 
varHH 

authorizedHH &
=HH' (
awaitHH) .
_currentUserServiceHH/ B
.HHB C
AuthorizeAsyncHHC Q
(HHQ R
policyHHR X
)HHX Y
;HHY Z
ifJJ 
(JJ 
!JJ 

authorizedJJ '
)JJ' (
{KK 
throwLL !
newLL" %$
ForbiddenAccessExceptionLL& >
(LL> ?
)LL? @
;LL@ A
}MM 
}NN 
}OO 
}PP 
returnSS 
awaitSS 
nextSS 
(SS 
)SS 
;SS  
}TT 	
}UU 
}VV ª
∏D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ClassicDomainServiceTests\ClassicDomainServiceTestUpdateDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =%
ClassicDomainServiceTests= V
{ 
public		 

class		 -
!ClassicDomainServiceTestUpdateDto		 2
{

 
public -
!ClassicDomainServiceTestUpdateDto 0
(0 1
)1 2
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
static -
!ClassicDomainServiceTestUpdateDto 7
Create8 >
(> ?
Guid? C
idD F
)F G
{ 	
return 
new -
!ClassicDomainServiceTestUpdateDto 8
{ 
Id 
= 
id 
} 
; 
} 	
} 
}  
√D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ClassicDomainServiceTests\ClassicDomainServiceTestDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =%
ClassicDomainServiceTests

= V
{ 
public 

static 
class 8
,ClassicDomainServiceTestDtoMappingExtensions D
{ 
public 
static '
ClassicDomainServiceTestDto 1,
 MapToClassicDomainServiceTestDto2 R
(R S
thisS W$
ClassicDomainServiceTestX p
projectFromq |
,| }
IMapper	~ Ö
mapper
Ü å
)
å ç
=> 
mapper 
. 
Map 
< '
ClassicDomainServiceTestDto 5
>5 6
(6 7
projectFrom7 B
)B C
;C D
public 
static 
List 
< '
ClassicDomainServiceTestDto 6
>6 70
$MapToClassicDomainServiceTestDtoList8 \
(\ ]
this] a
IEnumerableb m
<m n%
ClassicDomainServiceTest	n Ü
>
Ü á
projectFrom
à ì
,
ì î
IMapper
ï ú
mapper
ù £
)
£ §
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( ),
 MapToClassicDomainServiceTestDto) I
(I J
mapperJ P
)P Q
)Q R
.R S
ToListS Y
(Y Z
)Z [
;[ \
} 
} ã
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ClassicDomainServiceTests\ClassicDomainServiceTestDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =%
ClassicDomainServiceTests

= V
{ 
public 

class '
ClassicDomainServiceTestDto ,
:- .
IMapFrom/ 7
<7 8$
ClassicDomainServiceTest8 P
>P Q
{ 
public '
ClassicDomainServiceTestDto *
(* +
)+ ,
{ 	
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
static '
ClassicDomainServiceTestDto 1
Create2 8
(8 9
Guid9 =
id> @
)@ A
{ 	
return 
new '
ClassicDomainServiceTestDto 2
{ 
Id 
= 
id 
} 
; 
} 	
public 
void 
Mapping 
( 
Profile #
profile$ +
)+ ,
{ 	
profile 
. 
	CreateMap 
< $
ClassicDomainServiceTest 6
,6 7'
ClassicDomainServiceTestDto8 S
>S T
(T U
)U V
;V W
} 	
}   
}!! ÷

∏D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\ClassicDomainServiceTests\ClassicDomainServiceTestCreateDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =%
ClassicDomainServiceTests= V
{ 
public 

class -
!ClassicDomainServiceTestCreateDto 2
{		 
public

 -
!ClassicDomainServiceTestCreateDto

 0
(

0 1
)

1 2
{ 	
} 	
public 
static -
!ClassicDomainServiceTestCreateDto 7
Create8 >
(> ?
)? @
{ 	
return 
new -
!ClassicDomainServiceTestCreateDto 8
{ 
} 
; 
} 	
} 
} ï
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\UpdateBasic\UpdateBasicCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
UpdateBasicD O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
UpdateBasicCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
UpdateBasicCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
UpdateBasicCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
}  
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\UpdateBasic\UpdateBasicCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
UpdateBasicD O
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
class %
UpdateBasicCommandHandler *
:+ ,
IRequestHandler- <
<< =
UpdateBasicCommand= O
>O P
{ 
private 
readonly 
IBasicRepository )
_basicRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
UpdateBasicCommandHandler (
(( )
IBasicRepository) 9
basicRepository: I
)I J
{ 	
_basicRepository 
= 
basicRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
UpdateBasicCommand! 3
request4 ;
,; <
CancellationToken= N
cancellationTokenO `
)` a
{ 	
var 
basic 
= 
await 
_basicRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
if 
( 
basic 
is 
null 
) 
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. D
{D E
requestE L
.L M
IdM O
}O P
$strP Q
"Q R
)R S
;S T
}   
basic"" 
."" 
Name"" 
="" 
request""  
.""  !
Name""! %
;""% &
basic## 
.## 
Surname## 
=## 
request## #
.### $
Surname##$ +
;##+ ,
}$$ 	
}%% 
}&& ˜
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\UpdateBasic\UpdateBasicCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Basics		= C
.		C D
UpdateBasic		D O
{

 
public 

class 
UpdateBasicCommand #
:$ %
IRequest& .
,. /
ICommand0 8
{ 
public 
UpdateBasicCommand !
(! "
string" (
name) -
,- .
string/ 5
surname6 =
,= >
Guid? C
idD F
)F G
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
Id 
= 
id 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Æ
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasics\GetBasicsQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
	GetBasicsD M
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 #
GetBasicsQueryValidator

 (
:

) *
AbstractValidator

+ <
<

< =
GetBasicsQuery

= K
>

K L
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public #
GetBasicsQueryValidator &
(& '
)' (
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
OrderBy "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} è
£D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasics\GetBasicsQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
	GetBasicsD M
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class !
GetBasicsQueryHandler &
:' (
IRequestHandler) 8
<8 9
GetBasicsQuery9 G
,G H
CommonI O
.O P

PaginationP Z
.Z [
PagedResult[ f
<f g
BasicDtog o
>o p
>p q
{ 
private 
readonly 
IBasicRepository )
_basicRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public !
GetBasicsQueryHandler $
($ %
IBasicRepository% 5
basicRepository6 E
,E F
IMapperG N
mapperO U
)U V
{ 	
_basicRepository 
= 
basicRepository .
;. /
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Common  
.  !

Pagination! +
.+ ,
PagedResult, 7
<7 8
BasicDto8 @
>@ A
>A B
HandleC I
(I J
GetBasicsQuery 
request "
," #
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" 
basics"" 
="" 
await"" 
_basicRepository"" /
.""/ 0
FindAllAsync""0 <
(""< =
request""= D
.""D E
PageNo""E K
,""K L
request""M T
.""T U
PageSize""U ]
,""] ^
queryOptions""_ k
=>""l n
queryOptions""o {
.""{ |
OrderBy	""| É
(
""É Ñ
request
""Ñ ã
.
""ã å
OrderBy
""å ì
)
""ì î
,
""î ï
cancellationToken
""ñ ß
)
""ß ®
;
""® ©
return## 
basics## 
.## 
MapToPagedResult## *
(##* +
x##+ ,
=>##- /
x##0 1
.##1 2
MapToBasicDto##2 ?
(##? @
_mapper##@ G
)##G H
)##H I
;##I J
}$$ 	
}%% 
}&& Ë
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasics\GetBasicsQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Basics		= C
.		C D
	GetBasics		D M
{

 
public 

class 
GetBasicsQuery 
:  !
IRequest" *
<* +
PagedResult+ 6
<6 7
BasicDto7 ?
>? @
>@ A
,A B
IQueryC I
{ 
public 
GetBasicsQuery 
( 
int !
pageNo" (
,( )
int* -
pageSize. 6
,6 7
string8 >
orderBy? F
)F G
{ 	
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
OrderBy 
= 
orderBy 
; 
} 	
public 
int 
PageNo 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
OrderBy 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} à
µD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasicsNullable\GetBasicsNullableQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
GetBasicsNullableD U
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 +
GetBasicsNullableQueryValidator

 0
:

1 2
AbstractValidator

3 D
<

D E"
GetBasicsNullableQuery

E [
>

[ \
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public +
GetBasicsNullableQueryValidator .
(. /
)/ 0
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} 
≥D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasicsNullable\GetBasicsNullableQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
GetBasicsNullableD U
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class )
GetBasicsNullableQueryHandler .
:/ 0
IRequestHandler1 @
<@ A"
GetBasicsNullableQueryA W
,W X
CommonY _
._ `

Pagination` j
.j k
PagedResultk v
<v w
BasicDtow 
>	 Ä
>
Ä Å
{ 
private 
readonly 
IBasicRepository )
_basicRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public )
GetBasicsNullableQueryHandler ,
(, -
IBasicRepository- =
basicRepository> M
,M N
IMapperO V
mapperW ]
)] ^
{ 	
_basicRepository 
= 
basicRepository .
;. /
_mapper 
= 
mapper 
; 
} 	
[ 	
IntentManaged	 
( 
Mode 
. 
Fully !
,! "
Body# '
=( )
Mode* .
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Common  
.  !

Pagination! +
.+ ,
PagedResult, 7
<7 8
BasicDto8 @
>@ A
>A B
HandleC I
(I J"
GetBasicsNullableQuery "
request# *
,* +
CancellationToken   
cancellationToken   /
)  / 0
{!! 	
var"" 
basics"" 
="" 
await"" 
_basicRepository"" /
.""/ 0
FindAllAsync""0 <
(""< =
request""= D
.""D E
PageNo""E K
,""K L
request""M T
.""T U
PageSize""U ]
,""] ^
queryOptions""_ k
=>""l n
queryOptions""o {
.""{ |
OrderBy	""| É
(
""É Ñ
request
""Ñ ã
.
""ã å
OrderBy
""å ì
??
""î ñ
$str
""ó õ
)
""õ ú
,
""ú ù
cancellationToken
""û Ø
)
""Ø ∞
;
""∞ ±
return## 
basics## 
.## 
MapToPagedResult## *
(##* +
x##+ ,
=>##- /
x##0 1
.##1 2
MapToBasicDto##2 ?
(##? @
_mapper##@ G
)##G H
)##H I
;##I J
}$$ 	
}%% 
}&& Æ
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasicsNullable\GetBasicsNullableQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Basics		= C
.		C D
GetBasicsNullable		D U
{

 
public 

class "
GetBasicsNullableQuery '
:( )
IRequest* 2
<2 3
PagedResult3 >
<> ?
BasicDto? G
>G H
>H I
,I J
IQueryK Q
{ 
public "
GetBasicsNullableQuery %
(% &
int& )
pageNo* 0
,0 1
int2 5
pageSize6 >
,> ?
string@ F
?F G
orderByH O
)O P
{ 	
PageNo 
= 
pageNo 
; 
PageSize 
= 
pageSize 
;  
OrderBy 
= 
orderBy 
; 
} 	
public 
int 
PageNo 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
OrderBy 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} Í
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasicById\GetBasicByIdQueryValidator.cs
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
$str V
,V W
VersionX _
=` a
$strb g
)g h
]h i
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
GetBasicByIdD P
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 &
GetBasicByIdQueryValidator

 +
:

, -
AbstractValidator

. ?
<

? @
GetBasicByIdQuery

@ Q
>

Q R
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public &
GetBasicByIdQueryValidator )
() *
)* +
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} Ä
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasicById\GetBasicByIdQueryHandler.cs
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
$str C
,C D
VersionE L
=M N
$strO T
)T U
]U V
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
GetBasicByIdD P
{ 
[ 
IntentManaged 
( 
Mode 
. 
Merge 
, 
	Signature (
=) *
Mode+ /
./ 0
Fully0 5
)5 6
]6 7
public 

class $
GetBasicByIdQueryHandler )
:* +
IRequestHandler, ;
<; <
GetBasicByIdQuery< M
,M N
BasicDtoO W
>W X
{ 
private 
readonly 
IBasicRepository )
_basicRepository* :
;: ;
private 
readonly 
IMapper  
_mapper! (
;( )
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public $
GetBasicByIdQueryHandler '
(' (
IBasicRepository( 8
basicRepository9 H
,H I
IMapperJ Q
mapperR X
)X Y
{ 	
_basicRepository 
= 
basicRepository .
;. /
_mapper 
= 
mapper 
; 
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
BasicDto "
>" #
Handle$ *
(* +
GetBasicByIdQuery+ <
request= D
,D E
CancellationTokenF W
cancellationTokenX i
)i j
{ 	
var 
basic 
= 
await 
_basicRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
if   
(   
basic   
is   
null   
)   
{!! 
throw"" 
new"" 
NotFoundException"" +
(""+ ,
$""", .
$str"". D
{""D E
request""E L
.""L M
Id""M O
}""O P
$str""P Q
"""Q R
)""R S
;""S T
}## 
return$$ 
basic$$ 
.$$ 
MapToBasicDto$$ &
($$& '
_mapper$$' .
)$$. /
;$$/ 0
}%% 	
}&& 
}'' ˛

¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\GetBasicById\GetBasicByIdQuery.cs
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
$str B
,B C
VersionD K
=L M
$strN S
)S T
]T U
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Basics		= C
.		C D
GetBasicById		D P
{

 
public 

class 
GetBasicByIdQuery "
:# $
IRequest% -
<- .
BasicDto. 6
>6 7
,7 8
IQuery9 ?
{ 
public 
GetBasicByIdQuery  
(  !
Guid! %
id& (
)( )
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} Ï
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\DeleteBasic\DeleteBasicCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
DeleteBasicD O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
DeleteBasicCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
DeleteBasicCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
DeleteBasicCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
} 	
} 
} µ
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\DeleteBasic\DeleteBasicCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
DeleteBasicD O
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
class %
DeleteBasicCommandHandler *
:+ ,
IRequestHandler- <
<< =
DeleteBasicCommand= O
>O P
{ 
private 
readonly 
IBasicRepository )
_basicRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
DeleteBasicCommandHandler (
(( )
IBasicRepository) 9
basicRepository: I
)I J
{ 	
_basicRepository 
= 
basicRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
Handle  
(  !
DeleteBasicCommand! 3
request4 ;
,; <
CancellationToken= N
cancellationTokenO `
)` a
{ 	
var 
basic 
= 
await 
_basicRepository .
.. /
FindByIdAsync/ <
(< =
request= D
.D E
IdE G
,G H
cancellationTokenI Z
)Z [
;[ \
if 
( 
basic 
is 
null 
) 
{ 
throw 
new 
NotFoundException +
(+ ,
$", .
$str. D
{D E
requestE L
.L M
IdM O
}O P
$strP Q
"Q R
)R S
;S T
}   
_basicRepository"" 
."" 
Remove"" #
(""# $
basic""$ )
)"") *
;""* +
}## 	
}$$ 
}%% Õ

¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\DeleteBasic\DeleteBasicCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Basics		= C
.		C D
DeleteBasic		D O
{

 
public 

class 
DeleteBasicCommand #
:$ %
IRequest& .
,. /
ICommand0 8
{ 
public 
DeleteBasicCommand !
(! "
Guid" &
id' )
)) *
{ 	
Id 
= 
id 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
} 
} ï
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\CreateBasic\CreateBasicCommandValidator.cs
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
$str X
,X Y
VersionZ a
=b c
$strd i
)i j
]j k
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
CreateBasicD O
{ 
[		 
IntentManaged		 
(		 
Mode		 
.		 
Fully		 
,		 
Body		 #
=		$ %
Mode		& *
.		* +
Merge		+ 0
)		0 1
]		1 2
public

 

class

 '
CreateBasicCommandValidator

 ,
:

- .
AbstractValidator

/ @
<

@ A
CreateBasicCommand

A S
>

S T
{ 
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public '
CreateBasicCommandValidator *
(* +
)+ ,
{ 	$
ConfigureValidationRules $
($ %
)% &
;& '
} 	
private 
void $
ConfigureValidationRules -
(- .
). /
{ 	
RuleFor 
( 
v 
=> 
v 
. 
Name 
)  
. 
NotNull 
( 
) 
; 
RuleFor 
( 
v 
=> 
v 
. 
Surname "
)" #
. 
NotNull 
( 
) 
; 
} 	
} 
} Õ
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\CreateBasic\CreateBasicCommandHandler.cs
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
 E
,

E F
Version

G N
=

O P
$str

Q V
)

V W
]

W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Application1 <
.< =
Basics= C
.C D
CreateBasicD O
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
class %
CreateBasicCommandHandler *
:+ ,
IRequestHandler- <
<< =
CreateBasicCommand= O
,O P
GuidQ U
>U V
{ 
private 
readonly 
IBasicRepository )
_basicRepository* :
;: ;
[ 	
IntentManaged	 
( 
Mode 
. 
Merge !
)! "
]" #
public %
CreateBasicCommandHandler (
(( )
IBasicRepository) 9
basicRepository: I
)I J
{ 	
_basicRepository 
= 
basicRepository .
;. /
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
.. /
Fully/ 4
)4 5
]5 6
public 
async 
Task 
< 
Guid 
> 
Handle  &
(& '
CreateBasicCommand' 9
request: A
,A B
CancellationTokenC T
cancellationTokenU f
)f g
{ 	
var 
basic 
= 
new 
Basic !
{ 
Name 
= 
request 
. 
Name #
,# $
Surname 
= 
request !
.! "
Surname" )
}   
;   
_basicRepository"" 
."" 
Add""  
(""  !
basic""! &
)""& '
;""' (
await## 
_basicRepository## "
.##" #

UnitOfWork### -
.##- .
SaveChangesAsync##. >
(##> ?
cancellationToken##? P
)##P Q
;##Q R
return$$ 
basic$$ 
.$$ 
Id$$ 
;$$ 
}%% 	
}&& 
}'' §
¢D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\CreateBasic\CreateBasicCommand.cs
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
$str D
,D E
VersionF M
=N O
$strP U
)U V
]V W
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Application		1 <
.		< =
Basics		= C
.		C D
CreateBasic		D O
{

 
public 

class 
CreateBasicCommand #
:$ %
IRequest& .
<. /
Guid/ 3
>3 4
,4 5
ICommand6 >
{ 
public 
CreateBasicCommand !
(! "
string" (
name) -
,- .
string/ 5
surname6 =
)= >
{ 	
Name 
= 
name 
; 
Surname 
= 
surname 
; 
} 	
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
} 
} ‘
ùD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\BasicDtoMappingExtensions.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
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
0 1
Application

1 <
.

< =
Basics

= C
{ 
public 

static 
class %
BasicDtoMappingExtensions 1
{ 
public 
static 
BasicDto 
MapToBasicDto ,
(, -
this- 1
Basic2 7
projectFrom8 C
,C D
IMapperE L
mapperM S
)S T
=> 
mapper 
. 
Map 
< 
BasicDto "
>" #
(# $
projectFrom$ /
)/ 0
;0 1
public 
static 
List 
< 
BasicDto #
># $
MapToBasicDtoList% 6
(6 7
this7 ;
IEnumerable< G
<G H
BasicH M
>M N
projectFromO Z
,Z [
IMapper\ c
mapperd j
)j k
=> 
projectFrom 
. 
Select !
(! "
x" #
=>$ &
x' (
.( )
MapToBasicDto) 6
(6 7
mapper7 =
)= >
)> ?
.? @
ToList@ F
(F G
)G H
;H I
} 
} ú
åD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Application\Basics\BasicDto.cs
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
$str <
,< =
Version> E
=F G
$strH M
)M N
]N O
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
0 1
Application

1 <
.

< =
Basics

= C
{ 
public 

class 
BasicDto 
: 
IMapFrom $
<$ %
Basic% *
>* +
{ 
public 
BasicDto 
( 
) 
{ 	
Name 
= 
null 
! 
; 
Surname 
= 
null 
! 
; 
} 	
public 
Guid 
Id 
{ 
get 
; 
set !
;! "
}# $
public 
string 
Name 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Surname 
{ 
get  #
;# $
set% (
;( )
}* +
public 
static 
BasicDto 
Create %
(% &
Guid& *
id+ -
,- .
string/ 5
name6 :
,: ;
string< B
surnameC J
)J K
{ 	
return 
new 
BasicDto 
{ 
Id 
= 
id 
, 
Name 
= 
name 
, 
Surname 
= 
surname !
} 
; 
}   	
public"" 
void"" 
Mapping"" 
("" 
Profile"" #
profile""$ +
)""+ ,
{## 	
profile$$ 
.$$ 
	CreateMap$$ 
<$$ 
Basic$$ #
,$$# $
BasicDto$$% -
>$$- .
($$. /
)$$/ 0
;$$0 1
}%% 	
}&& 
}'' 