Ï
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Services\DomainEventService.cs
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
$str J
,J K
VersionL S
=T U
$strV [
)[ \
]\ ]
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Services@ H
{ 
public 

class 
DomainEventService #
:$ %
IDomainEventService& 9
{ 
private 
readonly 
ILogger  
<  !
DomainEventService! 3
>3 4
_logger5 <
;< =
private 
readonly 

IPublisher #
	_mediator$ -
;- .
public 
DomainEventService !
(! "
ILogger" )
<) *
DomainEventService* <
>< =
logger> D
,D E

IPublisherF P
mediatorQ Y
)Y Z
{ 	
_logger 
= 
logger 
; 
	_mediator 
= 
mediator  
;  !
} 	
public 
async 
Task 
Publish !
(! "
DomainEvent" -
domainEvent. 9
,9 :
CancellationToken; L
cancellationTokenM ^
=_ `
defaulta h
)h i
{ 	
_logger 
. 
LogInformation "
(" #
$str# M
,M N
domainEventO Z
.Z [
GetType[ b
(b c
)c d
.d e
Namee i
)i j
;j k
await 
	_mediator 
. 
Publish #
(# $5
)GetNotificationCorrespondingToDomainEvent$ M
(M N
domainEventN Y
)Y Z
,Z [
cancellationToken\ m
)m n
;n o
} 	
private!! 
static!! 
INotification!! $5
)GetNotificationCorrespondingToDomainEvent!!% N
(!!N O
DomainEvent!!O Z
domainEvent!![ f
)!!f g
{"" 	
var## 
result## 
=## 
	Activator## "
.##" #
CreateInstance### 1
(##1 2
typeof$$ 
($$ #
DomainEventNotification$$ .
<$$. /
>$$/ 0
)$$0 1
.$$1 2
MakeGenericType$$2 A
($$A B
domainEvent$$B M
.$$M N
GetType$$N U
($$U V
)$$V W
)$$W X
,$$X Y
domainEvent$$Z e
)$$e f
;$$f g
return&& 
result&& 
==&& 
null&& !
?'' 
throw'' 
new'' 
	Exception'' %
(''% &
$"''& (
$str''( Q
{''Q R
domainEvent''R ]
.''] ^
GetType''^ e
(''e f
)''f g
.''g h
Name''h l
}''l m
$str''m n
"''n o
)''o p
:(( 
((( 
INotification((  
)((  !
result((! '
;((' (
})) 	
}** 
}++ ˜
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\WarehouseRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
WarehouseRepository $
:% &
RepositoryBase' 5
<5 6
	Warehouse6 ?
,? @
	WarehouseA J
,J K 
ApplicationDbContextL `
>` a
,a b 
IWarehouseRepositoryc w
{ 
public 
WarehouseRepository "
(" # 
ApplicationDbContext# 7
	dbContext8 A
,A B
IMapperC J
mapperK Q
)Q R
:S T
baseU Y
(Y Z
	dbContextZ c
,c d
mappere k
)k l
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
	Warehouse #
?# $
>$ %
FindByIdAsync& 3
(3 4
Guid4 8
id9 ;
,; <
CancellationToken= N
cancellationTokenO `
=a b
defaultc j
)j k
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
	Warehouse$$ (
>$$( )
>$$) *
FindByIdsAsync$$+ 9
($$9 :
Guid$$: >
[$$> ?
]$$? @
ids$$A D
,$$D E
CancellationToken$$F W
cancellationToken$$X i
=$$j k
default$$l s
)$$s t
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) œ
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\UserRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
UserRepository 
:  !
RepositoryBase" 0
<0 1
User1 5
,5 6
User7 ;
,; < 
ApplicationDbContext= Q
>Q R
,R S
IUserRepositoryT c
{ 
public 
UserRepository 
(  
ApplicationDbContext 2
	dbContext3 <
,< =
IMapper> E
mapperF L
)L M
:N O
baseP T
(T U
	dbContextU ^
,^ _
mapper` f
)f g
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
User 
? 
>  
FindByIdAsync! .
(. /
Guid/ 3
id4 6
,6 7
CancellationToken8 I
cancellationTokenJ [
=\ ]
default^ e
)e f
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
User$$ #
>$$# $
>$$$ %
FindByIdsAsync$$& 4
($$4 5
Guid$$5 9
[$$9 :
]$$: ;
ids$$< ?
,$$? @
CancellationToken$$A R
cancellationToken$$S d
=$$e f
default$$g n
)$$n o
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) óﬁ
õD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\RepositoryBase.cs
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
$str R
,R S
VersionT [
=\ ]
$str^ c
)c d
]d e
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
{ 
public 

class 
RepositoryBase 
<  
TDomain  '
,' (
TPersistence) 5
,5 6

TDbContext7 A
>A B
:C D
IEFRepositoryE R
<R S
TDomainS Z
,Z [
TPersistence\ h
>h i
where 

TDbContext 
: 
	DbContext $
,$ %
IUnitOfWork& 1
where 
TPersistence 
: 
class "
," #
TDomain$ +
where 
TDomain 
: 
class 
{ 
	protected 
readonly 

TDbContext %

_dbContext& 0
;0 1
private 
readonly 
IMapper  
_mapper! (
;( )
public 
RepositoryBase 
( 

TDbContext (
	dbContext) 2
,2 3
IMapper4 ;
mapper< B
)B C
{ 	

_dbContext 
= 
	dbContext "
??# %
throw& +
new, /!
ArgumentNullException0 E
(E F
nameofF L
(L M
	dbContextM V
)V W
)W X
;X Y
_mapper   
=   
mapper   
;   
}!! 	
public## 
IUnitOfWork## 

UnitOfWork## %
=>##& (

_dbContext##) 3
;##3 4
public%% 
virtual%% 
void%% 
Remove%% "
(%%" #
TDomain%%# *
entity%%+ 1
)%%1 2
{&& 	
GetSet'' 
('' 
)'' 
.'' 
Remove'' 
('' 
('' 
TPersistence'' )
)'') *
entity''* 0
)''0 1
;''1 2
}(( 	
public** 
virtual** 
void** 
Add** 
(**  
TDomain**  '
entity**( .
)**. /
{++ 	
GetSet,, 
(,, 
),, 
.,, 
Add,, 
(,, 
(,, 
TPersistence,, &
),,& '
entity,,' -
),,- .
;,,. /
}-- 	
public// 
virtual// 
void// 
Update// "
(//" #
TDomain//# *
entity//+ 1
)//1 2
{00 	
GetSet11 
(11 
)11 
.11 
Update11 
(11 
(11 
TPersistence11 )
)11) *
entity11* 0
)110 1
;111 2
}22 	
public44 
virtual44 
async44 
Task44 !
<44! "
TDomain44" )
?44) *
>44* +
	FindAsync44, 5
(445 6

Expression55 
<55 
Func55 
<55 
TPersistence55 (
,55( )
bool55* .
>55. /
>55/ 0
filterExpression551 A
,55A B
CancellationToken66 
cancellationToken66 /
=660 1
default662 9
)669 :
{77 	
return88 
await88 
QueryInternal88 &
(88& '
filterExpression88' 7
)887 8
.888 9 
SingleOrDefaultAsync889 M
<88M N
TDomain88N U
>88U V
(88V W
cancellationToken88W h
)88h i
;88i j
}99 	
public;; 
virtual;; 
async;; 
Task;; !
<;;! "
TDomain;;" )
?;;) *
>;;* +
	FindAsync;;, 5
(;;5 6

Expression<< 
<<< 
Func<< 
<<< 
TPersistence<< (
,<<( )
bool<<* .
><<. /
><</ 0
filterExpression<<1 A
,<<A B
Func== 
<== 

IQueryable== 
<== 
TPersistence== (
>==( )
,==) *

IQueryable==+ 5
<==5 6
TPersistence==6 B
>==B C
>==C D
queryOptions==E Q
,==Q R
CancellationToken>> 
cancellationToken>> /
=>>0 1
default>>2 9
)>>9 :
{?? 	
return@@ 
await@@ 
QueryInternal@@ &
(@@& '
filterExpression@@' 7
,@@7 8
queryOptions@@9 E
)@@E F
.@@F G 
SingleOrDefaultAsync@@G [
<@@[ \
TDomain@@\ c
>@@c d
(@@d e
cancellationToken@@e v
)@@v w
;@@w x
}AA 	
publicCC 
virtualCC 
asyncCC 
TaskCC !
<CC! "
TDomainCC" )
?CC) *
>CC* +
	FindAsyncCC, 5
(CC5 6
FuncDD 
<DD 

IQueryableDD 
<DD 
TPersistenceDD (
>DD( )
,DD) *

IQueryableDD+ 5
<DD5 6
TPersistenceDD6 B
>DDB C
>DDC D
queryOptionsDDE Q
,DDQ R
CancellationTokenEE 
cancellationTokenEE /
=EE0 1
defaultEE2 9
)EE9 :
{FF 	
returnGG 
awaitGG 
QueryInternalGG &
(GG& '
queryOptionsGG' 3
)GG3 4
.GG4 5 
SingleOrDefaultAsyncGG5 I
<GGI J
TDomainGGJ Q
>GGQ R
(GGR S
cancellationTokenGGS d
)GGd e
;GGe f
}HH 	
publicJJ 
virtualJJ 
asyncJJ 
TaskJJ !
<JJ! "
ListJJ" &
<JJ& '
TDomainJJ' .
>JJ. /
>JJ/ 0
FindAllAsyncJJ1 =
(JJ= >
CancellationTokenJJ> O
cancellationTokenJJP a
=JJb c
defaultJJd k
)JJk l
{KK 	
returnLL 
awaitLL 
QueryInternalLL &
(LL& '
filterExpressionLL' 7
:LL7 8
nullLL9 =
)LL= >
.LL> ?
ToListAsyncLL? J
<LLJ K
TDomainLLK R
>LLR S
(LLS T
cancellationTokenLLT e
)LLe f
;LLf g
}MM 	
publicOO 
virtualOO 
asyncOO 
TaskOO !
<OO! "
ListOO" &
<OO& '
TDomainOO' .
>OO. /
>OO/ 0
FindAllAsyncOO1 =
(OO= >

ExpressionPP 
<PP 
FuncPP 
<PP 
TPersistencePP (
,PP( )
boolPP* .
>PP. /
>PP/ 0
filterExpressionPP1 A
,PPA B
CancellationTokenQQ 
cancellationTokenQQ /
=QQ0 1
defaultQQ2 9
)QQ9 :
{RR 	
returnSS 
awaitSS 
QueryInternalSS &
(SS& '
filterExpressionSS' 7
)SS7 8
.SS8 9
ToListAsyncSS9 D
<SSD E
TDomainSSE L
>SSL M
(SSM N
cancellationTokenSSN _
)SS_ `
;SS` a
}TT 	
publicVV 
virtualVV 
asyncVV 
TaskVV !
<VV! "
ListVV" &
<VV& '
TDomainVV' .
>VV. /
>VV/ 0
FindAllAsyncVV1 =
(VV= >

ExpressionWW 
<WW 
FuncWW 
<WW 
TPersistenceWW (
,WW( )
boolWW* .
>WW. /
>WW/ 0
filterExpressionWW1 A
,WWA B
FuncXX 
<XX 

IQueryableXX 
<XX 
TPersistenceXX (
>XX( )
,XX) *

IQueryableXX+ 5
<XX5 6
TPersistenceXX6 B
>XXB C
>XXC D
queryOptionsXXE Q
,XXQ R
CancellationTokenYY 
cancellationTokenYY /
=YY0 1
defaultYY2 9
)YY9 :
{ZZ 	
return[[ 
await[[ 
QueryInternal[[ &
([[& '
filterExpression[[' 7
,[[7 8
queryOptions[[9 E
)[[E F
.[[F G
ToListAsync[[G R
<[[R S
TDomain[[S Z
>[[Z [
([[[ \
cancellationToken[[\ m
)[[m n
;[[n o
}\\ 	
public^^ 
virtual^^ 
async^^ 
Task^^ !
<^^! "

IPagedList^^" ,
<^^, -
TDomain^^- 4
>^^4 5
>^^5 6
FindAllAsync^^7 C
(^^C D
int__ 
pageNo__ 
,__ 
int`` 
pageSize`` 
,`` 
CancellationTokenaa 
cancellationTokenaa /
=aa0 1
defaultaa2 9
)aa9 :
{bb 	
varcc 
querycc 
=cc 
QueryInternalcc %
(cc% &
filterExpressioncc& 6
:cc6 7
nullcc8 <
)cc< =
;cc= >
returndd 
awaitdd 
ToPagedListAsyncdd )
<dd) *
TDomaindd* 1
>dd1 2
(dd2 3
queryee 
,ee 
pageNoff 
,ff 
pageSizegg 
,gg 
cancellationTokenhh !
)hh! "
;hh" #
}ii 	
publickk 
virtualkk 
asynckk 
Taskkk !
<kk! "

IPagedListkk" ,
<kk, -
TDomainkk- 4
>kk4 5
>kk5 6
FindAllAsynckk7 C
(kkC D

Expressionll 
<ll 
Funcll 
<ll 
TPersistencell (
,ll( )
boolll* .
>ll. /
>ll/ 0
filterExpressionll1 A
,llA B
intmm 
pageNomm 
,mm 
intnn 
pageSizenn 
,nn 
CancellationTokenoo 
cancellationTokenoo /
=oo0 1
defaultoo2 9
)oo9 :
{pp 	
varqq 
queryqq 
=qq 
QueryInternalqq %
(qq% &
filterExpressionqq& 6
)qq6 7
;qq7 8
returnrr 
awaitrr 
ToPagedListAsyncrr )
<rr) *
TDomainrr* 1
>rr1 2
(rr2 3
queryss 
,ss 
pageNott 
,tt 
pageSizeuu 
,uu 
cancellationTokenvv !
)vv! "
;vv" #
}ww 	
publicyy 
virtualyy 
asyncyy 
Taskyy !
<yy! "

IPagedListyy" ,
<yy, -
TDomainyy- 4
>yy4 5
>yy5 6
FindAllAsyncyy7 C
(yyC D

Expressionzz 
<zz 
Funczz 
<zz 
TPersistencezz (
,zz( )
boolzz* .
>zz. /
>zz/ 0
filterExpressionzz1 A
,zzA B
int{{ 
pageNo{{ 
,{{ 
int|| 
pageSize|| 
,|| 
Func}} 
<}} 

IQueryable}} 
<}} 
TPersistence}} (
>}}( )
,}}) *

IQueryable}}+ 5
<}}5 6
TPersistence}}6 B
>}}B C
>}}C D
queryOptions}}E Q
,}}Q R
CancellationToken~~ 
cancellationToken~~ /
=~~0 1
default~~2 9
)~~9 :
{ 	
var
ÄÄ 
query
ÄÄ 
=
ÄÄ 
QueryInternal
ÄÄ %
(
ÄÄ% &
filterExpression
ÄÄ& 6
,
ÄÄ6 7
queryOptions
ÄÄ8 D
)
ÄÄD E
;
ÄÄE F
return
ÅÅ 
await
ÅÅ 
ToPagedListAsync
ÅÅ )
<
ÅÅ) *
TDomain
ÅÅ* 1
>
ÅÅ1 2
(
ÅÅ2 3
query
ÇÇ 
,
ÇÇ 
pageNo
ÉÉ 
,
ÉÉ 
pageSize
ÑÑ 
,
ÑÑ 
cancellationToken
ÖÖ !
)
ÖÖ! "
;
ÖÖ" #
}
ÜÜ 	
public
àà 
virtual
àà 
async
àà 
Task
àà !
<
àà! "
List
àà" &
<
àà& '
TDomain
àà' .
>
àà. /
>
àà/ 0
FindAllAsync
àà1 =
(
àà= >
Func
ââ 
<
ââ 

IQueryable
ââ 
<
ââ 
TPersistence
ââ (
>
ââ( )
,
ââ) *

IQueryable
ââ+ 5
<
ââ5 6
TPersistence
ââ6 B
>
ââB C
>
ââC D
queryOptions
ââE Q
,
ââQ R
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
ãã 	
return
åå 
await
åå 
QueryInternal
åå &
(
åå& '
queryOptions
åå' 3
)
åå3 4
.
åå4 5
ToListAsync
åå5 @
<
åå@ A
TDomain
ååA H
>
ååH I
(
ååI J
cancellationToken
ååJ [
)
åå[ \
;
åå\ ]
}
çç 	
public
èè 
virtual
èè 
async
èè 
Task
èè !
<
èè! "

IPagedList
èè" ,
<
èè, -
TDomain
èè- 4
>
èè4 5
>
èè5 6
FindAllAsync
èè7 C
(
èèC D
int
êê 
pageNo
êê 
,
êê 
int
ëë 
pageSize
ëë 
,
ëë 
Func
íí 
<
íí 

IQueryable
íí 
<
íí 
TPersistence
íí (
>
íí( )
,
íí) *

IQueryable
íí+ 5
<
íí5 6
TPersistence
íí6 B
>
ííB C
>
ííC D
queryOptions
ííE Q
,
ííQ R
CancellationToken
ìì 
cancellationToken
ìì /
=
ìì0 1
default
ìì2 9
)
ìì9 :
{
îî 	
var
ïï 
query
ïï 
=
ïï 
QueryInternal
ïï %
(
ïï% &
queryOptions
ïï& 2
)
ïï2 3
;
ïï3 4
return
ññ 
await
ññ 
ToPagedListAsync
ññ )
<
ññ) *
TDomain
ññ* 1
>
ññ1 2
(
ññ2 3
query
óó 
,
óó 
pageNo
òò 
,
òò 
pageSize
ôô 
,
ôô 
cancellationToken
öö !
)
öö! "
;
öö" #
}
õõ 	
public
ùù 
virtual
ùù 
async
ùù 
Task
ùù !
<
ùù! "
int
ùù" %
>
ùù% &

CountAsync
ùù' 1
(
ùù1 2

Expression
ûû 
<
ûû 
Func
ûû 
<
ûû 
TPersistence
ûû (
,
ûû( )
bool
ûû* .
>
ûû. /
>
ûû/ 0
filterExpression
ûû1 A
,
ûûA B
CancellationToken
üü 
cancellationToken
üü /
=
üü0 1
default
üü2 9
)
üü9 :
{
†† 	
return
°° 
await
°° 
QueryInternal
°° &
(
°°& '
filterExpression
°°' 7
)
°°7 8
.
°°8 9

CountAsync
°°9 C
(
°°C D
cancellationToken
°°D U
)
°°U V
;
°°V W
}
¢¢ 	
public
§§ 
virtual
§§ 
async
§§ 
Task
§§ !
<
§§! "
int
§§" %
>
§§% &

CountAsync
§§' 1
(
§§1 2
Func
•• 
<
•• 

IQueryable
•• 
<
•• 
TPersistence
•• (
>
••( )
,
••) *

IQueryable
••+ 5
<
••5 6
TPersistence
••6 B
>
••B C
>
••C D
?
••D E
queryOptions
••F R
=
••S T
default
••U \
,
••\ ]
CancellationToken
¶¶ 
cancellationToken
¶¶ /
=
¶¶0 1
default
¶¶2 9
)
¶¶9 :
{
ßß 	
return
®® 
await
®® 
QueryInternal
®® &
(
®®& '
queryOptions
®®' 3
)
®®3 4
.
®®4 5

CountAsync
®®5 ?
(
®®? @
cancellationToken
®®@ Q
)
®®Q R
;
®®R S
}
©© 	
public
´´ 
virtual
´´ 
async
´´ 
Task
´´ !
<
´´! "
bool
´´" &
>
´´& '
AnyAsync
´´( 0
(
´´0 1

Expression
¨¨ 
<
¨¨ 
Func
¨¨ 
<
¨¨ 
TPersistence
¨¨ (
,
¨¨( )
bool
¨¨* .
>
¨¨. /
>
¨¨/ 0
filterExpression
¨¨1 A
,
¨¨A B
CancellationToken
≠≠ 
cancellationToken
≠≠ /
=
≠≠0 1
default
≠≠2 9
)
≠≠9 :
{
ÆÆ 	
return
ØØ 
await
ØØ 
QueryInternal
ØØ &
(
ØØ& '
filterExpression
ØØ' 7
)
ØØ7 8
.
ØØ8 9
AnyAsync
ØØ9 A
(
ØØA B
cancellationToken
ØØB S
)
ØØS T
;
ØØT U
}
∞∞ 	
public
≤≤ 
virtual
≤≤ 
async
≤≤ 
Task
≤≤ !
<
≤≤! "
bool
≤≤" &
>
≤≤& '
AnyAsync
≤≤( 0
(
≤≤0 1
Func
≥≥ 
<
≥≥ 

IQueryable
≥≥ 
<
≥≥ 
TPersistence
≥≥ (
>
≥≥( )
,
≥≥) *

IQueryable
≥≥+ 5
<
≥≥5 6
TPersistence
≥≥6 B
>
≥≥B C
>
≥≥C D
?
≥≥D E
queryOptions
≥≥F R
=
≥≥S T
default
≥≥U \
,
≥≥\ ]
CancellationToken
¥¥ 
cancellationToken
¥¥ /
=
¥¥0 1
default
¥¥2 9
)
¥¥9 :
{
µµ 	
return
∂∂ 
await
∂∂ 
QueryInternal
∂∂ &
(
∂∂& '
queryOptions
∂∂' 3
)
∂∂3 4
.
∂∂4 5
AnyAsync
∂∂5 =
(
∂∂= >
cancellationToken
∂∂> O
)
∂∂O P
;
∂∂P Q
}
∑∑ 	
public
ππ 
async
ππ 
Task
ππ 
<
ππ 
List
ππ 
<
ππ 
TProjection
ππ *
>
ππ* +
>
ππ+ ,#
FindAllProjectToAsync
ππ- B
<
ππB C
TProjection
ππC N
>
ππN O
(
ππO P
CancellationToken
ππP a
cancellationToken
ππb s
=
ππt u
default
ππv }
)
ππ} ~
{
∫∫ 	
var
ªª 
	queryable
ªª 
=
ªª 
QueryInternal
ªª )
(
ªª) *
filterExpression
ªª* :
:
ªª: ;
null
ªª< @
)
ªª@ A
;
ªªA B
var
ºº 

projection
ºº 
=
ºº 
	queryable
ºº &
.
ºº& '
	ProjectTo
ºº' 0
<
ºº0 1
TProjection
ºº1 <
>
ºº< =
(
ºº= >
_mapper
ºº> E
.
ººE F#
ConfigurationProvider
ººF [
)
ºº[ \
;
ºº\ ]
return
ΩΩ 
await
ΩΩ 

projection
ΩΩ #
.
ΩΩ# $
ToListAsync
ΩΩ$ /
(
ΩΩ/ 0
cancellationToken
ΩΩ0 A
)
ΩΩA B
;
ΩΩB C
}
ææ 	
public
¿¿ 
async
¿¿ 
Task
¿¿ 
<
¿¿ 
List
¿¿ 
<
¿¿ 
TProjection
¿¿ *
>
¿¿* +
>
¿¿+ ,#
FindAllProjectToAsync
¿¿- B
<
¿¿B C
TProjection
¿¿C N
>
¿¿N O
(
¿¿O P
Func
¡¡ 
<
¡¡ 

IQueryable
¡¡ 
<
¡¡ 
TPersistence
¡¡ (
>
¡¡( )
,
¡¡) *

IQueryable
¡¡+ 5
<
¡¡5 6
TPersistence
¡¡6 B
>
¡¡B C
>
¡¡C D
queryOptions
¡¡E Q
,
¡¡Q R
CancellationToken
¬¬ 
cancellationToken
¬¬ /
=
¬¬0 1
default
¬¬2 9
)
¬¬9 :
{
√√ 	
var
ƒƒ 
	queryable
ƒƒ 
=
ƒƒ 
QueryInternal
ƒƒ )
(
ƒƒ) *
queryOptions
ƒƒ* 6
)
ƒƒ6 7
;
ƒƒ7 8
var
≈≈ 

projection
≈≈ 
=
≈≈ 
	queryable
≈≈ &
.
≈≈& '
	ProjectTo
≈≈' 0
<
≈≈0 1
TProjection
≈≈1 <
>
≈≈< =
(
≈≈= >
_mapper
≈≈> E
.
≈≈E F#
ConfigurationProvider
≈≈F [
)
≈≈[ \
;
≈≈\ ]
return
∆∆ 
await
∆∆ 

projection
∆∆ #
.
∆∆# $
ToListAsync
∆∆$ /
(
∆∆/ 0
cancellationToken
∆∆0 A
)
∆∆A B
;
∆∆B C
}
«« 	
public
…… 
async
…… 
Task
…… 
<
…… 
List
…… 
<
…… 
TProjection
…… *
>
……* +
>
……+ ,#
FindAllProjectToAsync
……- B
<
……B C
TProjection
……C N
>
……N O
(
……O P

Expression
   
<
   
Func
   
<
   
TPersistence
   (
,
  ( )
bool
  * .
>
  . /
>
  / 0
filterExpression
  1 A
,
  A B
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
ÕÕ 
	queryable
ÕÕ 
=
ÕÕ 
QueryInternal
ÕÕ )
(
ÕÕ) *
filterExpression
ÕÕ* :
)
ÕÕ: ;
;
ÕÕ; <
var
ŒŒ 

projection
ŒŒ 
=
ŒŒ 
	queryable
ŒŒ &
.
ŒŒ& '
	ProjectTo
ŒŒ' 0
<
ŒŒ0 1
TProjection
ŒŒ1 <
>
ŒŒ< =
(
ŒŒ= >
_mapper
ŒŒ> E
.
ŒŒE F#
ConfigurationProvider
ŒŒF [
)
ŒŒ[ \
;
ŒŒ\ ]
return
œœ 
await
œœ 

projection
œœ #
.
œœ# $
ToListAsync
œœ$ /
(
œœ/ 0
cancellationToken
œœ0 A
)
œœA B
;
œœB C
}
–– 	
public
““ 
async
““ 
Task
““ 
<
““ 
List
““ 
<
““ 
TProjection
““ *
>
““* +
>
““+ ,#
FindAllProjectToAsync
““- B
<
““B C
TProjection
““C N
>
““N O
(
““O P

Expression
”” 
<
”” 
Func
”” 
<
”” 
TPersistence
”” (
,
””( )
bool
””* .
>
””. /
>
””/ 0
filterExpression
””1 A
,
””A B
Func
‘‘ 
<
‘‘ 

IQueryable
‘‘ 
<
‘‘ 
TPersistence
‘‘ (
>
‘‘( )
,
‘‘) *

IQueryable
‘‘+ 5
<
‘‘5 6
TPersistence
‘‘6 B
>
‘‘B C
>
‘‘C D
queryOptions
‘‘E Q
,
‘‘Q R
CancellationToken
’’ 
cancellationToken
’’ /
=
’’0 1
default
’’2 9
)
’’9 :
{
÷÷ 	
var
◊◊ 
	queryable
◊◊ 
=
◊◊ 
QueryInternal
◊◊ )
(
◊◊) *
filterExpression
◊◊* :
,
◊◊: ;
queryOptions
◊◊< H
)
◊◊H I
;
◊◊I J
var
ÿÿ 

projection
ÿÿ 
=
ÿÿ 
	queryable
ÿÿ &
.
ÿÿ& '
	ProjectTo
ÿÿ' 0
<
ÿÿ0 1
TProjection
ÿÿ1 <
>
ÿÿ< =
(
ÿÿ= >
_mapper
ÿÿ> E
.
ÿÿE F#
ConfigurationProvider
ÿÿF [
)
ÿÿ[ \
;
ÿÿ\ ]
return
ŸŸ 
await
ŸŸ 

projection
ŸŸ #
.
ŸŸ# $
ToListAsync
ŸŸ$ /
(
ŸŸ/ 0
cancellationToken
ŸŸ0 A
)
ŸŸA B
;
ŸŸB C
}
⁄⁄ 	
public
‹‹ 
async
‹‹ 
Task
‹‹ 
<
‹‹ 

IPagedList
‹‹ $
<
‹‹$ %
TProjection
‹‹% 0
>
‹‹0 1
>
‹‹1 2#
FindAllProjectToAsync
‹‹3 H
<
‹‹H I
TProjection
‹‹I T
>
‹‹T U
(
‹‹U V
int
›› 
pageNo
›› 
,
›› 
int
ﬁﬁ 
pageSize
ﬁﬁ 
,
ﬁﬁ 
CancellationToken
ﬂﬂ 
cancellationToken
ﬂﬂ /
=
ﬂﬂ0 1
default
ﬂﬂ2 9
)
ﬂﬂ9 :
{
‡‡ 	
var
·· 
	queryable
·· 
=
·· 
QueryInternal
·· )
(
··) *
filterExpression
··* :
:
··: ;
null
··< @
)
··@ A
;
··A B
var
‚‚ 

projection
‚‚ 
=
‚‚ 
	queryable
‚‚ &
.
‚‚& '
	ProjectTo
‚‚' 0
<
‚‚0 1
TProjection
‚‚1 <
>
‚‚< =
(
‚‚= >
_mapper
‚‚> E
.
‚‚E F#
ConfigurationProvider
‚‚F [
)
‚‚[ \
;
‚‚\ ]
return
„„ 
await
„„ 
ToPagedListAsync
„„ )
(
„„) *

projection
‰‰ 
,
‰‰ 
pageNo
ÂÂ 
,
ÂÂ 
pageSize
ÊÊ 
,
ÊÊ 
cancellationToken
ÁÁ !
)
ÁÁ! "
;
ÁÁ" #
}
ËË 	
public
ÍÍ 
async
ÍÍ 
Task
ÍÍ 
<
ÍÍ 

IPagedList
ÍÍ $
<
ÍÍ$ %
TProjection
ÍÍ% 0
>
ÍÍ0 1
>
ÍÍ1 2#
FindAllProjectToAsync
ÍÍ3 H
<
ÍÍH I
TProjection
ÍÍI T
>
ÍÍT U
(
ÍÍU V
int
ÎÎ 
pageNo
ÎÎ 
,
ÎÎ 
int
ÏÏ 
pageSize
ÏÏ 
,
ÏÏ 
Func
ÌÌ 
<
ÌÌ 

IQueryable
ÌÌ 
<
ÌÌ 
TPersistence
ÌÌ (
>
ÌÌ( )
,
ÌÌ) *

IQueryable
ÌÌ+ 5
<
ÌÌ5 6
TPersistence
ÌÌ6 B
>
ÌÌB C
>
ÌÌC D
queryOptions
ÌÌE Q
,
ÌÌQ R
CancellationToken
ÓÓ 
cancellationToken
ÓÓ /
=
ÓÓ0 1
default
ÓÓ2 9
)
ÓÓ9 :
{
ÔÔ 	
var
 
	queryable
 
=
 
QueryInternal
 )
(
) *
queryOptions
* 6
)
6 7
;
7 8
var
ÒÒ 

projection
ÒÒ 
=
ÒÒ 
	queryable
ÒÒ &
.
ÒÒ& '
	ProjectTo
ÒÒ' 0
<
ÒÒ0 1
TProjection
ÒÒ1 <
>
ÒÒ< =
(
ÒÒ= >
_mapper
ÒÒ> E
.
ÒÒE F#
ConfigurationProvider
ÒÒF [
)
ÒÒ[ \
;
ÒÒ\ ]
return
ÚÚ 
await
ÚÚ 
ToPagedListAsync
ÚÚ )
(
ÚÚ) *

projection
ÛÛ 
,
ÛÛ 
pageNo
ÙÙ 
,
ÙÙ 
pageSize
ıı 
,
ıı 
cancellationToken
ˆˆ !
)
ˆˆ! "
;
ˆˆ" #
}
˜˜ 	
public
˘˘ 
async
˘˘ 
Task
˘˘ 
<
˘˘ 

IPagedList
˘˘ $
<
˘˘$ %
TProjection
˘˘% 0
>
˘˘0 1
>
˘˘1 2#
FindAllProjectToAsync
˘˘3 H
<
˘˘H I
TProjection
˘˘I T
>
˘˘T U
(
˘˘U V

Expression
˙˙ 
<
˙˙ 
Func
˙˙ 
<
˙˙ 
TPersistence
˙˙ (
,
˙˙( )
bool
˙˙* .
>
˙˙. /
>
˙˙/ 0
filterExpression
˙˙1 A
,
˙˙A B
int
˚˚ 
pageNo
˚˚ 
,
˚˚ 
int
¸¸ 
pageSize
¸¸ 
,
¸¸ 
CancellationToken
˝˝ 
cancellationToken
˝˝ /
=
˝˝0 1
default
˝˝2 9
)
˝˝9 :
{
˛˛ 	
var
ˇˇ 
	queryable
ˇˇ 
=
ˇˇ 
QueryInternal
ˇˇ )
(
ˇˇ) *
filterExpression
ˇˇ* :
)
ˇˇ: ;
;
ˇˇ; <
var
ÄÄ 

projection
ÄÄ 
=
ÄÄ 
	queryable
ÄÄ &
.
ÄÄ& '
	ProjectTo
ÄÄ' 0
<
ÄÄ0 1
TProjection
ÄÄ1 <
>
ÄÄ< =
(
ÄÄ= >
_mapper
ÄÄ> E
.
ÄÄE F#
ConfigurationProvider
ÄÄF [
)
ÄÄ[ \
;
ÄÄ\ ]
return
ÅÅ 
await
ÅÅ 
ToPagedListAsync
ÅÅ )
(
ÅÅ) *

projection
ÇÇ 
,
ÇÇ 
pageNo
ÉÉ 
,
ÉÉ 
pageSize
ÑÑ 
,
ÑÑ 
cancellationToken
ÖÖ !
)
ÖÖ! "
;
ÖÖ" #
}
ÜÜ 	
public
àà 
async
àà 
Task
àà 
<
àà 

IPagedList
àà $
<
àà$ %
TProjection
àà% 0
>
àà0 1
>
àà1 2#
FindAllProjectToAsync
àà3 H
<
ààH I
TProjection
ààI T
>
ààT U
(
ààU V

Expression
ââ 
<
ââ 
Func
ââ 
<
ââ 
TPersistence
ââ (
,
ââ( )
bool
ââ* .
>
ââ. /
>
ââ/ 0
filterExpression
ââ1 A
,
ââA B
int
ää 
pageNo
ää 
,
ää 
int
ãã 
pageSize
ãã 
,
ãã 
Func
åå 
<
åå 

IQueryable
åå 
<
åå 
TPersistence
åå (
>
åå( )
,
åå) *

IQueryable
åå+ 5
<
åå5 6
TPersistence
åå6 B
>
ååB C
>
ååC D
queryOptions
ååE Q
,
ååQ R
CancellationToken
çç 
cancellationToken
çç /
=
çç0 1
default
çç2 9
)
çç9 :
{
éé 	
var
èè 
	queryable
èè 
=
èè 
QueryInternal
èè )
(
èè) *
filterExpression
èè* :
,
èè: ;
queryOptions
èè< H
)
èèH I
;
èèI J
var
êê 

projection
êê 
=
êê 
	queryable
êê &
.
êê& '
	ProjectTo
êê' 0
<
êê0 1
TProjection
êê1 <
>
êê< =
(
êê= >
_mapper
êê> E
.
êêE F#
ConfigurationProvider
êêF [
)
êê[ \
;
êê\ ]
return
ëë 
await
ëë 
ToPagedListAsync
ëë )
(
ëë) *

projection
íí 
,
íí 
pageNo
ìì 
,
ìì 
pageSize
îî 
,
îî 
cancellationToken
ïï !
)
ïï! "
;
ïï" #
}
ññ 	
public
òò 
async
òò 
Task
òò 
<
òò 
List
òò 
<
òò 
TProjection
òò *
>
òò* +
>
òò+ ,#
FindAllProjectToAsync
òò- B
<
òòB C
TProjection
òòC N
>
òòN O
(
òòO P

Expression
ôô 
<
ôô 
Func
ôô 
<
ôô 
TPersistence
ôô (
,
ôô( )
bool
ôô* .
>
ôô. /
>
ôô/ 0
?
ôô0 1
filterExpression
ôô2 B
,
ôôB C
Func
öö 
<
öö 

IQueryable
öö 
<
öö 
TProjection
öö '
>
öö' (
,
öö( )

IQueryable
öö* 4
>
öö4 5
filterProjection
öö6 F
,
ööF G
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
ùù 
	queryable
ùù 
=
ùù 
QueryInternal
ùù )
(
ùù) *
filterExpression
ùù* :
)
ùù: ;
;
ùù; <
var
ûû 

projection
ûû 
=
ûû 
	queryable
ûû &
.
ûû& '
	ProjectTo
ûû' 0
<
ûû0 1
TProjection
ûû1 <
>
ûû< =
(
ûû= >
_mapper
ûû> E
.
ûûE F#
ConfigurationProvider
ûûF [
)
ûû[ \
;
ûû\ ]
var
üü 
response
üü 
=
üü 
filterProjection
üü +
(
üü+ ,

projection
üü, 6
)
üü6 7
;
üü7 8
return
†† 
await
†† 
response
†† !
.
††! "
Cast
††" &
<
††& '
TProjection
††' 2
>
††2 3
(
††3 4
)
††4 5
.
††5 6
ToListAsync
††6 A
(
††A B
)
††B C
;
††C D
}
°° 	
public
££ 
async
££ 
Task
££ 
<
££ 
TProjection
££ %
?
££% &
>
££& ' 
FindProjectToAsync
££( :
<
££: ;
TProjection
££; F
>
££F G
(
££G H

Expression
§§ 
<
§§ 
Func
§§ 
<
§§ 
TPersistence
§§ (
,
§§( )
bool
§§* .
>
§§. /
>
§§/ 0
filterExpression
§§1 A
,
§§A B
CancellationToken
•• 
cancellationToken
•• /
=
••0 1
default
••2 9
)
••9 :
{
¶¶ 	
var
ßß 
	queryable
ßß 
=
ßß 
QueryInternal
ßß )
(
ßß) *
filterExpression
ßß* :
)
ßß: ;
;
ßß; <
var
®® 

projection
®® 
=
®® 
	queryable
®® &
.
®®& '
	ProjectTo
®®' 0
<
®®0 1
TProjection
®®1 <
>
®®< =
(
®®= >
_mapper
®®> E
.
®®E F#
ConfigurationProvider
®®F [
)
®®[ \
;
®®\ ]
return
©© 
await
©© 

projection
©© #
.
©©# $!
FirstOrDefaultAsync
©©$ 7
(
©©7 8
cancellationToken
©©8 I
)
©©I J
;
©©J K
}
™™ 	
public
¨¨ 
async
¨¨ 
Task
¨¨ 
<
¨¨ 
TProjection
¨¨ %
?
¨¨% &
>
¨¨& ' 
FindProjectToAsync
¨¨( :
<
¨¨: ;
TProjection
¨¨; F
>
¨¨F G
(
¨¨G H

Expression
≠≠ 
<
≠≠ 
Func
≠≠ 
<
≠≠ 
TPersistence
≠≠ (
,
≠≠( )
bool
≠≠* .
>
≠≠. /
>
≠≠/ 0
filterExpression
≠≠1 A
,
≠≠A B
Func
ÆÆ 
<
ÆÆ 

IQueryable
ÆÆ 
<
ÆÆ 
TPersistence
ÆÆ (
>
ÆÆ( )
,
ÆÆ) *

IQueryable
ÆÆ+ 5
<
ÆÆ5 6
TPersistence
ÆÆ6 B
>
ÆÆB C
>
ÆÆC D
queryOptions
ÆÆE Q
,
ÆÆQ R
CancellationToken
ØØ 
cancellationToken
ØØ /
=
ØØ0 1
default
ØØ2 9
)
ØØ9 :
{
∞∞ 	
var
±± 
	queryable
±± 
=
±± 
QueryInternal
±± )
(
±±) *
filterExpression
±±* :
,
±±: ;
queryOptions
±±< H
)
±±H I
;
±±I J
var
≤≤ 

projection
≤≤ 
=
≤≤ 
	queryable
≤≤ &
.
≤≤& '
	ProjectTo
≤≤' 0
<
≤≤0 1
TProjection
≤≤1 <
>
≤≤< =
(
≤≤= >
_mapper
≤≤> E
.
≤≤E F#
ConfigurationProvider
≤≤F [
)
≤≤[ \
;
≤≤\ ]
return
≥≥ 
await
≥≥ 

projection
≥≥ #
.
≥≥# $!
FirstOrDefaultAsync
≥≥$ 7
(
≥≥7 8
cancellationToken
≥≥8 I
)
≥≥I J
;
≥≥J K
}
¥¥ 	
public
∂∂ 
async
∂∂ 
Task
∂∂ 
<
∂∂ 
TProjection
∂∂ %
?
∂∂% &
>
∂∂& ' 
FindProjectToAsync
∂∂( :
<
∂∂: ;
TProjection
∂∂; F
>
∂∂F G
(
∂∂G H
Func
∑∑ 
<
∑∑ 

IQueryable
∑∑ 
<
∑∑ 
TPersistence
∑∑ (
>
∑∑( )
,
∑∑) *

IQueryable
∑∑+ 5
<
∑∑5 6
TPersistence
∑∑6 B
>
∑∑B C
>
∑∑C D
queryOptions
∑∑E Q
,
∑∑Q R
CancellationToken
∏∏ 
cancellationToken
∏∏ /
=
∏∏0 1
default
∏∏2 9
)
∏∏9 :
{
ππ 	
var
∫∫ 
	queryable
∫∫ 
=
∫∫ 
QueryInternal
∫∫ )
(
∫∫) *
queryOptions
∫∫* 6
)
∫∫6 7
;
∫∫7 8
var
ªª 

projection
ªª 
=
ªª 
	queryable
ªª &
.
ªª& '
	ProjectTo
ªª' 0
<
ªª0 1
TProjection
ªª1 <
>
ªª< =
(
ªª= >
_mapper
ªª> E
.
ªªE F#
ConfigurationProvider
ªªF [
)
ªª[ \
;
ªª\ ]
return
ºº 
await
ºº 

projection
ºº #
.
ºº# $!
FirstOrDefaultAsync
ºº$ 7
(
ºº7 8
cancellationToken
ºº8 I
)
ººI J
;
ººJ K
}
ΩΩ 	
public
øø 
async
øø 
Task
øø 
<
øø 
IEnumerable
øø %
>
øø% &5
'FindAllProjectToWithTransformationAsync
øø' N
<
øøN O
TProjection
øøO Z
>
øøZ [
(
øø[ \

Expression
¿¿ 
<
¿¿ 
Func
¿¿ 
<
¿¿ 
TPersistence
¿¿ (
,
¿¿( )
bool
¿¿* .
>
¿¿. /
>
¿¿/ 0
?
¿¿0 1
filterExpression
¿¿2 B
,
¿¿B C
Func
¡¡ 
<
¡¡ 

IQueryable
¡¡ 
<
¡¡ 
TProjection
¡¡ '
>
¡¡' (
,
¡¡( )

IQueryable
¡¡* 4
>
¡¡4 5
	transform
¡¡6 ?
,
¡¡? @
CancellationToken
¬¬ 
cancellationToken
¬¬ /
=
¬¬0 1
default
¬¬2 9
)
¬¬9 :
{
√√ 	
var
ƒƒ 
	queryable
ƒƒ 
=
ƒƒ 
QueryInternal
ƒƒ )
(
ƒƒ) *
filterExpression
ƒƒ* :
)
ƒƒ: ;
;
ƒƒ; <
var
≈≈ 

projection
≈≈ 
=
≈≈ 
	queryable
≈≈ &
.
≈≈& '
	ProjectTo
≈≈' 0
<
≈≈0 1
TProjection
≈≈1 <
>
≈≈< =
(
≈≈= >
_mapper
≈≈> E
.
≈≈E F#
ConfigurationProvider
≈≈F [
)
≈≈[ \
;
≈≈\ ]
var
∆∆ 
response
∆∆ 
=
∆∆ 
	transform
∆∆ $
(
∆∆$ %

projection
∆∆% /
)
∆∆/ 0
;
∆∆0 1
return
«« 
await
«« 
response
«« !
.
««! "
Cast
««" &
<
««& '
object
««' -
>
««- .
(
««. /
)
««/ 0
.
««0 1
ToListAsync
««1 <
(
««< =
)
««= >
;
««> ?
}
»» 	
	protected
   
virtual
   

IQueryable
   $
<
  $ %
TPersistence
  % 1
>
  1 2
QueryInternal
  3 @
(
  @ A

Expression
  A K
<
  K L
Func
  L P
<
  P Q
TPersistence
  Q ]
,
  ] ^
bool
  _ c
>
  c d
>
  d e
?
  e f
filterExpression
  g w
)
  w x
{
ÀÀ 	
var
ÃÃ 
	queryable
ÃÃ 
=
ÃÃ 
CreateQuery
ÃÃ '
(
ÃÃ' (
)
ÃÃ( )
;
ÃÃ) *
if
ÕÕ 
(
ÕÕ 
filterExpression
ÕÕ  
!=
ÕÕ! #
null
ÕÕ$ (
)
ÕÕ( )
{
ŒŒ 
	queryable
œœ 
=
œœ 
	queryable
œœ %
.
œœ% &
Where
œœ& +
(
œœ+ ,
filterExpression
œœ, <
)
œœ< =
;
œœ= >
}
–– 
return
—— 
	queryable
—— 
;
—— 
}
““ 	
	protected
‘‘ 
virtual
‘‘ 

IQueryable
‘‘ $
<
‘‘$ %
TResult
‘‘% ,
>
‘‘, -
QueryInternal
‘‘. ;
<
‘‘; <
TResult
‘‘< C
>
‘‘C D
(
‘‘D E

Expression
’’ 
<
’’ 
Func
’’ 
<
’’ 
TPersistence
’’ (
,
’’( )
bool
’’* .
>
’’. /
>
’’/ 0
filterExpression
’’1 A
,
’’A B
Func
÷÷ 
<
÷÷ 

IQueryable
÷÷ 
<
÷÷ 
TPersistence
÷÷ (
>
÷÷( )
,
÷÷) *

IQueryable
÷÷+ 5
<
÷÷5 6
TResult
÷÷6 =
>
÷÷= >
>
÷÷> ?
queryOptions
÷÷@ L
)
÷÷L M
{
◊◊ 	
var
ÿÿ 
	queryable
ÿÿ 
=
ÿÿ 
CreateQuery
ÿÿ '
(
ÿÿ' (
)
ÿÿ( )
;
ÿÿ) *
	queryable
ŸŸ 
=
ŸŸ 
	queryable
ŸŸ !
.
ŸŸ! "
Where
ŸŸ" '
(
ŸŸ' (
filterExpression
ŸŸ( 8
)
ŸŸ8 9
;
ŸŸ9 :
var
⁄⁄ 
result
⁄⁄ 
=
⁄⁄ 
queryOptions
⁄⁄ %
(
⁄⁄% &
	queryable
⁄⁄& /
)
⁄⁄/ 0
;
⁄⁄0 1
return
€€ 
result
€€ 
;
€€ 
}
‹‹ 	
	protected
ﬁﬁ 
virtual
ﬁﬁ 

IQueryable
ﬁﬁ $
<
ﬁﬁ$ %
TPersistence
ﬁﬁ% 1
>
ﬁﬁ1 2
QueryInternal
ﬁﬁ3 @
(
ﬁﬁ@ A
Func
ﬁﬁA E
<
ﬁﬁE F

IQueryable
ﬁﬁF P
<
ﬁﬁP Q
TPersistence
ﬁﬁQ ]
>
ﬁﬁ] ^
,
ﬁﬁ^ _

IQueryable
ﬁﬁ` j
<
ﬁﬁj k
TPersistence
ﬁﬁk w
>
ﬁﬁw x
>
ﬁﬁx y
?
ﬁﬁy z
queryOptionsﬁﬁ{ á
)ﬁﬁá à
{
ﬂﬂ 	
var
‡‡ 
	queryable
‡‡ 
=
‡‡ 
CreateQuery
‡‡ '
(
‡‡' (
)
‡‡( )
;
‡‡) *
if
·· 
(
·· 
queryOptions
·· 
!=
·· 
null
··  $
)
··$ %
{
‚‚ 
	queryable
„„ 
=
„„ 
queryOptions
„„ (
(
„„( )
	queryable
„„) 2
)
„„2 3
;
„„3 4
}
‰‰ 
return
ÂÂ 
	queryable
ÂÂ 
;
ÂÂ 
}
ÊÊ 	
	protected
ËË 
virtual
ËË 

IQueryable
ËË $
<
ËË$ %
TPersistence
ËË% 1
>
ËË1 2
CreateQuery
ËË3 >
(
ËË> ?
)
ËË? @
{
ÈÈ 	
return
ÍÍ 
GetSet
ÍÍ 
(
ÍÍ 
)
ÍÍ 
;
ÍÍ 
}
ÎÎ 	
	protected
ÌÌ 
virtual
ÌÌ 
DbSet
ÌÌ 
<
ÌÌ  
TPersistence
ÌÌ  ,
>
ÌÌ, -
GetSet
ÌÌ. 4
(
ÌÌ4 5
)
ÌÌ5 6
{
ÓÓ 	
return
ÔÔ 

_dbContext
ÔÔ 
.
ÔÔ 
Set
ÔÔ !
<
ÔÔ! "
TPersistence
ÔÔ" .
>
ÔÔ. /
(
ÔÔ/ 0
)
ÔÔ0 1
;
ÔÔ1 2
}
 	
private
ÚÚ 
static
ÚÚ 
async
ÚÚ 
Task
ÚÚ !
<
ÚÚ! "

IPagedList
ÚÚ" ,
<
ÚÚ, -
T
ÚÚ- .
>
ÚÚ. /
>
ÚÚ/ 0
ToPagedListAsync
ÚÚ1 A
<
ÚÚA B
T
ÚÚB C
>
ÚÚC D
(
ÚÚD E

IQueryable
ÛÛ 
<
ÛÛ 
T
ÛÛ 
>
ÛÛ 
	queryable
ÛÛ #
,
ÛÛ# $
int
ÙÙ 
pageNo
ÙÙ 
,
ÙÙ 
int
ıı 
pageSize
ıı 
,
ıı 
CancellationToken
ˆˆ 
cancellationToken
ˆˆ /
=
ˆˆ0 1
default
ˆˆ2 9
)
ˆˆ9 :
{
˜˜ 	
var
¯¯ 
count
¯¯ 
=
¯¯ 
await
¯¯ 
	queryable
¯¯ '
.
¯¯' (

CountAsync
¯¯( 2
(
¯¯2 3
cancellationToken
¯¯3 D
)
¯¯D E
;
¯¯E F
var
˘˘ 
skip
˘˘ 
=
˘˘ 
(
˘˘ 
(
˘˘ 
pageNo
˘˘ 
-
˘˘  !
$num
˘˘" #
)
˘˘# $
*
˘˘% &
pageSize
˘˘' /
)
˘˘/ 0
;
˘˘0 1
var
˚˚ 
results
˚˚ 
=
˚˚ 
await
˚˚ 
	queryable
˚˚  )
.
¸¸ 
Skip
¸¸ 
(
¸¸ 
skip
¸¸ 
)
¸¸ 
.
˝˝ 
Take
˝˝ 
(
˝˝ 
pageSize
˝˝ 
)
˝˝ 
.
˛˛ 
ToListAsync
˛˛ 
(
˛˛ 
cancellationToken
˛˛ .
)
˛˛. /
;
˛˛/ 0
return
ˇˇ 
new
ˇˇ 
	PagedList
ˇˇ  
<
ˇˇ  !
T
ˇˇ! "
>
ˇˇ" #
(
ˇˇ# $
count
ˇˇ$ )
,
ˇˇ) *
pageNo
ˇˇ+ 1
,
ˇˇ1 2
pageSize
ˇˇ3 ;
,
ˇˇ; <
results
ˇˇ= D
)
ˇˇD E
;
ˇˇE F
}
ÄÄ 	
}
ÅÅ 
}ÇÇ ◊
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\QuoteRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
QuoteRepository  
:! "
RepositoryBase# 1
<1 2
Quote2 7
,7 8
Quote9 >
,> ? 
ApplicationDbContext@ T
>T U
,U V
IQuoteRepositoryW g
{ 
public 
QuoteRepository 
(  
ApplicationDbContext 3
	dbContext4 =
,= >
IMapper? F
mapperG M
)M N
:O P
baseQ U
(U V
	dbContextV _
,_ `
mappera g
)g h
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Quote 
?  
>  !
FindByIdAsync" /
(/ 0
Guid0 4
id5 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Quote$$ $
>$$$ %
>$$% &
FindByIdsAsync$$' 5
($$5 6
Guid$$6 :
[$$: ;
]$$; <
ids$$= @
,$$@ A
CancellationToken$$B S
cancellationToken$$T e
=$$f g
default$$h o
)$$o p
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) Á
ûD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\ProductRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
ProductRepository "
:# $
RepositoryBase% 3
<3 4
Product4 ;
,; <
Product= D
,D E 
ApplicationDbContextF Z
>Z [
,[ \
IProductRepository] o
{ 
public 
ProductRepository  
(  ! 
ApplicationDbContext! 5
	dbContext6 ?
,? @
IMapperA H
mapperI O
)O P
:Q R
baseS W
(W X
	dbContextX a
,a b
mapperc i
)i j
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Product !
?! "
>" #
FindByIdAsync$ 1
(1 2
Guid2 6
id7 9
,9 :
CancellationToken; L
cancellationTokenM ^
=_ `
defaulta h
)h i
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Product$$ &
>$$& '
>$$' (
FindByIdsAsync$$) 7
($$7 8
Guid$$8 <
[$$< =
]$$= >
ids$$? B
,$$B C
CancellationToken$$D U
cancellationToken$$V g
=$$h i
default$$j q
)$$q r
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) ﬂ
ùD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\PersonRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
PersonRepository !
:" #
RepositoryBase$ 2
<2 3
Person3 9
,9 :
Person; A
,A B 
ApplicationDbContextC W
>W X
,X Y
IPersonRepositoryZ k
{ 
public 
PersonRepository 
(   
ApplicationDbContext  4
	dbContext5 >
,> ?
IMapper@ G
mapperH N
)N O
:P Q
baseR V
(V W
	dbContextW `
,` a
mapperb h
)h i
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Person  
?  !
>! "
FindByIdAsync# 0
(0 1
Guid1 5
id6 8
,8 9
CancellationToken: K
cancellationTokenL ]
=^ _
default` g
)g h
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Person$$ %
>$$% &
>$$& '
FindByIdsAsync$$( 6
($$6 7
Guid$$7 ;
[$$; <
]$$< =
ids$$> A
,$$A B
CancellationToken$$C T
cancellationToken$$U f
=$$g h
default$$i p
)$$p q
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) Ô
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\PagingTSRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
PagingTSRepository #
:$ %
RepositoryBase& 4
<4 5
PagingTS5 =
,= >
PagingTS? G
,G H 
ApplicationDbContextI ]
>] ^
,^ _
IPagingTSRepository` s
{ 
public 
PagingTSRepository !
(! " 
ApplicationDbContext" 6
	dbContext7 @
,@ A
IMapperB I
mapperJ P
)P Q
:R S
baseT X
(X Y
	dbContextY b
,b c
mapperd j
)j k
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
PagingTS "
?" #
># $
FindByIdAsync% 2
(2 3
Guid3 7
id8 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
PagingTS$$ '
>$$' (
>$$( )
FindByIdsAsync$$* 8
($$8 9
Guid$$9 =
[$$= >
]$$> ?
ids$$@ C
,$$C D
CancellationToken$$E V
cancellationToken$$W h
=$$i j
default$$k r
)$$r s
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) ◊
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\OrderRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
OrderRepository  
:! "
RepositoryBase# 1
<1 2
Order2 7
,7 8
Order9 >
,> ? 
ApplicationDbContext@ T
>T U
,U V
IOrderRepositoryW g
{ 
public 
OrderRepository 
(  
ApplicationDbContext 3
	dbContext4 =
,= >
IMapper? F
mapperG M
)M N
:O P
baseQ U
(U V
	dbContextV _
,_ `
mappera g
)g h
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Order 
?  
>  !
FindByIdAsync" /
(/ 0
Guid0 4
id5 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Order$$ $
>$$$ %
>$$% &
FindByIdsAsync$$' 5
($$5 6
Guid$$6 :
[$$: ;
]$$; <
ids$$= @
,$$@ A
CancellationToken$$B S
cancellationToken$$T e
=$$f g
default$$h o
)$$o p
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) Ô
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\OptionalRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
OptionalRepository #
:$ %
RepositoryBase& 4
<4 5
Optional5 =
,= >
Optional? G
,G H 
ApplicationDbContextI ]
>] ^
,^ _
IOptionalRepository` s
{ 
public 
OptionalRepository !
(! " 
ApplicationDbContext" 6
	dbContext7 @
,@ A
IMapperB I
mapperJ P
)P Q
:R S
baseT X
(X Y
	dbContextY b
,b c
mapperd j
)j k
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Optional "
?" #
># $
FindByIdAsync% 2
(2 3
Guid3 7
id8 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Optional$$ '
>$$' (
>$$( )
FindByIdsAsync$$* 8
($$8 9
Guid$$9 =
[$$= >
]$$> ?
ids$$@ C
,$$C D
CancellationToken$$E V
cancellationToken$$W h
=$$i j
default$$k r
)$$r s
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) Œ
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\MappingTests\NestingParentRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M
MappingTestsM Y
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
class #
NestingParentRepository (
:) *
RepositoryBase+ 9
<9 :
NestingParent: G
,G H
NestingParentI V
,V W 
ApplicationDbContextX l
>l m
,m n%
INestingParentRepository	o á
{ 
public #
NestingParentRepository &
(& ' 
ApplicationDbContext' ;
	dbContext< E
,E F
IMapperG N
mapperO U
)U V
:W X
baseY ]
(] ^
	dbContext^ g
,g h
mapperi o
)o p
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
NestingParent   '
?  ' (
>  ( )
FindByIdAsync  * 7
(  7 8
Guid  8 <
id  = ?
,  ? @
CancellationToken  A R
cancellationToken  S d
=  e f
default  g n
)  n o
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
NestingParent%% ,
>%%, -
>%%- .
FindByIdsAsync%%/ =
(%%= >
Guid%%> B
[%%B C
]%%C D
ids%%E H
,%%H I
CancellationToken%%J [
cancellationToken%%\ m
=%%n o
default%%p w
)%%w x
{&& 	
return'' 
await'' 
FindAllAsync'' %
(''% &
x''& '
=>''( *
ids''+ .
.''. /
Contains''/ 7
(''7 8
x''8 9
.''9 :
Id'': <
)''< =
,''= >
cancellationToken''? P
)''P Q
;''Q R
}(( 	
})) 
}** ∆
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\Indexing\FilteredIndexRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M
IndexingM U
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
class #
FilteredIndexRepository (
:) *
RepositoryBase+ 9
<9 :
FilteredIndex: G
,G H
FilteredIndexI V
,V W 
ApplicationDbContextX l
>l m
,m n%
IFilteredIndexRepository	o á
{ 
public #
FilteredIndexRepository &
(& ' 
ApplicationDbContext' ;
	dbContext< E
,E F
IMapperG N
mapperO U
)U V
:W X
baseY ]
(] ^
	dbContext^ g
,g h
mapperi o
)o p
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
FilteredIndex   '
?  ' (
>  ( )
FindByIdAsync  * 7
(  7 8
Guid  8 <
id  = ?
,  ? @
CancellationToken  A R
cancellationToken  S d
=  e f
default  g n
)  n o
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
FilteredIndex%% ,
>%%, -
>%%- .
FindByIdsAsync%%/ =
(%%= >
Guid%%> B
[%%B C
]%%C D
ids%%E H
,%%H I
CancellationToken%%J [
cancellationToken%%\ m
=%%n o
default%%p w
)%%w x
{&& 	
return'' 
await'' 
FindAllAsync'' %
(''% &
x''& '
=>''( *
ids''+ .
.''. /
Contains''/ 7
(''7 8
x''8 9
.''9 :
Id'': <
)''< =
,''= >
cancellationToken''? P
)''P Q
;''Q R
}(( 	
})) 
}** ∏
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\FuneralCoverQuoteRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class '
FuneralCoverQuoteRepository ,
:- .
RepositoryBase/ =
<= >
FuneralCoverQuote> O
,O P
FuneralCoverQuoteQ b
,b c 
ApplicationDbContextd x
>x y
,y z)
IFuneralCoverQuoteRepository	{ ó
{ 
public '
FuneralCoverQuoteRepository *
(* + 
ApplicationDbContext+ ?
	dbContext@ I
,I J
IMapperK R
mapperS Y
)Y Z
:[ \
base] a
(a b
	dbContextb k
,k l
mapperm s
)s t
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
FuneralCoverQuote +
?+ ,
>, -
FindByIdAsync. ;
(; <
Guid< @
idA C
,C D
CancellationTokenE V
cancellationTokenW h
=i j
defaultk r
)r s
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
FuneralCoverQuote$$ 0
>$$0 1
>$$1 2
FindByIdsAsync$$3 A
($$A B
Guid%% 
[%% 
]%% 
ids%% 
,%% 
CancellationToken&& 
cancellationToken&& /
=&&0 1
default&&2 9
)&&9 :
{'' 	
return(( 
await(( 
FindAllAsync(( %
(((% &
x((& '
=>((( *
ids((+ .
.((. /
Contains((/ 7
(((7 8
x((8 9
.((9 :
Id((: <
)((< =
,((= >
cancellationToken((? P
)((P Q
;((Q R
})) 	
}** 
}++ ˇ
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\FileUploadRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class  
FileUploadRepository %
:& '
RepositoryBase( 6
<6 7

FileUpload7 A
,A B

FileUploadC M
,M N 
ApplicationDbContextO c
>c d
,d e!
IFileUploadRepositoryf {
{ 
public  
FileUploadRepository #
(# $ 
ApplicationDbContext$ 8
	dbContext9 B
,B C
IMapperD K
mapperL R
)R S
:T U
baseV Z
(Z [
	dbContext[ d
,d e
mapperf l
)l m
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 

FileUpload $
?$ %
>% &
FindByIdAsync' 4
(4 5
Guid5 9
id: <
,< =
CancellationToken> O
cancellationTokenP a
=b c
defaultd k
)k l
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 

FileUpload$$ )
>$$) *
>$$* +
FindByIdsAsync$$, :
($$: ;
Guid$$; ?
[$$? @
]$$@ A
ids$$B E
,$$E F
CancellationToken$$G X
cancellationToken$$Y j
=$$k l
default$$m t
)$$t u
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) Ù
æD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\ExtensiveDomainServices\ConcreteEntityBRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M#
ExtensiveDomainServicesM d
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
class %
ConcreteEntityBRepository *
:+ ,
RepositoryBase- ;
<; <
ConcreteEntityB< K
,K L
ConcreteEntityBM \
,\ ] 
ApplicationDbContext^ r
>r s
,s t'
IConcreteEntityBRepository	u è
{ 
public %
ConcreteEntityBRepository (
(( ) 
ApplicationDbContext) =
	dbContext> G
,G H
IMapperI P
mapperQ W
)W X
:Y Z
base[ _
(_ `
	dbContext` i
,i j
mapperk q
)q r
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
ConcreteEntityB   )
?  ) *
>  * +
FindByIdAsync  , 9
(  9 :
Guid  : >
id  ? A
,  A B
CancellationToken  C T
cancellationToken  U f
=  g h
default  i p
)  p q
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
ConcreteEntityB%% .
>%%. /
>%%/ 0
FindByIdsAsync%%1 ?
(%%? @
Guid%%@ D
[%%D E
]%%E F
ids%%G J
,%%J K
CancellationToken%%L ]
cancellationToken%%^ o
=%%p q
default%%r y
)%%y z
{&& 	
return'' 
await'' 
FindAllAsync'' %
(''% &
x''& '
=>''( *
ids''+ .
.''. /
Contains''/ 7
(''7 8
x''8 9
.''9 :
Id'': <
)''< =
,''= >
cancellationToken''? P
)''P Q
;''Q R
}(( 	
})) 
}** Ù
æD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\ExtensiveDomainServices\ConcreteEntityARepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M#
ExtensiveDomainServicesM d
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
class %
ConcreteEntityARepository *
:+ ,
RepositoryBase- ;
<; <
ConcreteEntityA< K
,K L
ConcreteEntityAM \
,\ ] 
ApplicationDbContext^ r
>r s
,s t'
IConcreteEntityARepository	u è
{ 
public %
ConcreteEntityARepository (
(( ) 
ApplicationDbContext) =
	dbContext> G
,G H
IMapperI P
mapperQ W
)W X
:Y Z
base[ _
(_ `
	dbContext` i
,i j
mapperk q
)q r
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
ConcreteEntityA   )
?  ) *
>  * +
FindByIdAsync  , 9
(  9 :
Guid  : >
id  ? A
,  A B
CancellationToken  C T
cancellationToken  U f
=  g h
default  i p
)  p q
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
ConcreteEntityA%% .
>%%. /
>%%/ 0
FindByIdsAsync%%1 ?
(%%? @
Guid%%@ D
[%%D E
]%%E F
ids%%G J
,%%J K
CancellationToken%%L ]
cancellationToken%%^ o
=%%p q
default%%r y
)%%y z
{&& 	
return'' 
await'' 
FindAllAsync'' %
(''% &
x''& '
=>''( *
ids''+ .
.''. /
Contains''/ 7
(''7 8
x''8 9
.''9 :
Id'': <
)''< =
,''= >
cancellationToken''? P
)''P Q
;''Q R
}(( 	
})) 
}** ”
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\ExtensiveDomainServices\BaseEntityBRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M#
ExtensiveDomainServicesM d
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
class !
BaseEntityBRepository &
:' (
RepositoryBase) 7
<7 8
BaseEntityB8 C
,C D
BaseEntityBE P
,P Q 
ApplicationDbContextR f
>f g
,g h"
IBaseEntityBRepositoryi 
{ 
public !
BaseEntityBRepository $
($ % 
ApplicationDbContext% 9
	dbContext: C
,C D
IMapperE L
mapperM S
)S T
:U V
baseW [
([ \
	dbContext\ e
,e f
mapperg m
)m n
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
BaseEntityB   %
?  % &
>  & '
FindByIdAsync  ( 5
(  5 6
Guid  6 :
id  ; =
,  = >
CancellationToken  ? P
cancellationToken  Q b
=  c d
default  e l
)  l m
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
BaseEntityB%% *
>%%* +
>%%+ ,
FindByIdsAsync%%- ;
(%%; <
Guid%%< @
[%%@ A
]%%A B
ids%%C F
,%%F G
CancellationToken%%H Y
cancellationToken%%Z k
=%%l m
default%%n u
)%%u v
{&& 	
return'' 
await'' 
FindAllAsync'' %
(''% &
x''& '
=>''( *
ids''+ .
.''. /
Contains''/ 7
(''7 8
x''8 9
.''9 :
Id'': <
)''< =
,''= >
cancellationToken''? P
)''P Q
;''Q R
}(( 	
})) 
}** ”
∫D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\ExtensiveDomainServices\BaseEntityARepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M#
ExtensiveDomainServicesM d
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
class !
BaseEntityARepository &
:' (
RepositoryBase) 7
<7 8
BaseEntityA8 C
,C D
BaseEntityAE P
,P Q 
ApplicationDbContextR f
>f g
,g h"
IBaseEntityARepositoryi 
{ 
public !
BaseEntityARepository $
($ % 
ApplicationDbContext% 9
	dbContext: C
,C D
IMapperE L
mapperM S
)S T
:U V
baseW [
([ \
	dbContext\ e
,e f
mapperg m
)m n
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
BaseEntityA   %
?  % &
>  & '
FindByIdAsync  ( 5
(  5 6
Guid  6 :
id  ; =
,  = >
CancellationToken  ? P
cancellationToken  Q b
=  c d
default  e l
)  l m
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
BaseEntityA%% *
>%%* +
>%%+ ,
FindByIdsAsync%%- ;
(%%; <
Guid%%< @
[%%@ A
]%%A B
ids%%C F
,%%F G
CancellationToken%%H Y
cancellationToken%%Z k
=%%l m
default%%n u
)%%u v
{&& 	
return'' 
await'' 
FindAllAsync'' %
(''% &
x''& '
=>''( *
ids''+ .
.''. /
Contains''/ 7
(''7 8
x''8 9
.''9 :
Id'': <
)''< =
,''= >
cancellationToken''? P
)''P Q
;''Q R
}(( 	
})) 
}** Ú
∑D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\DomainServices\DomainServiceTestRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M
DomainServicesM [
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
class '
DomainServiceTestRepository ,
:- .
RepositoryBase/ =
<= >
DomainServiceTest> O
,O P
DomainServiceTestQ b
,b c 
ApplicationDbContextd x
>x y
,y z)
IDomainServiceTestRepository	{ ó
{ 
public '
DomainServiceTestRepository *
(* + 
ApplicationDbContext+ ?
	dbContext@ I
,I J
IMapperK R
mapperS Y
)Y Z
:[ \
base] a
(a b
	dbContextb k
,k l
mapperm s
)s t
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   
DomainServiceTest   +
?  + ,
>  , -
FindByIdAsync  . ;
(  ; <
Guid  < @
id  A C
,  C D
CancellationToken  E V
cancellationToken  W h
=  i j
default  k r
)  r s
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% 
DomainServiceTest%% 0
>%%0 1
>%%1 2
FindByIdsAsync%%3 A
(%%A B
Guid&& 
[&& 
]&& 
ids&& 
,&& 
CancellationToken'' 
cancellationToken'' /
=''0 1
default''2 9
)''9 :
{(( 	
return)) 
await)) 
FindAllAsync)) %
())% &
x))& '
=>))( *
ids))+ .
.)). /
Contains))/ 7
())7 8
x))8 9
.))9 :
Id)): <
)))< =
,))= >
cancellationToken))? P
)))P Q
;))Q R
}** 	
}++ 
},, ∞
æD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\DomainServices\ClassicDomainServiceTestRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M
DomainServicesM [
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
class .
"ClassicDomainServiceTestRepository 3
:4 5
RepositoryBase6 D
<D E$
ClassicDomainServiceTestE ]
,] ^$
ClassicDomainServiceTest_ w
,w x!
ApplicationDbContext	y ç
>
ç é
,
é è1
#IClassicDomainServiceTestRepository
ê ≥
{ 
public .
"ClassicDomainServiceTestRepository 1
(1 2 
ApplicationDbContext2 F
	dbContextG P
,P Q
IMapperR Y
mapperZ `
)` a
:b c
based h
(h i
	dbContexti r
,r s
mappert z
)z {
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   $
ClassicDomainServiceTest   2
?  2 3
>  3 4
FindByIdAsync  5 B
(  B C
Guid  C G
id  H J
,  J K
CancellationToken  L ]
cancellationToken  ^ o
=  p q
default  r y
)  y z
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% $
ClassicDomainServiceTest%% 7
>%%7 8
>%%8 9
FindByIdsAsync%%: H
(%%H I
Guid&& 
[&& 
]&& 
ids&& 
,&& 
CancellationToken'' 
cancellationToken'' /
=''0 1
default''2 9
)''9 :
{(( 	
return)) 
await)) 
FindAllAsync)) %
())% &
x))& '
=>))( *
ids))+ .
.)). /
Contains))/ 7
())7 8
x))8 9
.))9 :
Id)): <
)))< =
,))= >
cancellationToken))? P
)))P Q
;))Q R
}** 	
}++ 
},, Ô
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\CustomerRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
CustomerRepository #
:$ %
RepositoryBase& 4
<4 5
Customer5 =
,= >
Customer? G
,G H 
ApplicationDbContextI ]
>] ^
,^ _
ICustomerRepository` s
{ 
public 
CustomerRepository !
(! " 
ApplicationDbContext" 6
	dbContext7 @
,@ A
IMapperB I
mapperJ P
)P Q
:R S
baseT X
(X Y
	dbContextY b
,b c
mapperd j
)j k
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Customer "
?" #
># $
FindByIdAsync% 2
(2 3
Guid3 7
id8 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Customer$$ '
>$$' (
>$$( )
FindByIdsAsync$$* 8
($$8 9
Guid$$9 =
[$$= >
]$$> ?
ids$$@ C
,$$C D
CancellationToken$$E V
cancellationToken$$W h
=$$i j
default$$k r
)$$r s
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) Ü
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\CorporateFuneralCoverQuoteRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 0
$CorporateFuneralCoverQuoteRepository 5
:6 7
RepositoryBase8 F
<F G&
CorporateFuneralCoverQuoteG a
,a b&
CorporateFuneralCoverQuotec }
,} ~!
ApplicationDbContext	 ì
>
ì î
,
î ï3
%ICorporateFuneralCoverQuoteRepository
ñ ª
{ 
public 0
$CorporateFuneralCoverQuoteRepository 3
(3 4 
ApplicationDbContext4 H
	dbContextI R
,R S
IMapperT [
mapper\ b
)b c
:d e
basef j
(j k
	dbContextk t
,t u
mapperv |
)| }
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< &
CorporateFuneralCoverQuote 4
?4 5
>5 6
FindByIdAsync7 D
(D E
Guid   
id   
,   
CancellationToken!! 
cancellationToken!! /
=!!0 1
default!!2 9
)!!9 :
{"" 	
return## 
await## 
	FindAsync## "
(##" #
x### $
=>##% '
x##( )
.##) *
Id##* ,
==##- /
id##0 2
,##2 3
cancellationToken##4 E
)##E F
;##F G
}$$ 	
public&& 
async&& 
Task&& 
<&& 
List&& 
<&& &
CorporateFuneralCoverQuote&& 9
>&&9 :
>&&: ;
FindByIdsAsync&&< J
(&&J K
Guid'' 
['' 
]'' 
ids'' 
,'' 
CancellationToken(( 
cancellationToken(( /
=((0 1
default((2 9
)((9 :
{)) 	
return** 
await** 
FindAllAsync** %
(**% &
x**& '
=>**( *
ids**+ .
.**. /
Contains**/ 7
(**7 8
x**8 9
.**9 :
Id**: <
)**< =
,**= >
cancellationToken**? P
)**P Q
;**Q R
}++ 	
},, 
}-- Ô
üD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\ContractRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
ContractRepository #
:$ %
RepositoryBase& 4
<4 5
Contract5 =
,= >
Contract? G
,G H 
ApplicationDbContextI ]
>] ^
,^ _
IContractRepository` s
{ 
public 
ContractRepository !
(! " 
ApplicationDbContext" 6
	dbContext7 @
,@ A
IMapperB I
mapperJ P
)P Q
:R S
baseT X
(X Y
	dbContextY b
,b c
mapperd j
)j k
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Contract "
?" #
># $
FindByIdAsync% 2
(2 3
Guid3 7
id8 :
,: ;
CancellationToken< M
cancellationTokenN _
=` a
defaultb i
)i j
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Contract$$ '
>$$' (
>$$( )
FindByIdsAsync$$* 8
($$8 9
Guid$$9 =
[$$= >
]$$> ?
ids$$@ C
,$$C D
CancellationToken$$E V
cancellationToken$$W h
=$$i j
default$$k r
)$$r s
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) ◊
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\BasicRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
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
class 
BasicRepository  
:! "
RepositoryBase# 1
<1 2
Basic2 7
,7 8
Basic9 >
,> ? 
ApplicationDbContext@ T
>T U
,U V
IBasicRepositoryW g
{ 
public 
BasicRepository 
(  
ApplicationDbContext 3
	dbContext4 =
,= >
IMapper? F
mapperG M
)M N
:O P
baseQ U
(U V
	dbContextV _
,_ `
mappera g
)g h
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public 
async 
Task 
< 
Basic 
?  
>  !
FindByIdAsync" /
(/ 0
Guid0 4
id5 7
,7 8
CancellationToken9 J
cancellationTokenK \
=] ^
default_ f
)f g
{   	
return!! 
await!! 
	FindAsync!! "
(!!" #
x!!# $
=>!!% '
x!!( )
.!!) *
Id!!* ,
==!!- /
id!!0 2
,!!2 3
cancellationToken!!4 E
)!!E F
;!!F G
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
List$$ 
<$$ 
Basic$$ $
>$$$ %
>$$% &
FindByIdsAsync$$' 5
($$5 6
Guid$$6 :
[$$: ;
]$$; <
ids$$= @
,$$@ A
CancellationToken$$B S
cancellationToken$$T e
=$$f g
default$$h o
)$$o p
{%% 	
return&& 
await&& 
FindAllAsync&& %
(&&% &
x&&& '
=>&&( *
ids&&+ .
.&&. /
Contains&&/ 7
(&&7 8
x&&8 9
.&&9 :
Id&&: <
)&&< =
,&&= >
cancellationToken&&? P
)&&P Q
;&&Q R
}'' 	
}(( 
})) í
∏D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Repositories\AnemicChild\ParentWithAnemicChildRepository.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Repositories@ L
.L M
AnemicChildM X
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
class +
ParentWithAnemicChildRepository 0
:1 2
RepositoryBase3 A
<A B!
ParentWithAnemicChildB W
,W X!
ParentWithAnemicChildY n
,n o!
ApplicationDbContext	p Ñ
>
Ñ Ö
,
Ö Ü.
 IParentWithAnemicChildRepository
á ß
{ 
public +
ParentWithAnemicChildRepository .
(. / 
ApplicationDbContext/ C
	dbContextD M
,M N
IMapperO V
mapperW ]
)] ^
:_ `
basea e
(e f
	dbContextf o
,o p
mapperq w
)w x
{ 	
} 	
public 
async 
Task 
< 
TProjection %
?% &
>& '"
FindByIdProjectToAsync( >
<> ?
TProjection? J
>J K
(K L
Guid 
id 
, 
CancellationToken 
cancellationToken /
=0 1
default2 9
)9 :
{ 	
return 
await 
FindProjectToAsync +
<+ ,
TProjection, 7
>7 8
(8 9
x9 :
=>; =
x> ?
.? @
Id@ B
==C E
idF H
,H I
cancellationTokenJ [
)[ \
;\ ]
} 	
public   
async   
Task   
<   !
ParentWithAnemicChild   /
?  / 0
>  0 1
FindByIdAsync  2 ?
(  ? @
Guid  @ D
id  E G
,  G H
CancellationToken  I Z
cancellationToken  [ l
=  m n
default  o v
)  v w
{!! 	
return"" 
await"" 
	FindAsync"" "
(""" #
x""# $
=>""% '
x""( )
."") *
Id""* ,
==""- /
id""0 2
,""2 3
cancellationToken""4 E
)""E F
;""F G
}## 	
public%% 
async%% 
Task%% 
<%% 
List%% 
<%% !
ParentWithAnemicChild%% 4
>%%4 5
>%%5 6
FindByIdsAsync%%7 E
(%%E F
Guid&& 
[&& 
]&& 
ids&& 
,&& 
CancellationToken'' 
cancellationToken'' /
=''0 1
default''2 9
)''9 :
{(( 	
return)) 
await)) 
FindAllAsync)) %
())% &
x))& '
=>))( *
ids))+ .
.)). /
Contains))/ 7
())7 8
x))8 9
.))9 :
Id)): <
)))< =
,))= >
cancellationToken))? P
)))P Q
;))Q R
}** 	
}++ 
},, –
±D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\WarehouseConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
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
0 1
Infrastructure

1 ?
.

? @
Persistence

@ K
.

K L
Configurations

L Z
{ 
public 

class "
WarehouseConfiguration '
:( )$
IEntityTypeConfiguration* B
<B C
	WarehouseC L
>L M
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
	Warehouse0 9
>9 :
builder; B
)B C
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Size$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
OwnsOne 
( 
x 
=>  
x! "
." #
Address# *
,* +
ConfigureAddress, <
)< =
;= >
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
public 
static 
void 
ConfigureAddress +
(+ ,"
OwnedNavigationBuilder, B
<B C
	WarehouseC L
,L M
AddressN U
>U V
builderW ^
)^ _
{ 	
builder 
. 
	WithOwner 
( 
) 
;  
builder!! 
.!! 
Property!! 
(!! 
x!! 
=>!! !
x!!" #
.!!# $
Line1!!$ )
)!!) *
."" 

IsRequired"" 
("" 
)"" 
;"" 
builder$$ 
.$$ 
Property$$ 
($$ 
x$$ 
=>$$ !
x$$" #
.$$# $
Line2$$$ )
)$$) *
.%% 

IsRequired%% 
(%% 
)%% 
;%% 
builder'' 
.'' 
Property'' 
('' 
x'' 
=>'' !
x''" #
.''# $
City''$ (
)''( )
.(( 

IsRequired(( 
((( 
)(( 
;(( 
builder** 
.** 
Property** 
(** 
x** 
=>** !
x**" #
.**# $
Postal**$ *
)*** +
.++ 

IsRequired++ 
(++ 
)++ 
;++ 
},, 	
}-- 
}.. Ñ>
¨D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\UserConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class 
UserConfiguration "
:# $$
IEntityTypeConfiguration% =
<= >
User> B
>B C
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
User0 4
>4 5
builder6 =
)= >
{ 	
builder 
. 
HasBaseType 
<  
Person  &
>& '
(' (
)( )
;) *
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Email$ )
)) *
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
QuoteId$ +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
HasOne 
( 
x 
=> 
x  !
.! "
Quote" '
)' (
. 
WithMany 
( 
) 
. 
HasForeignKey 
( 
x  
=>! #
x$ %
.% &
QuoteId& -
)- .
. 
OnDelete 
( 
DeleteBehavior (
.( )
Restrict) 1
)1 2
;2 3
builder 
. 
OwnsMany 
( 
x 
=> !
x" #
.# $
	Addresses$ -
,- .
ConfigureAddresses/ A
)A B
;B C
builder 
. 
OwnsOne 
( 
x 
=>  
x! "
." #"
DefaultDeliveryAddress# 9
,9 :+
ConfigureDefaultDeliveryAddress; Z
)Z [
. 

Navigation 
( 
x 
=>  
x! "
." #"
DefaultDeliveryAddress# 9
)9 :
.: ;

IsRequired; E
(E F
)F G
;G H
builder!! 
.!! 
OwnsOne!! 
(!! 
x!! 
=>!!  
x!!! "
.!!" #!
DefaultBillingAddress!!# 8
,!!8 9*
ConfigureDefaultBillingAddress!!: X
)!!X Y
;!!Y Z
}"" 	
public$$ 
static$$ 
void$$ 
ConfigureAddresses$$ -
($$- ."
OwnedNavigationBuilder$$. D
<$$D E
User$$E I
,$$I J
UserAddress$$K V
>$$V W
builder$$X _
)$$_ `
{%% 	
builder&& 
.&& 
	WithOwner&& 
(&& 
)&& 
.'' 
HasForeignKey'' 
('' 
x''  
=>''! #
x''$ %
.''% &
UserId''& ,
)'', -
;''- .
builder)) 
.)) 
HasKey)) 
()) 
x)) 
=>)) 
x))  !
.))! "
Id))" $
)))$ %
;))% &
builder++ 
.++ 
Property++ 
(++ 
x++ 
=>++ !
x++" #
.++# $
UserId++$ *
)++* +
.,, 

IsRequired,, 
(,, 
),, 
;,, 
builder.. 
... 
Property.. 
(.. 
x.. 
=>.. !
x.." #
...# $
Line1..$ )
)..) *
.// 

IsRequired// 
(// 
)// 
;// 
builder11 
.11 
Property11 
(11 
x11 
=>11 !
x11" #
.11# $
Line211$ )
)11) *
.22 

IsRequired22 
(22 
)22 
;22 
builder44 
.44 
Property44 
(44 
x44 
=>44 !
x44" #
.44# $
City44$ (
)44( )
.55 

IsRequired55 
(55 
)55 
;55 
builder77 
.77 
Property77 
(77 
x77 
=>77 !
x77" #
.77# $
Postal77$ *
)77* +
.88 

IsRequired88 
(88 
)88 
;88 
}99 	
public;; 
static;; 
void;; +
ConfigureDefaultDeliveryAddress;; :
(;;: ;"
OwnedNavigationBuilder;;; Q
<;;Q R
User;;R V
,;;V W
UserDefaultAddress;;X j
>;;j k
builder;;l s
);;s t
{<< 	
builder== 
.== 
	WithOwner== 
(== 
)== 
.>> 
HasForeignKey>> 
(>> 
x>>  
=>>>! #
x>>$ %
.>>% &
Id>>& (
)>>( )
;>>) *
builder@@ 
.@@ 
HasKey@@ 
(@@ 
x@@ 
=>@@ 
x@@  !
.@@! "
Id@@" $
)@@$ %
;@@% &
builderBB 
.BB 
PropertyBB 
(BB 
xBB 
=>BB !
xBB" #
.BB# $
Line1BB$ )
)BB) *
.CC 

IsRequiredCC 
(CC 
)CC 
;CC 
builderEE 
.EE 
PropertyEE 
(EE 
xEE 
=>EE !
xEE" #
.EE# $
Line2EE$ )
)EE) *
.FF 

IsRequiredFF 
(FF 
)FF 
;FF 
}GG 	
publicII 
staticII 
voidII *
ConfigureDefaultBillingAddressII 9
(II9 :"
OwnedNavigationBuilderII: P
<IIP Q
UserIIQ U
,IIU V
UserDefaultAddressIIW i
>IIi j
builderIIk r
)IIr s
{JJ 	
builderKK 
.KK 
	WithOwnerKK 
(KK 
)KK 
.LL 
HasForeignKeyLL 
(LL 
xLL  
=>LL! #
xLL$ %
.LL% &
IdLL& (
)LL( )
;LL) *
builderNN 
.NN 
HasKeyNN 
(NN 
xNN 
=>NN 
xNN  !
.NN! "
IdNN" $
)NN$ %
;NN% &
builderPP 
.PP 
PropertyPP 
(PP 
xPP 
=>PP !
xPP" #
.PP# $
Line1PP$ )
)PP) *
.QQ 

IsRequiredQQ 
(QQ 
)QQ 
;QQ 
builderSS 
.SS 
PropertySS 
(SS 
xSS 
=>SS !
xSS" #
.SS# $
Line2SS$ )
)SS) *
.TT 

IsRequiredTT 
(TT 
)TT 
;TT 
}UU 	
}VV 
}WW ˙%
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\QuoteConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class 
QuoteConfiguration #
:$ %$
IEntityTypeConfiguration& >
<> ?
Quote? D
>D E
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Quote0 5
>5 6
builder7 >
)> ?
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
RefNo$ )
)) *
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
PersonId$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
PersonEmail$ /
)/ 0
;0 1
builder 
. 
OwnsMany 
( 
x 
=> !
x" #
.# $

QuoteLines$ .
,. /
ConfigureQuoteLines0 C
)C D
;D E
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
public 
static 
void 
ConfigureQuoteLines .
(. /"
OwnedNavigationBuilder/ E
<E F
QuoteF K
,K L
	QuoteLineM V
>V W
builderX _
)_ `
{ 	
builder   
.   
	WithOwner   
(   
)   
.!! 
HasForeignKey!! 
(!! 
x!!  
=>!!! #
x!!$ %
.!!% &
QuoteId!!& -
)!!- .
;!!. /
builder## 
.## 
HasKey## 
(## 
x## 
=>## 
x##  !
.##! "
Id##" $
)##$ %
;##% &
builder%% 
.%% 
Property%% 
(%% 
x%% 
=>%% !
x%%" #
.%%# $
QuoteId%%$ +
)%%+ ,
.&& 

IsRequired&& 
(&& 
)&& 
;&& 
builder(( 
.(( 
Property(( 
((( 
x(( 
=>(( !
x((" #
.((# $
	ProductId(($ -
)((- .
.)) 

IsRequired)) 
()) 
))) 
;)) 
builder++ 
.++ 
Property++ 
(++ 
x++ 
=>++ !
x++" #
.++# $
Units++$ )
)++) *
.,, 

IsRequired,, 
(,, 
),, 
;,, 
builder.. 
... 
Property.. 
(.. 
x.. 
=>.. !
x.." #
...# $
	UnitPrice..$ -
)..- .
.// 

IsRequired// 
(// 
)// 
;// 
builder11 
.11 
HasOne11 
(11 
x11 
=>11 
x11  !
.11! "
Product11" )
)11) *
.22 
WithMany22 
(22 
)22 
.33 
HasForeignKey33 
(33 
x33  
=>33! #
x33$ %
.33% &
	ProductId33& /
)33/ 0
.44 
OnDelete44 
(44 
DeleteBehavior44 (
.44( )
Restrict44) 1
)441 2
;442 3
}55 	
}66 
}77 ∆
ØD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\ProductConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
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
0 1
Infrastructure

1 ?
.

? @
Persistence

@ K
.

K L
Configurations

L Z
{ 
public 

class  
ProductConfiguration %
:& '$
IEntityTypeConfiguration( @
<@ A
ProductA H
>H I
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Product0 7
>7 8
builder9 @
)@ A
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
OwnsMany 
( 
x 
=> !
x" #
.# $
Tags$ (
,( )
ConfigureTags* 7
)7 8
;8 9
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
public 
static 
void 
ConfigureTags (
(( )"
OwnedNavigationBuilder) ?
<? @
Product@ G
,G H
TagI L
>L M
builderN U
)U V
{ 	
builder 
. 
	WithOwner 
( 
) 
;  
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder!! 
.!! 
Property!! 
(!! 
x!! 
=>!! !
x!!" #
.!!# $
Value!!$ )
)!!) *
."" 

IsRequired"" 
("" 
)"" 
;"" 
}## 	
}$$ 
}%% ‰
ÆD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\PersonConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class 
PersonConfiguration $
:% &$
IEntityTypeConfiguration' ?
<? @
Person@ F
>F G
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Person0 6
>6 7
builder8 ?
)? @
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Surname$ +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} Ó
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\PagingTSConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class !
PagingTSConfiguration &
:' ($
IEntityTypeConfiguration) A
<A B
PagingTSB J
>J K
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
PagingTS0 8
>8 9
builder: A
)A B
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} ´J
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\OrderConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
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
0 1
Infrastructure

1 ?
.

? @
Persistence

@ K
.

K L
Configurations

L Z
{ 
public 

class 
OrderConfiguration #
:$ %$
IEntityTypeConfiguration& >
<> ?
Order? D
>D E
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Order0 5
>5 6
builder7 >
)> ?
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
RefNo$ )
)) *
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
	OrderDate$ -
)- .
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
OrderStatus$ /
)/ 0
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $

CustomerId$ .
). /
. 

IsRequired 
( 
) 
; 
builder 
. 
HasOne 
( 
x 
=> 
x  !
.! "
Customer" *
)* +
. 
WithMany 
( 
) 
.   
HasForeignKey   
(   
x    
=>  ! #
x  $ %
.  % &

CustomerId  & 0
)  0 1
.!! 
OnDelete!! 
(!! 
DeleteBehavior!! (
.!!( )
Restrict!!) 1
)!!1 2
;!!2 3
builder## 
.## 
OwnsMany## 
(## 
x## 
=>## !
x##" #
.### $

OrderItems##$ .
,##. /
ConfigureOrderItems##0 C
)##C D
;##D E
builder%% 
.%% 
OwnsOne%% 
(%% 
x%% 
=>%%  
x%%! "
.%%" #
DeliveryAddress%%# 2
,%%2 3$
ConfigureDeliveryAddress%%4 L
)%%L M
.&& 

Navigation&& 
(&& 
x&& 
=>&&  
x&&! "
.&&" #
DeliveryAddress&&# 2
)&&2 3
.&&3 4

IsRequired&&4 >
(&&> ?
)&&? @
;&&@ A
builder(( 
.(( 
OwnsOne(( 
((( 
x(( 
=>((  
x((! "
.((" #
BillingAddress((# 1
,((1 2#
ConfigureBillingAddress((3 J
)((J K
;((K L
builder** 
.** 
Ignore** 
(** 
e** 
=>** 
e**  !
.**! "
DomainEvents**" .
)**. /
;**/ 0
}++ 	
public-- 
static-- 
void-- 
ConfigureOrderItems-- .
(--. /"
OwnedNavigationBuilder--/ E
<--E F
Order--F K
,--K L
	OrderItem--M V
>--V W
builder--X _
)--_ `
{.. 	
builder// 
.// 
	WithOwner// 
(// 
)// 
.00 
HasForeignKey00 
(00 
x00  
=>00! #
x00$ %
.00% &
OrderId00& -
)00- .
;00. /
builder22 
.22 
HasKey22 
(22 
x22 
=>22 
x22  !
.22! "
Id22" $
)22$ %
;22% &
builder44 
.44 
Property44 
(44 
x44 
=>44 !
x44" #
.44# $
Quantity44$ ,
)44, -
.55 

IsRequired55 
(55 
)55 
;55 
builder77 
.77 
Property77 
(77 
x77 
=>77 !
x77" #
.77# $
Units77$ )
)77) *
.88 

IsRequired88 
(88 
)88 
;88 
builder:: 
.:: 
Property:: 
(:: 
x:: 
=>:: !
x::" #
.::# $
	UnitPrice::$ -
)::- .
.;; 

IsRequired;; 
(;; 
);; 
;;; 
builder== 
.== 
Property== 
(== 
x== 
=>== !
x==" #
.==# $
OrderId==$ +
)==+ ,
.>> 

IsRequired>> 
(>> 
)>> 
;>> 
builder@@ 
.@@ 
Property@@ 
(@@ 
x@@ 
=>@@ !
x@@" #
.@@# $
	ProductId@@$ -
)@@- .
.AA 

IsRequiredAA 
(AA 
)AA 
;AA 
builderCC 
.CC 
HasOneCC 
(CC 
xCC 
=>CC 
xCC  !
.CC! "
ProductCC" )
)CC) *
.DD 
WithManyDD 
(DD 
)DD 
.EE 
HasForeignKeyEE 
(EE 
xEE  
=>EE! #
xEE$ %
.EE% &
	ProductIdEE& /
)EE/ 0
.FF 
OnDeleteFF 
(FF 
DeleteBehaviorFF (
.FF( )
RestrictFF) 1
)FF1 2
;FF2 3
}GG 	
publicII 
staticII 
voidII $
ConfigureDeliveryAddressII 3
(II3 4"
OwnedNavigationBuilderII4 J
<IIJ K
OrderIIK P
,IIP Q
AddressIIR Y
>IIY Z
builderII[ b
)IIb c
{JJ 	
builderKK 
.KK 
	WithOwnerKK 
(KK 
)KK 
;KK  
builderMM 
.MM 
PropertyMM 
(MM 
xMM 
=>MM !
xMM" #
.MM# $
Line1MM$ )
)MM) *
.NN 

IsRequiredNN 
(NN 
)NN 
;NN 
builderPP 
.PP 
PropertyPP 
(PP 
xPP 
=>PP !
xPP" #
.PP# $
Line2PP$ )
)PP) *
.QQ 

IsRequiredQQ 
(QQ 
)QQ 
;QQ 
builderSS 
.SS 
PropertySS 
(SS 
xSS 
=>SS !
xSS" #
.SS# $
CitySS$ (
)SS( )
.TT 

IsRequiredTT 
(TT 
)TT 
;TT 
builderVV 
.VV 
PropertyVV 
(VV 
xVV 
=>VV !
xVV" #
.VV# $
PostalVV$ *
)VV* +
.WW 

IsRequiredWW 
(WW 
)WW 
;WW 
}XX 	
publicZZ 
staticZZ 
voidZZ #
ConfigureBillingAddressZZ 2
(ZZ2 3"
OwnedNavigationBuilderZZ3 I
<ZZI J
OrderZZJ O
,ZZO P
AddressZZQ X
>ZZX Y
builderZZZ a
)ZZa b
{[[ 	
builder\\ 
.\\ 
	WithOwner\\ 
(\\ 
)\\ 
;\\  
builder^^ 
.^^ 
Property^^ 
(^^ 
x^^ 
=>^^ !
x^^" #
.^^# $
Line1^^$ )
)^^) *
.__ 

IsRequired__ 
(__ 
)__ 
;__ 
builderaa 
.aa 
Propertyaa 
(aa 
xaa 
=>aa !
xaa" #
.aa# $
Line2aa$ )
)aa) *
.bb 

IsRequiredbb 
(bb 
)bb 
;bb 
builderdd 
.dd 
Propertydd 
(dd 
xdd 
=>dd !
xdd" #
.dd# $
Citydd$ (
)dd( )
.ee 

IsRequiredee 
(ee 
)ee 
;ee 
buildergg 
.gg 
Propertygg 
(gg 
xgg 
=>gg !
xgg" #
.gg# $
Postalgg$ *
)gg* +
.hh 

IsRequiredhh 
(hh 
)hh 
;hh 
}ii 	
}jj 
}kk Ó
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\OptionalConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class !
OptionalConfiguration &
:' ($
IEntityTypeConfiguration) A
<A B
OptionalB J
>J K
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Optional0 8
>8 9
builder: A
)A B
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} Ô&
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\MappingTests\NestingParentConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [
MappingTests		[ g
{

 
public 

class &
NestingParentConfiguration +
:, -$
IEntityTypeConfiguration. F
<F G
NestingParentG T
>T U
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
NestingParent0 =
>= >
builder? F
)F G
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
OwnsMany 
( 
x 
=> !
x" #
.# $
NestingChildren$ 3
,3 4$
ConfigureNestingChildren5 M
)M N
;N O
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
public 
static 
void $
ConfigureNestingChildren 3
(3 4"
OwnedNavigationBuilder4 J
<J K
NestingParentK X
,X Y
NestingChildZ f
>f g
builderh o
)o p
{ 	
builder 
. 
	WithOwner 
( 
) 
. 
HasForeignKey 
( 
x  
=>! #
x$ %
.% &
NestingParentId& 5
)5 6
;6 7
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder   
.   
Property   
(   
x   
=>   !
x  " #
.  # $
Description  $ /
)  / 0
.!! 

IsRequired!! 
(!! 
)!! 
;!! 
builder## 
.## 
Property## 
(## 
x## 
=>## !
x##" #
.### $
NestingParentId##$ 3
)##3 4
.$$ 

IsRequired$$ 
($$ 
)$$ 
;$$ 
builder&& 
.&& 
OwnsOne&& 
(&& 
x&& 
=>&&  
x&&! "
.&&" #
NestingChildChild&&# 4
,&&4 5&
ConfigureNestingChildChild&&6 P
)&&P Q
.'' 

Navigation'' 
('' 
x'' 
=>''  
x''! "
.''" #
NestingChildChild''# 4
)''4 5
.''5 6

IsRequired''6 @
(''@ A
)''A B
;''B C
}(( 	
public** 
static** 
void** &
ConfigureNestingChildChild** 5
(**5 6"
OwnedNavigationBuilder**6 L
<**L M
NestingChild**M Y
,**Y Z
NestingChildChild**[ l
>**l m
builder**n u
)**u v
{++ 	
builder,, 
.,, 
	WithOwner,, 
(,, 
),, 
.-- 
HasForeignKey-- 
(-- 
x--  
=>--! #
x--$ %
.--% &
Id--& (
)--( )
;--) *
builder// 
.// 
HasKey// 
(// 
x// 
=>// 
x//  !
.//! "
Id//" $
)//$ %
;//% &
builder11 
.11 
Property11 
(11 
x11 
=>11 !
x11" #
.11# $
Name11$ (
)11( )
.22 

IsRequired22 
(22 
)22 
;22 
}33 	
}44 
}55 ≠
æD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\Indexing\FilteredIndexConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [
Indexing		[ c
{

 
public 

class &
FilteredIndexConfiguration +
:, -$
IEntityTypeConfiguration. F
<F G
FilteredIndexG T
>T U
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
FilteredIndex0 =
>= >
builder? F
)F G
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
IsActive$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
HasIndex 
( 
x 
=> !
new" %
{& '
x( )
.) *
Name* .
,. /
x0 1
.1 2
IsActive2 :
}; <
)< =
. 
	HasFilter 
( 
$str 0
)0 1
;1 2
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} ¡
πD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\FuneralCoverQuoteConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class *
FuneralCoverQuoteConfiguration /
:0 1$
IEntityTypeConfiguration2 J
<J K
FuneralCoverQuoteK \
>\ ]
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
FuneralCoverQuote0 A
>A B
builderC J
)J K
{ 	
builder 
. 
HasBaseType 
<  
Quote  %
>% &
(& '
)' (
;( )
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Amount$ *
)* +
. 

IsRequired 
( 
) 
; 
} 	
} 
} ˙
≤D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\FileUploadConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class #
FileUploadConfiguration (
:) *$
IEntityTypeConfiguration+ C
<C D

FileUploadD N
>N O
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0

FileUpload0 :
>: ;
builder< C
)C D
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Filename$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Content$ +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
ContentType$ /
)/ 0
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} ë
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\ExtensiveDomainServices\ConcreteEntityBConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [#
ExtensiveDomainServices		[ r
{

 
public 

class (
ConcreteEntityBConfiguration -
:. /$
IEntityTypeConfiguration0 H
<H I
ConcreteEntityBI X
>X Y
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
ConcreteEntityB0 ?
>? @
builderA H
)H I
{ 	
builder 
. 
HasBaseType 
<  
BaseEntityB  +
>+ ,
(, -
)- .
;. /
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
ConcreteAttr$ 0
)0 1
. 

IsRequired 
( 
) 
; 
} 	
} 
} ë
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\ExtensiveDomainServices\ConcreteEntityAConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [#
ExtensiveDomainServices		[ r
{

 
public 

class (
ConcreteEntityAConfiguration -
:. /$
IEntityTypeConfiguration0 H
<H I
ConcreteEntityAI X
>X Y
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
ConcreteEntityA0 ?
>? @
builderA H
)H I
{ 	
builder 
. 
HasBaseType 
<  
BaseEntityA  +
>+ ,
(, -
)- .
;. /
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
ConcreteAttr$ 0
)0 1
. 

IsRequired 
( 
) 
; 
} 	
} 
}  
ÀD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\ExtensiveDomainServices\BaseEntityBConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [#
ExtensiveDomainServices		[ r
{

 
public 

class $
BaseEntityBConfiguration )
:* +$
IEntityTypeConfiguration, D
<D E
BaseEntityBE P
>P Q
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
BaseEntityB0 ;
>; <
builder= D
)D E
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
BaseAttr$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
}  
ÀD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\ExtensiveDomainServices\BaseEntityAConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [#
ExtensiveDomainServices		[ r
{

 
public 

class $
BaseEntityAConfiguration )
:* +$
IEntityTypeConfiguration, D
<D E
BaseEntityAE P
>P Q
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
BaseEntityA0 ;
>; <
builder= D
)D E
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
BaseAttr$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} —
»D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\DomainServices\DomainServiceTestConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [
DomainServices		[ i
{

 
public 

class *
DomainServiceTestConfiguration /
:0 1$
IEntityTypeConfiguration2 J
<J K
DomainServiceTestK \
>\ ]
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
DomainServiceTest0 A
>A B
builderC J
)J K
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} Ì
œD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\DomainServices\ClassicDomainServiceTestConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [
DomainServices		[ i
{

 
public 

class 1
%ClassicDomainServiceTestConfiguration 6
:7 8$
IEntityTypeConfiguration9 Q
<Q R$
ClassicDomainServiceTestR j
>j k
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0$
ClassicDomainServiceTest0 H
>H I
builderJ Q
)Q R
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} ¥
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\CustomerConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class !
CustomerConfiguration &
:' ($
IEntityTypeConfiguration) A
<A B
CustomerB J
>J K
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Customer0 8
>8 9
builder: A
)A B
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Surname$ +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
IsActive$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
OwnsOne 
( 
x 
=>  
x! "
." #
Preferences# .
,. / 
ConfigurePreferences0 D
)D E
;E F
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
public 
static 
void  
ConfigurePreferences /
(/ 0"
OwnedNavigationBuilder0 F
<F G
CustomerG O
,O P
PreferencesQ \
>\ ]
builder^ e
)e f
{   	
builder!! 
.!! 
	WithOwner!! 
(!! 
)!! 
."" 
HasForeignKey"" 
("" 
x""  
=>""! #
x""$ %
.""% &
Id""& (
)""( )
;"") *
builder$$ 
.$$ 
HasKey$$ 
($$ 
x$$ 
=>$$ 
x$$  !
.$$! "
Id$$" $
)$$$ %
;$$% &
builder&& 
.&& 
Property&& 
(&& 
x&& 
=>&& !
x&&" #
.&&# $

Newsletter&&$ .
)&&. /
.'' 

IsRequired'' 
('' 
)'' 
;'' 
builder)) 
.)) 
Property)) 
()) 
x)) 
=>)) !
x))" #
.))# $
Specials))$ ,
))), -
.** 

IsRequired** 
(** 
)** 
;** 
}++ 	
},, 
}-- ˜
¬D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\CorporateFuneralCoverQuoteConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class 3
'CorporateFuneralCoverQuoteConfiguration 8
:9 :$
IEntityTypeConfiguration; S
<S T&
CorporateFuneralCoverQuoteT n
>n o
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0&
CorporateFuneralCoverQuote0 J
>J K
builderL S
)S T
{ 	
builder 
. 
HasBaseType 
<  
FuneralCoverQuote  1
>1 2
(2 3
)3 4
;4 5
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
	Corporate$ -
)- .
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Registration$ 0
)0 1
. 

IsRequired 
( 
) 
; 
} 	
} 
} Ì
∞D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\ContractConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class !
ContractConfiguration &
:' ($
IEntityTypeConfiguration) A
<A B
ContractB J
>J K
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Contract0 8
>8 9
builder: A
)A B
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
IsActive$ ,
), -
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} ‡
≠D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\BasicConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
{

 
public 

class 
BasicConfiguration #
:$ %$
IEntityTypeConfiguration& >
<> ?
Basic? D
>D E
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0
Basic0 5
>5 6
builder7 >
)> ?
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Surname$ +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
} 
} ¡"
…D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\Configurations\AnemicChild\ParentWithAnemicChildConfiguration.cs
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
$str N
,N O
VersionP W
=X Y
$strZ _
)_ `
]` a
	namespace		 	
AdvancedMappingCrud		
 
.		 
Repositories		 *
.		* +
Tests		+ 0
.		0 1
Infrastructure		1 ?
.		? @
Persistence		@ K
.		K L
Configurations		L Z
.		Z [
AnemicChild		[ f
{

 
public 

class .
"ParentWithAnemicChildConfiguration 3
:4 5$
IEntityTypeConfiguration6 N
<N O!
ParentWithAnemicChildO d
>d e
{ 
public 
void 
	Configure 
( 
EntityTypeBuilder /
</ 0!
ParentWithAnemicChild0 E
>E F
builderG N
)N O
{ 	
builder 
. 
HasKey 
( 
x 
=> 
x  !
.! "
Id" $
)$ %
;% &
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Name$ (
)( )
. 

IsRequired 
( 
) 
; 
builder 
. 
Property 
( 
x 
=> !
x" #
.# $
Surname$ +
)+ ,
. 

IsRequired 
( 
) 
; 
builder 
. 
OwnsMany 
( 
x 
=> !
x" #
.# $
AnemicChildren$ 2
,2 3#
ConfigureAnemicChildren4 K
)K L
;L M
builder 
. 
Ignore 
( 
e 
=> 
e  !
.! "
DomainEvents" .
). /
;/ 0
} 	
public 
static 
void #
ConfigureAnemicChildren 2
(2 3"
OwnedNavigationBuilder3 I
<I J!
ParentWithAnemicChildJ _
,_ `
Domaina g
.g h
Entitiesh p
.p q
AnemicChildq |
.| }
AnemicChild	} à
>
à â
builder
ä ë
)
ë í
{ 	
builder 
. 
	WithOwner 
( 
) 
. 
HasForeignKey 
( 
x  
=>! #
x$ %
.% &#
ParentWithAnemicChildId& =
)= >
;> ?
builder!! 
.!! 
HasKey!! 
(!! 
x!! 
=>!! 
x!!  !
.!!! "
Id!!" $
)!!$ %
;!!% &
builder## 
.## 
Property## 
(## 
x## 
=>## !
x##" #
.### $#
ParentWithAnemicChildId##$ ;
)##; <
.$$ 

IsRequired$$ 
($$ 
)$$ 
;$$ 
builder&& 
.&& 
Property&& 
(&& 
x&& 
=>&& !
x&&" #
.&&# $
Line1&&$ )
)&&) *
.'' 

IsRequired'' 
('' 
)'' 
;'' 
builder)) 
.)) 
Property)) 
()) 
x)) 
=>)) !
x))" #
.))# $
Line2))$ )
)))) *
.** 

IsRequired** 
(** 
)** 
;** 
builder,, 
.,, 
Property,, 
(,, 
x,, 
=>,, !
x,," #
.,,# $
City,,$ (
),,( )
.-- 

IsRequired-- 
(-- 
)-- 
;-- 
}.. 	
}// 
}00 ˛t
†D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Persistence\ApplicationDbContext.cs
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
$str @
,@ A
VersionB I
=J K
$strL Q
)Q R
]R S
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Persistence@ K
{ 
public 

class  
ApplicationDbContext %
:& '
	DbContext( 1
,1 2
IUnitOfWork3 >
{ 
private 
readonly 
IDomainEventService ,
_domainEventService- @
;@ A
public  
ApplicationDbContext #
(# $
DbContextOptions$ 4
<4 5 
ApplicationDbContext5 I
>I J
optionsK R
,R S
IDomainEventServiceT g
domainEventServiceh z
)z {
:| }
base	~ Ç
(
Ç É
options
É ä
)
ä ã
{   	
_domainEventService!! 
=!!  !
domainEventService!!" 4
;!!4 5
}"" 	
public$$ 
DbSet$$ 
<$$ 
Basic$$ 
>$$ 
Basics$$ "
{$$# $
get$$% (
;$$( )
set$$* -
;$$- .
}$$/ 0
public&& 
DbSet&& 
<&& 
Contract&& 
>&& 
	Contracts&& (
{&&) *
get&&+ .
;&&. /
set&&0 3
;&&3 4
}&&5 6
public(( 
DbSet(( 
<(( &
CorporateFuneralCoverQuote(( /
>((/ 0'
CorporateFuneralCoverQuotes((1 L
{((M N
get((O R
;((R S
set((T W
;((W X
}((Y Z
public** 
DbSet** 
<** 
Customer** 
>** 
	Customers** (
{**) *
get**+ .
;**. /
set**0 3
;**3 4
}**5 6
public++ 
DbSet++ 
<++ 

FileUpload++ 
>++  
FileUploads++! ,
{++- .
get++/ 2
;++2 3
set++4 7
;++7 8
}++9 :
public,, 
DbSet,, 
<,, 
FuneralCoverQuote,, &
>,,& '
FuneralCoverQuotes,,( :
{,,; <
get,,= @
;,,@ A
set,,B E
;,,E F
},,G H
public-- 
DbSet-- 
<-- 
Optional-- 
>-- 
	Optionals-- (
{--) *
get--+ .
;--. /
set--0 3
;--3 4
}--5 6
public.. 
DbSet.. 
<.. 
Order.. 
>.. 
Orders.. "
{..# $
get..% (
;..( )
set..* -
;..- .
}../ 0
public// 
DbSet// 
<// 
PagingTS// 
>// 
PagingTs// '
{//( )
get//* -
;//- .
set/// 2
;//2 3
}//4 5
public00 
DbSet00 
<00 
Person00 
>00 
People00 #
{00$ %
get00& )
;00) *
set00+ .
;00. /
}000 1
public11 
DbSet11 
<11 
Product11 
>11 
Products11 &
{11' (
get11) ,
;11, -
set11. 1
;111 2
}113 4
public22 
DbSet22 
<22 
Quote22 
>22 
Quotes22 "
{22# $
get22% (
;22( )
set22* -
;22- .
}22/ 0
public33 
DbSet33 
<33 
User33 
>33 
Users33  
{33! "
get33# &
;33& '
set33( +
;33+ ,
}33- .
public44 
DbSet44 
<44 
	Warehouse44 
>44 

Warehouses44  *
{44+ ,
get44- 0
;440 1
set442 5
;445 6
}447 8
public55 
DbSet55 
<55 !
ParentWithAnemicChild55 *
>55* +$
ParentWithAnemicChildren55, D
{55E F
get55G J
;55J K
set55L O
;55O P
}55Q R
public66 
DbSet66 
<66 $
ClassicDomainServiceTest66 -
>66- .%
ClassicDomainServiceTests66/ H
{66I J
get66K N
;66N O
set66P S
;66S T
}66U V
public77 
DbSet77 
<77 
DomainServiceTest77 &
>77& '
DomainServiceTests77( :
{77; <
get77= @
;77@ A
set77B E
;77E F
}77G H
public88 
DbSet88 
<88 
BaseEntityA88  
>88  !
BaseEntityAs88" .
{88/ 0
get881 4
;884 5
set886 9
;889 :
}88; <
public99 
DbSet99 
<99 
BaseEntityB99  
>99  !
BaseEntityBs99" .
{99/ 0
get991 4
;994 5
set996 9
;999 :
}99; <
public:: 
DbSet:: 
<:: 
ConcreteEntityA:: $
>::$ %
ConcreteEntityAs::& 6
{::7 8
get::9 <
;::< =
set::> A
;::A B
}::C D
public;; 
DbSet;; 
<;; 
ConcreteEntityB;; $
>;;$ %
ConcreteEntityBs;;& 6
{;;7 8
get;;9 <
;;;< =
set;;> A
;;;A B
};;C D
public<< 
DbSet<< 
<<< 
FilteredIndex<< "
><<" #
FilteredIndices<<$ 3
{<<4 5
get<<6 9
;<<9 :
set<<; >
;<<> ?
}<<@ A
public== 
DbSet== 
<== 
NestingParent== "
>==" #
NestingParents==$ 2
{==3 4
get==5 8
;==8 9
set==: =
;=== >
}==? @
public?? 
override?? 
async?? 
Task?? "
<??" #
int??# &
>??& '
SaveChangesAsync??( 8
(??8 9
bool@@ %
acceptAllChangesOnSuccess@@ *
,@@* +
CancellationTokenAA 
cancellationTokenAA /
=AA0 1
defaultAA2 9
)AA9 :
{BB 	
awaitCC 
DispatchEventsAsyncCC %
(CC% &
cancellationTokenCC& 7
)CC7 8
;CC8 9
returnDD 
awaitDD 
baseDD 
.DD 
SaveChangesAsyncDD .
(DD. /%
acceptAllChangesOnSuccessDD/ H
,DDH I
cancellationTokenDDJ [
)DD[ \
;DD\ ]
}EE 	
publicGG 
overrideGG 
intGG 
SaveChangesGG '
(GG' (
boolGG( ,%
acceptAllChangesOnSuccessGG- F
)GGF G
{HH 	
DispatchEventsAsyncII 
(II  
)II  !
.II! "

GetAwaiterII" ,
(II, -
)II- .
.II. /
	GetResultII/ 8
(II8 9
)II9 :
;II: ;
returnJJ 
baseJJ 
.JJ 
SaveChangesJJ #
(JJ# $%
acceptAllChangesOnSuccessJJ$ =
)JJ= >
;JJ> ?
}KK 	
	protectedMM 
overrideMM 
voidMM 
OnModelCreatingMM  /
(MM/ 0
ModelBuilderMM0 <
modelBuilderMM= I
)MMI J
{NN 	
baseOO 
.OO 
OnModelCreatingOO  
(OO  !
modelBuilderOO! -
)OO- .
;OO. /
ConfigureModelQQ 
(QQ 
modelBuilderQQ '
)QQ' (
;QQ( )
modelBuilderRR 
.RR 
ApplyConfigurationRR +
(RR+ ,
newRR, /
BasicConfigurationRR0 B
(RRB C
)RRC D
)RRD E
;RRE F
modelBuilderSS 
.SS 
ApplyConfigurationSS +
(SS+ ,
newSS, /!
ContractConfigurationSS0 E
(SSE F
)SSF G
)SSG H
;SSH I
modelBuilderTT 
.TT 
ApplyConfigurationTT +
(TT+ ,
newTT, /3
'CorporateFuneralCoverQuoteConfigurationTT0 W
(TTW X
)TTX Y
)TTY Z
;TTZ [
modelBuilderUU 
.UU 
ApplyConfigurationUU +
(UU+ ,
newUU, /!
CustomerConfigurationUU0 E
(UUE F
)UUF G
)UUG H
;UUH I
modelBuilderVV 
.VV 
ApplyConfigurationVV +
(VV+ ,
newVV, /#
FileUploadConfigurationVV0 G
(VVG H
)VVH I
)VVI J
;VVJ K
modelBuilderWW 
.WW 
ApplyConfigurationWW +
(WW+ ,
newWW, /*
FuneralCoverQuoteConfigurationWW0 N
(WWN O
)WWO P
)WWP Q
;WWQ R
modelBuilderXX 
.XX 
ApplyConfigurationXX +
(XX+ ,
newXX, /!
OptionalConfigurationXX0 E
(XXE F
)XXF G
)XXG H
;XXH I
modelBuilderYY 
.YY 
ApplyConfigurationYY +
(YY+ ,
newYY, /
OrderConfigurationYY0 B
(YYB C
)YYC D
)YYD E
;YYE F
modelBuilderZZ 
.ZZ 
ApplyConfigurationZZ +
(ZZ+ ,
newZZ, /!
PagingTSConfigurationZZ0 E
(ZZE F
)ZZF G
)ZZG H
;ZZH I
modelBuilder[[ 
.[[ 
ApplyConfiguration[[ +
([[+ ,
new[[, /
PersonConfiguration[[0 C
([[C D
)[[D E
)[[E F
;[[F G
modelBuilder\\ 
.\\ 
ApplyConfiguration\\ +
(\\+ ,
new\\, / 
ProductConfiguration\\0 D
(\\D E
)\\E F
)\\F G
;\\G H
modelBuilder]] 
.]] 
ApplyConfiguration]] +
(]]+ ,
new]], /
QuoteConfiguration]]0 B
(]]B C
)]]C D
)]]D E
;]]E F
modelBuilder^^ 
.^^ 
ApplyConfiguration^^ +
(^^+ ,
new^^, /
UserConfiguration^^0 A
(^^A B
)^^B C
)^^C D
;^^D E
modelBuilder__ 
.__ 
ApplyConfiguration__ +
(__+ ,
new__, /"
WarehouseConfiguration__0 F
(__F G
)__G H
)__H I
;__I J
modelBuilder`` 
.`` 
ApplyConfiguration`` +
(``+ ,
new``, /.
"ParentWithAnemicChildConfiguration``0 R
(``R S
)``S T
)``T U
;``U V
modelBuilderaa 
.aa 
ApplyConfigurationaa +
(aa+ ,
newaa, /1
%ClassicDomainServiceTestConfigurationaa0 U
(aaU V
)aaV W
)aaW X
;aaX Y
modelBuilderbb 
.bb 
ApplyConfigurationbb +
(bb+ ,
newbb, /*
DomainServiceTestConfigurationbb0 N
(bbN O
)bbO P
)bbP Q
;bbQ R
modelBuildercc 
.cc 
ApplyConfigurationcc +
(cc+ ,
newcc, /$
BaseEntityAConfigurationcc0 H
(ccH I
)ccI J
)ccJ K
;ccK L
modelBuilderdd 
.dd 
ApplyConfigurationdd +
(dd+ ,
newdd, /$
BaseEntityBConfigurationdd0 H
(ddH I
)ddI J
)ddJ K
;ddK L
modelBuilderee 
.ee 
ApplyConfigurationee +
(ee+ ,
newee, /(
ConcreteEntityAConfigurationee0 L
(eeL M
)eeM N
)eeN O
;eeO P
modelBuilderff 
.ff 
ApplyConfigurationff +
(ff+ ,
newff, /(
ConcreteEntityBConfigurationff0 L
(ffL M
)ffM N
)ffN O
;ffO P
modelBuildergg 
.gg 
ApplyConfigurationgg +
(gg+ ,
newgg, /&
FilteredIndexConfigurationgg0 J
(ggJ K
)ggK L
)ggL M
;ggM N
modelBuilderhh 
.hh 
ApplyConfigurationhh +
(hh+ ,
newhh, /&
NestingParentConfigurationhh0 J
(hhJ K
)hhK L
)hhL M
;hhM N
}ii 	
[kk 	
IntentManagedkk	 
(kk 
Modekk 
.kk 
Ignorekk "
)kk" #
]kk# $
privatell 
voidll 
ConfigureModelll #
(ll# $
ModelBuilderll$ 0
modelBuilderll1 =
)ll= >
{mm 	
}ww 	
privateyy 
asyncyy 
Taskyy 
DispatchEventsAsyncyy .
(yy. /
CancellationTokenyy/ @
cancellationTokenyyA R
=yyS T
defaultyyU \
)yy\ ]
{zz 	
while{{ 
({{ 
true{{ 
){{ 
{|| 
var}} 
domainEventEntity}} %
=}}& '
ChangeTracker}}( 5
.~~ 
Entries~~ 
<~~ 
IHasDomainEvent~~ ,
>~~, -
(~~- .
)~~. /
. 
Select 
( 
x 
=>  
x! "
." #
Entity# )
.) *
DomainEvents* 6
)6 7
.
ÄÄ 

SelectMany
ÄÄ 
(
ÄÄ  
x
ÄÄ  !
=>
ÄÄ" $
x
ÄÄ% &
)
ÄÄ& '
.
ÅÅ 
FirstOrDefault
ÅÅ #
(
ÅÅ# $
domainEvent
ÅÅ$ /
=>
ÅÅ0 2
!
ÅÅ3 4
domainEvent
ÅÅ4 ?
.
ÅÅ? @
IsPublished
ÅÅ@ K
)
ÅÅK L
;
ÅÅL M
if
ÉÉ 
(
ÉÉ 
domainEventEntity
ÉÉ %
is
ÉÉ& (
null
ÉÉ) -
)
ÉÉ- .
{
ÑÑ 
break
ÖÖ 
;
ÖÖ 
}
ÜÜ 
domainEventEntity
àà !
.
àà! "
IsPublished
àà" -
=
àà. /
true
àà0 4
;
àà4 5
await
ââ !
_domainEventService
ââ )
.
ââ) *
Publish
ââ* 1
(
ââ1 2
domainEventEntity
ââ2 C
,
ââC D
cancellationToken
ââE V
)
ââV W
;
ââW X
}
ää 
}
ãã 	
}
åå 
}çç ¯à
©D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\HttpClients\ProductServiceProxyHttpClient.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
HttpClients@ K
{ 
public 

class )
ProductServiceProxyHttpClient .
:/ 0 
IProductServiceProxy1 E
{ 
private 
readonly !
JsonSerializerOptions .
_serializerOptions/ A
;A B
private 
readonly 

HttpClient #
_httpClient$ /
;/ 0
public )
ProductServiceProxyHttpClient ,
(, -

HttpClient- 7

httpClient8 B
)B C
{ 	
_httpClient 
= 

httpClient $
;$ %
_serializerOptions 
=  
new! $!
JsonSerializerOptions% :
{  
PropertyNamingPolicy $
=% &
JsonNamingPolicy' 7
.7 8
	CamelCase8 A
}   
;   
}!! 	
public## 
async## 
Task## 
<## 
Guid## 
>## 
CreateProductAsync##  2
(##2 3 
CreateProductCommand$$  
command$$! (
,$$( )
CancellationToken%% 
cancellationToken%% /
=%%0 1
default%%2 9
)%%9 :
{&& 	
var'' 
relativeUri'' 
='' 
$"''  
$str''  +
"''+ ,
;'', -
var(( 
httpRequest(( 
=(( 
new(( !
HttpRequestMessage((" 4
(((4 5

HttpMethod((5 ?
.((? @
Post((@ D
,((D E
relativeUri((F Q
)((Q R
;((R S
httpRequest)) 
.)) 
Headers)) 
.))  
Accept))  &
.))& '
Add))' *
())* +
new))+ .+
MediaTypeWithQualityHeaderValue))/ N
())N O
$str))O a
)))a b
)))b c
;))c d
var++ 
content++ 
=++ 
JsonSerializer++ (
.++( )
	Serialize++) 2
(++2 3
command++3 :
,++: ;
_serializerOptions++< N
)++N O
;++O P
httpRequest,, 
.,, 
Content,, 
=,,  !
new,," %
StringContent,,& 3
(,,3 4
content,,4 ;
,,,; <
Encoding,,= E
.,,E F
UTF8,,F J
,,,J K
$str,,L ^
),,^ _
;,,_ `
using.. 
(.. 
var.. 
response.. 
=..  !
await.." '
_httpClient..( 3
...3 4
	SendAsync..4 =
(..= >
httpRequest..> I
,..I J 
HttpCompletionOption..K _
..._ `
ResponseHeadersRead..` s
,..s t
cancellationToken	..u Ü
)
..Ü á
.
..á à
ConfigureAwait
..à ñ
(
..ñ ó
false
..ó ú
)
..ú ù
)
..ù û
{// 
if00 
(00 
!00 
response00 
.00 
IsSuccessStatusCode00 1
)001 2
{11 
throw22 
await22 &
HttpClientRequestException22  :
.22: ;
Create22; A
(22A B
_httpClient22B M
.22M N
BaseAddress22N Y
!22Y Z
,22Z [
httpRequest22\ g
,22g h
response22i q
,22q r
cancellationToken	22s Ñ
)
22Ñ Ö
.
22Ö Ü
ConfigureAwait
22Ü î
(
22î ï
false
22ï ö
)
22ö õ
;
22õ ú
}33 
using55 
(55 
var55 
contentStream55 (
=55) *
await55+ 0
response551 9
.559 :
Content55: A
.55A B
ReadAsStreamAsync55B S
(55S T
cancellationToken55T e
)55e f
.55f g
ConfigureAwait55g u
(55u v
false55v {
)55{ |
)55| }
{66 
var77 

wrappedObj77 "
=77# $
(77% &
await77& +
JsonSerializer77, :
.77: ;
DeserializeAsync77; K
<77K L
JsonResponse77L X
<77X Y
Guid77Y ]
>77] ^
>77^ _
(77_ `
contentStream77` m
,77m n
_serializerOptions	77o Å
,
77Å Ç
cancellationToken
77É î
)
77î ï
.
77ï ñ
ConfigureAwait
77ñ §
(
77§ •
false
77• ™
)
77™ ´
)
77´ ¨
!
77¨ ≠
;
77≠ Æ
return88 

wrappedObj88 %
!88% &
.88& '
Value88' ,
;88, -
}99 
}:: 
};; 	
public== 
async== 
Task== 
DeleteProductAsync== ,
(==, -
Guid==- 1
id==2 4
,==4 5
CancellationToken==6 G
cancellationToken==H Y
===Z [
default==\ c
)==c d
{>> 	
var?? 
relativeUri?? 
=?? 
$"??  
$str??  ,
{??, -
id??- /
}??/ 0
"??0 1
;??1 2
var@@ 
httpRequest@@ 
=@@ 
new@@ !
HttpRequestMessage@@" 4
(@@4 5

HttpMethod@@5 ?
.@@? @
Delete@@@ F
,@@F G
relativeUri@@H S
)@@S T
;@@T U
httpRequestAA 
.AA 
HeadersAA 
.AA  
AcceptAA  &
.AA& '
AddAA' *
(AA* +
newAA+ .+
MediaTypeWithQualityHeaderValueAA/ N
(AAN O
$strAAO a
)AAa b
)AAb c
;AAc d
usingCC 
(CC 
varCC 
responseCC 
=CC  !
awaitCC" '
_httpClientCC( 3
.CC3 4
	SendAsyncCC4 =
(CC= >
httpRequestCC> I
,CCI J 
HttpCompletionOptionCCK _
.CC_ `
ResponseHeadersReadCC` s
,CCs t
cancellationToken	CCu Ü
)
CCÜ á
.
CCá à
ConfigureAwait
CCà ñ
(
CCñ ó
false
CCó ú
)
CCú ù
)
CCù û
{DD 
ifEE 
(EE 
!EE 
responseEE 
.EE 
IsSuccessStatusCodeEE 1
)EE1 2
{FF 
throwGG 
awaitGG &
HttpClientRequestExceptionGG  :
.GG: ;
CreateGG; A
(GGA B
_httpClientGGB M
.GGM N
BaseAddressGGN Y
!GGY Z
,GGZ [
httpRequestGG\ g
,GGg h
responseGGi q
,GGq r
cancellationToken	GGs Ñ
)
GGÑ Ö
.
GGÖ Ü
ConfigureAwait
GGÜ î
(
GGî ï
false
GGï ö
)
GGö õ
;
GGõ ú
}HH 
}II 
}JJ 	
publicLL 
asyncLL 
TaskLL 
UpdateProductAsyncLL ,
(LL, -
GuidMM 
idMM 
,MM  
UpdateProductCommandNN  
commandNN! (
,NN( )
CancellationTokenOO 
cancellationTokenOO /
=OO0 1
defaultOO2 9
)OO9 :
{PP 	
varQQ 
relativeUriQQ 
=QQ 
$"QQ  
$strQQ  ,
{QQ, -
idQQ- /
}QQ/ 0
"QQ0 1
;QQ1 2
varRR 
httpRequestRR 
=RR 
newRR !
HttpRequestMessageRR" 4
(RR4 5

HttpMethodRR5 ?
.RR? @
PutRR@ C
,RRC D
relativeUriRRE P
)RRP Q
;RRQ R
httpRequestSS 
.SS 
HeadersSS 
.SS  
AcceptSS  &
.SS& '
AddSS' *
(SS* +
newSS+ .+
MediaTypeWithQualityHeaderValueSS/ N
(SSN O
$strSSO a
)SSa b
)SSb c
;SSc d
varUU 
contentUU 
=UU 
JsonSerializerUU (
.UU( )
	SerializeUU) 2
(UU2 3
commandUU3 :
,UU: ;
_serializerOptionsUU< N
)UUN O
;UUO P
httpRequestVV 
.VV 
ContentVV 
=VV  !
newVV" %
StringContentVV& 3
(VV3 4
contentVV4 ;
,VV; <
EncodingVV= E
.VVE F
UTF8VVF J
,VVJ K
$strVVL ^
)VV^ _
;VV_ `
usingXX 
(XX 
varXX 
responseXX 
=XX  !
awaitXX" '
_httpClientXX( 3
.XX3 4
	SendAsyncXX4 =
(XX= >
httpRequestXX> I
,XXI J 
HttpCompletionOptionXXK _
.XX_ `
ResponseHeadersReadXX` s
,XXs t
cancellationToken	XXu Ü
)
XXÜ á
.
XXá à
ConfigureAwait
XXà ñ
(
XXñ ó
false
XXó ú
)
XXú ù
)
XXù û
{YY 
ifZZ 
(ZZ 
!ZZ 
responseZZ 
.ZZ 
IsSuccessStatusCodeZZ 1
)ZZ1 2
{[[ 
throw\\ 
await\\ &
HttpClientRequestException\\  :
.\\: ;
Create\\; A
(\\A B
_httpClient\\B M
.\\M N
BaseAddress\\N Y
!\\Y Z
,\\Z [
httpRequest\\\ g
,\\g h
response\\i q
,\\q r
cancellationToken	\\s Ñ
)
\\Ñ Ö
.
\\Ö Ü
ConfigureAwait
\\Ü î
(
\\î ï
false
\\ï ö
)
\\ö õ
;
\\õ ú
}]] 
}^^ 
}__ 	
publicaa 
asyncaa 
Taskaa 
<aa 

ProductDtoaa $
>aa$ %
GetProductByIdAsyncaa& 9
(aa9 :
Guidaa: >
idaa? A
,aaA B
CancellationTokenaaC T
cancellationTokenaaU f
=aag h
defaultaai p
)aap q
{bb 	
varcc 
relativeUricc 
=cc 
$"cc  
$strcc  ,
{cc, -
idcc- /
}cc/ 0
"cc0 1
;cc1 2
vardd 
httpRequestdd 
=dd 
newdd !
HttpRequestMessagedd" 4
(dd4 5

HttpMethoddd5 ?
.dd? @
Getdd@ C
,ddC D
relativeUriddE P
)ddP Q
;ddQ R
httpRequestee 
.ee 
Headersee 
.ee  
Acceptee  &
.ee& '
Addee' *
(ee* +
newee+ .+
MediaTypeWithQualityHeaderValueee/ N
(eeN O
$streeO a
)eea b
)eeb c
;eec d
usinggg 
(gg 
vargg 
responsegg 
=gg  !
awaitgg" '
_httpClientgg( 3
.gg3 4
	SendAsyncgg4 =
(gg= >
httpRequestgg> I
,ggI J 
HttpCompletionOptionggK _
.gg_ `
ResponseHeadersReadgg` s
,ggs t
cancellationToken	ggu Ü
)
ggÜ á
.
ggá à
ConfigureAwait
ggà ñ
(
ggñ ó
false
ggó ú
)
ggú ù
)
ggù û
{hh 
ifii 
(ii 
!ii 
responseii 
.ii 
IsSuccessStatusCodeii 1
)ii1 2
{jj 
throwkk 
awaitkk &
HttpClientRequestExceptionkk  :
.kk: ;
Createkk; A
(kkA B
_httpClientkkB M
.kkM N
BaseAddresskkN Y
!kkY Z
,kkZ [
httpRequestkk\ g
,kkg h
responsekki q
,kkq r
cancellationToken	kks Ñ
)
kkÑ Ö
.
kkÖ Ü
ConfigureAwait
kkÜ î
(
kkî ï
false
kkï ö
)
kkö õ
;
kkõ ú
}ll 
usingnn 
(nn 
varnn 
contentStreamnn (
=nn) *
awaitnn+ 0
responsenn1 9
.nn9 :
Contentnn: A
.nnA B
ReadAsStreamAsyncnnB S
(nnS T
cancellationTokennnT e
)nne f
.nnf g
ConfigureAwaitnng u
(nnu v
falsennv {
)nn{ |
)nn| }
{oo 
returnpp 
(pp 
awaitpp !
JsonSerializerpp" 0
.pp0 1
DeserializeAsyncpp1 A
<ppA B

ProductDtoppB L
>ppL M
(ppM N
contentStreamppN [
,pp[ \
_serializerOptionspp] o
,ppo p
cancellationToken	ppq Ç
)
ppÇ É
.
ppÉ Ñ
ConfigureAwait
ppÑ í
(
ppí ì
false
ppì ò
)
ppò ô
)
ppô ö
!
ppö õ
;
ppõ ú
}qq 
}rr 
}ss 	
publicuu 
asyncuu 
Taskuu 
<uu 
Listuu 
<uu 

ProductDtouu )
>uu) *
>uu* +
GetProductsAsyncuu, <
(uu< =
CancellationTokenuu= N
cancellationTokenuuO `
=uua b
defaultuuc j
)uuj k
{vv 	
varww 
relativeUriww 
=ww 
$"ww  
$strww  +
"ww+ ,
;ww, -
varxx 
httpRequestxx 
=xx 
newxx !
HttpRequestMessagexx" 4
(xx4 5

HttpMethodxx5 ?
.xx? @
Getxx@ C
,xxC D
relativeUrixxE P
)xxP Q
;xxQ R
httpRequestyy 
.yy 
Headersyy 
.yy  
Acceptyy  &
.yy& '
Addyy' *
(yy* +
newyy+ .+
MediaTypeWithQualityHeaderValueyy/ N
(yyN O
$stryyO a
)yya b
)yyb c
;yyc d
using{{ 
({{ 
var{{ 
response{{ 
={{  !
await{{" '
_httpClient{{( 3
.{{3 4
	SendAsync{{4 =
({{= >
httpRequest{{> I
,{{I J 
HttpCompletionOption{{K _
.{{_ `
ResponseHeadersRead{{` s
,{{s t
cancellationToken	{{u Ü
)
{{Ü á
.
{{á à
ConfigureAwait
{{à ñ
(
{{ñ ó
false
{{ó ú
)
{{ú ù
)
{{ù û
{|| 
if}} 
(}} 
!}} 
response}} 
.}} 
IsSuccessStatusCode}} 1
)}}1 2
{~~ 
throw 
await &
HttpClientRequestException  :
.: ;
Create; A
(A B
_httpClientB M
.M N
BaseAddressN Y
!Y Z
,Z [
httpRequest\ g
,g h
responsei q
,q r
cancellationToken	s Ñ
)
Ñ Ö
.
Ö Ü
ConfigureAwait
Ü î
(
î ï
false
ï ö
)
ö õ
;
õ ú
}
ÄÄ 
using
ÇÇ 
(
ÇÇ 
var
ÇÇ 
contentStream
ÇÇ (
=
ÇÇ) *
await
ÇÇ+ 0
response
ÇÇ1 9
.
ÇÇ9 :
Content
ÇÇ: A
.
ÇÇA B
ReadAsStreamAsync
ÇÇB S
(
ÇÇS T
cancellationToken
ÇÇT e
)
ÇÇe f
.
ÇÇf g
ConfigureAwait
ÇÇg u
(
ÇÇu v
false
ÇÇv {
)
ÇÇ{ |
)
ÇÇ| }
{
ÉÉ 
return
ÑÑ 
(
ÑÑ 
await
ÑÑ !
JsonSerializer
ÑÑ" 0
.
ÑÑ0 1
DeserializeAsync
ÑÑ1 A
<
ÑÑA B
List
ÑÑB F
<
ÑÑF G

ProductDto
ÑÑG Q
>
ÑÑQ R
>
ÑÑR S
(
ÑÑS T
contentStream
ÑÑT a
,
ÑÑa b 
_serializerOptions
ÑÑc u
,
ÑÑu v 
cancellationTokenÑÑw à
)ÑÑà â
.ÑÑâ ä
ConfigureAwaitÑÑä ò
(ÑÑò ô
falseÑÑô û
)ÑÑû ü
)ÑÑü †
!ÑÑ† °
;ÑÑ° ¢
}
ÖÖ 
}
ÜÜ 
}
áá 	
public
ââ 
void
ââ 
Dispose
ââ 
(
ââ 
)
ââ 
{
ää 	
Dispose
ãã 
(
ãã 
true
ãã 
)
ãã 
;
ãã 
GC
åå 
.
åå 
SuppressFinalize
åå 
(
åå  
this
åå  $
)
åå$ %
;
åå% &
}
çç 	
	protected
èè 
virtual
èè 
void
èè 
Dispose
èè &
(
èè& '
bool
èè' +
	disposing
èè, 5
)
èè5 6
{
êê 	
}
íí 	
}
ìì 
}îî Ä

òD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\HttpClients\JsonResponse.cs
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
.0 1
Infrastructure1 ?
.? @
HttpClients@ K
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
} ∞X
®D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\HttpClients\FileUploadsServiceHttpClient.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
HttpClients@ K
{ 
public 

class (
FileUploadsServiceHttpClient -
:. /
IFileUploadsService0 C
{ 
private 
readonly !
JsonSerializerOptions .
_serializerOptions/ A
;A B
private 
readonly 

HttpClient #
_httpClient$ /
;/ 0
public (
FileUploadsServiceHttpClient +
(+ ,

HttpClient, 6

httpClient7 A
)A B
{ 	
_httpClient 
= 

httpClient $
;$ %
_serializerOptions 
=  
new! $!
JsonSerializerOptions% :
{  
PropertyNamingPolicy   $
=  % &
JsonNamingPolicy  ' 7
.  7 8
	CamelCase  8 A
}!! 
;!! 
}"" 	
public$$ 
async$$ 
Task$$ 
<$$ 
Guid$$ 
>$$ 
UploadFileAsync$$  /
($$/ 0
string%% 
?%% 
contentType%% 
,%%  
long&& 
?&& 
contentLength&& 
,&&  
UploadFileCommand'' 
command'' %
,''% &
CancellationToken(( 
cancellationToken(( /
=((0 1
default((2 9
)((9 :
{)) 	
var** 
relativeUri** 
=** 
$"**  
$str**  <
"**< =
;**= >
var++ 
httpRequest++ 
=++ 
new++ !
HttpRequestMessage++" 4
(++4 5

HttpMethod++5 ?
.++? @
Post++@ D
,++D E
relativeUri++F Q
)++Q R
;++R S
httpRequest,, 
.,, 
Headers,, 
.,,  
Accept,,  &
.,,& '
Add,,' *
(,,* +
new,,+ .+
MediaTypeWithQualityHeaderValue,,/ N
(,,N O
$str,,O a
),,a b
),,b c
;,,c d
if.. 
(.. 
contentLength.. 
!=..  
null..! %
)..% &
{// 
httpRequest00 
.00 
Headers00 #
.00# $
Add00$ '
(00' (
$str00( 8
,008 9
contentLength00: G
.00G H
ToString00H P
(00P Q
)00Q R
)00R S
;00S T
}11 
httpRequest22 
.22 
Content22 
=22  !
new22" %
StreamContent22& 3
(223 4
command224 ;
.22; <
Content22< C
)22C D
;22D E
httpRequest33 
.33 
Content33 
.33  
Headers33  '
.33' (
ContentType33( 3
=334 5
new336 9
System33: @
.33@ A
Net33A D
.33D E
Http33E I
.33I J
Headers33J Q
.33Q R 
MediaTypeHeaderValue33R f
(33f g
command33g n
.33n o
ContentType33o z
??33{ }
$str	33~ ò
)
33ò ô
;
33ô ö
if55 
(55 
command55 
.55 
Filename55  
!=55! #
null55$ (
)55( )
{66 
httpRequest77 
.77 
Content77 #
.77# $
Headers77$ +
.77+ ,
ContentDisposition77, >
=77? @
new77A D)
ContentDispositionHeaderValue77E b
(77b c
$str77c n
)77n o
{77p q
FileName77r z
=77{ |
command	77} Ñ
.
77Ñ Ö
Filename
77Ö ç
}
77é è
;
77è ê
}88 
using:: 
(:: 
var:: 
response:: 
=::  !
await::" '
_httpClient::( 3
.::3 4
	SendAsync::4 =
(::= >
httpRequest::> I
,::I J 
HttpCompletionOption::K _
.::_ `
ResponseHeadersRead::` s
,::s t
cancellationToken	::u Ü
)
::Ü á
.
::á à
ConfigureAwait
::à ñ
(
::ñ ó
false
::ó ú
)
::ú ù
)
::ù û
{;; 
if<< 
(<< 
!<< 
response<< 
.<< 
IsSuccessStatusCode<< 1
)<<1 2
{== 
throw>> 
await>> &
HttpClientRequestException>>  :
.>>: ;
Create>>; A
(>>A B
_httpClient>>B M
.>>M N
BaseAddress>>N Y
!>>Y Z
,>>Z [
httpRequest>>\ g
,>>g h
response>>i q
,>>q r
cancellationToken	>>s Ñ
)
>>Ñ Ö
.
>>Ö Ü
ConfigureAwait
>>Ü î
(
>>î ï
false
>>ï ö
)
>>ö õ
;
>>õ ú
}?? 
usingAA 
(AA 
varAA 
contentStreamAA (
=AA) *
awaitAA+ 0
responseAA1 9
.AA9 :
ContentAA: A
.AAA B
ReadAsStreamAsyncAAB S
(AAS T
cancellationTokenAAT e
)AAe f
.AAf g
ConfigureAwaitAAg u
(AAu v
falseAAv {
)AA{ |
)AA| }
{BB 
varCC 

wrappedObjCC "
=CC# $
(CC% &
awaitCC& +
JsonSerializerCC, :
.CC: ;
DeserializeAsyncCC; K
<CCK L
JsonResponseCCL X
<CCX Y
GuidCCY ]
>CC] ^
>CC^ _
(CC_ `
contentStreamCC` m
,CCm n
_serializerOptions	CCo Å
,
CCÅ Ç
cancellationToken
CCÉ î
)
CCî ï
.
CCï ñ
ConfigureAwait
CCñ §
(
CC§ •
false
CC• ™
)
CC™ ´
)
CC´ ¨
!
CC¨ ≠
;
CC≠ Æ
returnDD 

wrappedObjDD %
!DD% &
.DD& '
ValueDD' ,
;DD, -
}EE 
}FF 
}GG 	
publicII 
asyncII 
TaskII 
<II 
FileDownloadDtoII )
>II) *
DownloadFileAsyncII+ <
(II< =
GuidII= A
idIIB D
,IID E
CancellationTokenIIF W
cancellationTokenIIX i
=IIj k
defaultIIl s
)IIs t
{JJ 	
varKK 
relativeUriKK 
=KK 
$"KK  
$strKK  0
{KK0 1
idKK1 3
}KK3 4
"KK4 5
;KK5 6
varLL 
httpRequestLL 
=LL 
newLL !
HttpRequestMessageLL" 4
(LL4 5

HttpMethodLL5 ?
.LL? @
GetLL@ C
,LLC D
relativeUriLLE P
)LLP Q
;LLQ R
httpRequestMM 
.MM 
HeadersMM 
.MM  
AcceptMM  &
.MM& '
AddMM' *
(MM* +
newMM+ .+
MediaTypeWithQualityHeaderValueMM/ N
(MMN O
$strMMO a
)MMa b
)MMb c
;MMc d
usingOO 
(OO 
varOO 
responseOO 
=OO  !
awaitOO" '
_httpClientOO( 3
.OO3 4
	SendAsyncOO4 =
(OO= >
httpRequestOO> I
,OOI J 
HttpCompletionOptionOOK _
.OO_ `
ResponseHeadersReadOO` s
,OOs t
cancellationToken	OOu Ü
)
OOÜ á
.
OOá à
ConfigureAwait
OOà ñ
(
OOñ ó
false
OOó ú
)
OOú ù
)
OOù û
{PP 
ifQQ 
(QQ 
!QQ 
responseQQ 
.QQ 
IsSuccessStatusCodeQQ 1
)QQ1 2
{RR 
throwSS 
awaitSS &
HttpClientRequestExceptionSS  :
.SS: ;
CreateSS; A
(SSA B
_httpClientSSB M
.SSM N
BaseAddressSSN Y
!SSY Z
,SSZ [
httpRequestSS\ g
,SSg h
responseSSi q
,SSq r
cancellationToken	SSs Ñ
)
SSÑ Ö
.
SSÖ Ü
ConfigureAwait
SSÜ î
(
SSî ï
false
SSï ö
)
SSö õ
;
SSõ ú
}TT 
varVV 
memoryStreamVV  
=VV! "
newVV# &
MemoryStreamVV' 3
(VV3 4
)VV4 5
;VV5 6
varWW 
responseStreamWW "
=WW# $
awaitWW% *
responseWW+ 3
.WW3 4
ContentWW4 ;
.WW; <
ReadAsStreamAsyncWW< M
(WWM N
cancellationTokenWWN _
)WW_ `
;WW` a
awaitXX 
responseStreamXX $
.XX$ %
CopyToAsyncXX% 0
(XX0 1
memoryStreamXX1 =
,XX= >
cancellationTokenXX? P
)XXP Q
;XXQ R
memoryStreamYY 
.YY 
SeekYY !
(YY! "
$numYY" #
,YY# $

SeekOriginYY% /
.YY/ 0
BeginYY0 5
)YY5 6
;YY6 7
return[[ 
FileDownloadDto[[ &
.[[& '
Create[[' -
([[- .
memoryStream[[. :
,[[: ;
filename[[< D
:[[D E
response[[F N
.[[N O
Content[[O V
.[[V W
Headers[[W ^
.[[^ _
ContentDisposition[[_ q
?[[q r
.[[r s
FileName[[s {
,[[{ |
contentType	[[} à
:
[[à â
response
[[ä í
.
[[í ì
Content
[[ì ö
.
[[ö õ
Headers
[[õ ¢
.
[[¢ £
ContentType
[[£ Æ
?
[[Æ Ø
.
[[Ø ∞
	MediaType
[[∞ π
??
[[∫ º
$str
[[Ω ø
)
[[ø ¿
;
[[¿ ¡
}\\ 
}]] 	
public__ 
void__ 
Dispose__ 
(__ 
)__ 
{`` 	
Disposeaa 
(aa 
trueaa 
)aa 
;aa 
GCbb 
.bb 
SuppressFinalizebb 
(bb  
thisbb  $
)bb$ %
;bb% &
}cc 	
	protectedee 
virtualee 
voidee 
Disposeee &
(ee& '
boolee' +
	disposingee, 5
)ee5 6
{ff 	
}hh 	
}ii 
}jj ãâ
´D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\HttpClients\CustomersServiceProxyHttpClient.cs
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
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
,* +
Targets, 3
=4 5
Targets6 =
.= >
Usings> D
)D E
]E F
[ 
assembly 	
:	 

IntentTemplate 
( 
$str E
,E F
VersionG N
=O P
$strQ V
)V W
]W X
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
HttpClients@ K
{ 
public 

class +
CustomersServiceProxyHttpClient 0
:1 2"
ICustomersServiceProxy3 I
{ 
private 
readonly !
JsonSerializerOptions .
_serializerOptions/ A
;A B
private 
readonly 

HttpClient #
_httpClient$ /
;/ 0
public +
CustomersServiceProxyHttpClient .
(. /

HttpClient/ 9

httpClient: D
)D E
{ 	
_httpClient 
= 

httpClient $
;$ %
_serializerOptions 
=  
new! $!
JsonSerializerOptions% :
{  
PropertyNamingPolicy $
=% &
JsonNamingPolicy' 7
.7 8
	CamelCase8 A
}   
;   
}!! 	
public## 
async## 
Task## 
<## 
Guid## 
>## 
CreateCustomerAsync##  3
(##3 4!
CreateCustomerCommand$$ !
command$$" )
,$$) *
CancellationToken%% 
cancellationToken%% /
=%%0 1
default%%2 9
)%%9 :
{&& 	
var'' 
relativeUri'' 
='' 
$"''  
$str''  ,
"'', -
;''- .
var(( 
httpRequest(( 
=(( 
new(( !
HttpRequestMessage((" 4
(((4 5

HttpMethod((5 ?
.((? @
Post((@ D
,((D E
relativeUri((F Q
)((Q R
;((R S
httpRequest)) 
.)) 
Headers)) 
.))  
Accept))  &
.))& '
Add))' *
())* +
new))+ .+
MediaTypeWithQualityHeaderValue))/ N
())N O
$str))O a
)))a b
)))b c
;))c d
var++ 
content++ 
=++ 
JsonSerializer++ (
.++( )
	Serialize++) 2
(++2 3
command++3 :
,++: ;
_serializerOptions++< N
)++N O
;++O P
httpRequest,, 
.,, 
Content,, 
=,,  !
new,," %
StringContent,,& 3
(,,3 4
content,,4 ;
,,,; <
Encoding,,= E
.,,E F
UTF8,,F J
,,,J K
$str,,L ^
),,^ _
;,,_ `
using.. 
(.. 
var.. 
response.. 
=..  !
await.." '
_httpClient..( 3
...3 4
	SendAsync..4 =
(..= >
httpRequest..> I
,..I J 
HttpCompletionOption..K _
..._ `
ResponseHeadersRead..` s
,..s t
cancellationToken	..u Ü
)
..Ü á
.
..á à
ConfigureAwait
..à ñ
(
..ñ ó
false
..ó ú
)
..ú ù
)
..ù û
{// 
if00 
(00 
!00 
response00 
.00 
IsSuccessStatusCode00 1
)001 2
{11 
throw22 
await22 &
HttpClientRequestException22  :
.22: ;
Create22; A
(22A B
_httpClient22B M
.22M N
BaseAddress22N Y
!22Y Z
,22Z [
httpRequest22\ g
,22g h
response22i q
,22q r
cancellationToken	22s Ñ
)
22Ñ Ö
.
22Ö Ü
ConfigureAwait
22Ü î
(
22î ï
false
22ï ö
)
22ö õ
;
22õ ú
}33 
using55 
(55 
var55 
contentStream55 (
=55) *
await55+ 0
response551 9
.559 :
Content55: A
.55A B
ReadAsStreamAsync55B S
(55S T
cancellationToken55T e
)55e f
.55f g
ConfigureAwait55g u
(55u v
false55v {
)55{ |
)55| }
{66 
var77 

wrappedObj77 "
=77# $
(77% &
await77& +
JsonSerializer77, :
.77: ;
DeserializeAsync77; K
<77K L
JsonResponse77L X
<77X Y
Guid77Y ]
>77] ^
>77^ _
(77_ `
contentStream77` m
,77m n
_serializerOptions	77o Å
,
77Å Ç
cancellationToken
77É î
)
77î ï
.
77ï ñ
ConfigureAwait
77ñ §
(
77§ •
false
77• ™
)
77™ ´
)
77´ ¨
!
77¨ ≠
;
77≠ Æ
return88 

wrappedObj88 %
!88% &
.88& '
Value88' ,
;88, -
}99 
}:: 
};; 	
public== 
async== 
Task== 
DeleteCustomerAsync== -
(==- .
Guid==. 2
id==3 5
,==5 6
CancellationToken==7 H
cancellationToken==I Z
===[ \
default==] d
)==d e
{>> 	
var?? 
relativeUri?? 
=?? 
$"??  
$str??  -
{??- .
id??. 0
}??0 1
"??1 2
;??2 3
var@@ 
httpRequest@@ 
=@@ 
new@@ !
HttpRequestMessage@@" 4
(@@4 5

HttpMethod@@5 ?
.@@? @
Delete@@@ F
,@@F G
relativeUri@@H S
)@@S T
;@@T U
httpRequestAA 
.AA 
HeadersAA 
.AA  
AcceptAA  &
.AA& '
AddAA' *
(AA* +
newAA+ .+
MediaTypeWithQualityHeaderValueAA/ N
(AAN O
$strAAO a
)AAa b
)AAb c
;AAc d
usingCC 
(CC 
varCC 
responseCC 
=CC  !
awaitCC" '
_httpClientCC( 3
.CC3 4
	SendAsyncCC4 =
(CC= >
httpRequestCC> I
,CCI J 
HttpCompletionOptionCCK _
.CC_ `
ResponseHeadersReadCC` s
,CCs t
cancellationToken	CCu Ü
)
CCÜ á
.
CCá à
ConfigureAwait
CCà ñ
(
CCñ ó
false
CCó ú
)
CCú ù
)
CCù û
{DD 
ifEE 
(EE 
!EE 
responseEE 
.EE 
IsSuccessStatusCodeEE 1
)EE1 2
{FF 
throwGG 
awaitGG &
HttpClientRequestExceptionGG  :
.GG: ;
CreateGG; A
(GGA B
_httpClientGGB M
.GGM N
BaseAddressGGN Y
!GGY Z
,GGZ [
httpRequestGG\ g
,GGg h
responseGGi q
,GGq r
cancellationToken	GGs Ñ
)
GGÑ Ö
.
GGÖ Ü
ConfigureAwait
GGÜ î
(
GGî ï
false
GGï ö
)
GGö õ
;
GGõ ú
}HH 
}II 
}JJ 	
publicLL 
asyncLL 
TaskLL 
UpdateCustomerAsyncLL -
(LL- .
GuidMM 
idMM 
,MM !
UpdateCustomerCommandNN !
commandNN" )
,NN) *
CancellationTokenOO 
cancellationTokenOO /
=OO0 1
defaultOO2 9
)OO9 :
{PP 	
varQQ 
relativeUriQQ 
=QQ 
$"QQ  
$strQQ  -
{QQ- .
idQQ. 0
}QQ0 1
"QQ1 2
;QQ2 3
varRR 
httpRequestRR 
=RR 
newRR !
HttpRequestMessageRR" 4
(RR4 5

HttpMethodRR5 ?
.RR? @
PutRR@ C
,RRC D
relativeUriRRE P
)RRP Q
;RRQ R
httpRequestSS 
.SS 
HeadersSS 
.SS  
AcceptSS  &
.SS& '
AddSS' *
(SS* +
newSS+ .+
MediaTypeWithQualityHeaderValueSS/ N
(SSN O
$strSSO a
)SSa b
)SSb c
;SSc d
varUU 
contentUU 
=UU 
JsonSerializerUU (
.UU( )
	SerializeUU) 2
(UU2 3
commandUU3 :
,UU: ;
_serializerOptionsUU< N
)UUN O
;UUO P
httpRequestVV 
.VV 
ContentVV 
=VV  !
newVV" %
StringContentVV& 3
(VV3 4
contentVV4 ;
,VV; <
EncodingVV= E
.VVE F
UTF8VVF J
,VVJ K
$strVVL ^
)VV^ _
;VV_ `
usingXX 
(XX 
varXX 
responseXX 
=XX  !
awaitXX" '
_httpClientXX( 3
.XX3 4
	SendAsyncXX4 =
(XX= >
httpRequestXX> I
,XXI J 
HttpCompletionOptionXXK _
.XX_ `
ResponseHeadersReadXX` s
,XXs t
cancellationToken	XXu Ü
)
XXÜ á
.
XXá à
ConfigureAwait
XXà ñ
(
XXñ ó
false
XXó ú
)
XXú ù
)
XXù û
{YY 
ifZZ 
(ZZ 
!ZZ 
responseZZ 
.ZZ 
IsSuccessStatusCodeZZ 1
)ZZ1 2
{[[ 
throw\\ 
await\\ &
HttpClientRequestException\\  :
.\\: ;
Create\\; A
(\\A B
_httpClient\\B M
.\\M N
BaseAddress\\N Y
!\\Y Z
,\\Z [
httpRequest\\\ g
,\\g h
response\\i q
,\\q r
cancellationToken	\\s Ñ
)
\\Ñ Ö
.
\\Ö Ü
ConfigureAwait
\\Ü î
(
\\î ï
false
\\ï ö
)
\\ö õ
;
\\õ ú
}]] 
}^^ 
}__ 	
publicaa 
asyncaa 
Taskaa 
<aa 
CustomerDtoaa %
>aa% & 
GetCustomerByIdAsyncaa' ;
(aa; <
Guidaa< @
idaaA C
,aaC D
CancellationTokenaaE V
cancellationTokenaaW h
=aai j
defaultaak r
)aar s
{bb 	
varcc 
relativeUricc 
=cc 
$"cc  
$strcc  -
{cc- .
idcc. 0
}cc0 1
"cc1 2
;cc2 3
vardd 
httpRequestdd 
=dd 
newdd !
HttpRequestMessagedd" 4
(dd4 5

HttpMethoddd5 ?
.dd? @
Getdd@ C
,ddC D
relativeUriddE P
)ddP Q
;ddQ R
httpRequestee 
.ee 
Headersee 
.ee  
Acceptee  &
.ee& '
Addee' *
(ee* +
newee+ .+
MediaTypeWithQualityHeaderValueee/ N
(eeN O
$streeO a
)eea b
)eeb c
;eec d
usinggg 
(gg 
vargg 
responsegg 
=gg  !
awaitgg" '
_httpClientgg( 3
.gg3 4
	SendAsyncgg4 =
(gg= >
httpRequestgg> I
,ggI J 
HttpCompletionOptionggK _
.gg_ `
ResponseHeadersReadgg` s
,ggs t
cancellationToken	ggu Ü
)
ggÜ á
.
ggá à
ConfigureAwait
ggà ñ
(
ggñ ó
false
ggó ú
)
ggú ù
)
ggù û
{hh 
ifii 
(ii 
!ii 
responseii 
.ii 
IsSuccessStatusCodeii 1
)ii1 2
{jj 
throwkk 
awaitkk &
HttpClientRequestExceptionkk  :
.kk: ;
Createkk; A
(kkA B
_httpClientkkB M
.kkM N
BaseAddresskkN Y
!kkY Z
,kkZ [
httpRequestkk\ g
,kkg h
responsekki q
,kkq r
cancellationToken	kks Ñ
)
kkÑ Ö
.
kkÖ Ü
ConfigureAwait
kkÜ î
(
kkî ï
false
kkï ö
)
kkö õ
;
kkõ ú
}ll 
usingnn 
(nn 
varnn 
contentStreamnn (
=nn) *
awaitnn+ 0
responsenn1 9
.nn9 :
Contentnn: A
.nnA B
ReadAsStreamAsyncnnB S
(nnS T
cancellationTokennnT e
)nne f
.nnf g
ConfigureAwaitnng u
(nnu v
falsennv {
)nn{ |
)nn| }
{oo 
returnpp 
(pp 
awaitpp !
JsonSerializerpp" 0
.pp0 1
DeserializeAsyncpp1 A
<ppA B
CustomerDtoppB M
>ppM N
(ppN O
contentStreamppO \
,pp\ ]
_serializerOptionspp^ p
,ppp q
cancellationToken	ppr É
)
ppÉ Ñ
.
ppÑ Ö
ConfigureAwait
ppÖ ì
(
ppì î
false
ppî ô
)
ppô ö
)
ppö õ
!
ppõ ú
;
ppú ù
}qq 
}rr 
}ss 	
publicuu 
asyncuu 
Taskuu 
<uu 
Listuu 
<uu 
CustomerDtouu *
>uu* +
>uu+ ,
GetCustomersAsyncuu- >
(uu> ?
CancellationTokenuu? P
cancellationTokenuuQ b
=uuc d
defaultuue l
)uul m
{vv 	
varww 
relativeUriww 
=ww 
$"ww  
$strww  ,
"ww, -
;ww- .
varxx 
httpRequestxx 
=xx 
newxx !
HttpRequestMessagexx" 4
(xx4 5

HttpMethodxx5 ?
.xx? @
Getxx@ C
,xxC D
relativeUrixxE P
)xxP Q
;xxQ R
httpRequestyy 
.yy 
Headersyy 
.yy  
Acceptyy  &
.yy& '
Addyy' *
(yy* +
newyy+ .+
MediaTypeWithQualityHeaderValueyy/ N
(yyN O
$stryyO a
)yya b
)yyb c
;yyc d
using{{ 
({{ 
var{{ 
response{{ 
={{  !
await{{" '
_httpClient{{( 3
.{{3 4
	SendAsync{{4 =
({{= >
httpRequest{{> I
,{{I J 
HttpCompletionOption{{K _
.{{_ `
ResponseHeadersRead{{` s
,{{s t
cancellationToken	{{u Ü
)
{{Ü á
.
{{á à
ConfigureAwait
{{à ñ
(
{{ñ ó
false
{{ó ú
)
{{ú ù
)
{{ù û
{|| 
if}} 
(}} 
!}} 
response}} 
.}} 
IsSuccessStatusCode}} 1
)}}1 2
{~~ 
throw 
await &
HttpClientRequestException  :
.: ;
Create; A
(A B
_httpClientB M
.M N
BaseAddressN Y
!Y Z
,Z [
httpRequest\ g
,g h
responsei q
,q r
cancellationToken	s Ñ
)
Ñ Ö
.
Ö Ü
ConfigureAwait
Ü î
(
î ï
false
ï ö
)
ö õ
;
õ ú
}
ÄÄ 
using
ÇÇ 
(
ÇÇ 
var
ÇÇ 
contentStream
ÇÇ (
=
ÇÇ) *
await
ÇÇ+ 0
response
ÇÇ1 9
.
ÇÇ9 :
Content
ÇÇ: A
.
ÇÇA B
ReadAsStreamAsync
ÇÇB S
(
ÇÇS T
cancellationToken
ÇÇT e
)
ÇÇe f
.
ÇÇf g
ConfigureAwait
ÇÇg u
(
ÇÇu v
false
ÇÇv {
)
ÇÇ{ |
)
ÇÇ| }
{
ÉÉ 
return
ÑÑ 
(
ÑÑ 
await
ÑÑ !
JsonSerializer
ÑÑ" 0
.
ÑÑ0 1
DeserializeAsync
ÑÑ1 A
<
ÑÑA B
List
ÑÑB F
<
ÑÑF G
CustomerDto
ÑÑG R
>
ÑÑR S
>
ÑÑS T
(
ÑÑT U
contentStream
ÑÑU b
,
ÑÑb c 
_serializerOptions
ÑÑd v
,
ÑÑv w 
cancellationTokenÑÑx â
)ÑÑâ ä
.ÑÑä ã
ConfigureAwaitÑÑã ô
(ÑÑô ö
falseÑÑö ü
)ÑÑü †
)ÑÑ† °
!ÑÑ° ¢
;ÑÑ¢ £
}
ÖÖ 
}
ÜÜ 
}
áá 	
public
ââ 
void
ââ 
Dispose
ââ 
(
ââ 
)
ââ 
{
ää 	
Dispose
ãã 
(
ãã 
true
ãã 
)
ãã 
;
ãã 
GC
åå 
.
åå 
SuppressFinalize
åå 
(
åå  
this
åå  $
)
åå$ %
;
åå% &
}
çç 	
	protected
èè 
virtual
èè 
void
èè 
Dispose
èè &
(
èè& '
bool
èè' +
	disposing
èè, 5
)
èè5 6
{
êê 	
}
íí 	
}
ìì 
}îî ƒQ
úD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Eventing\MassTransitEventBus.cs
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
$str K
,K L
VersionM T
=U V
$strW \
)\ ]
]] ^
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Eventing@ H
{ 
public 

class 
MassTransitEventBus $
:% &
	IEventBus' 0
{ 
private 
readonly 
List 
< 
object $
>$ %
_messagesToPublish& 8
=9 :
[; <
]< =
;= >
private 
readonly 
List 
< 
MessageToSend +
>+ ,
_messagesToSend- <
== >
[? @
]@ A
;A B
private 
readonly 
IServiceProvider )
_serviceProvider* :
;: ;
public 
MassTransitEventBus "
(" #
IServiceProvider# 3
serviceProvider4 C
)C D
{ 	
_serviceProvider 
= 
serviceProvider .
;. /
} 	
public 
ConsumeContext 
? 
ConsumeContext -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
public 
void 
Publish 
< 
T 
> 
( 
T  
message! (
)( )
where 
T 
: 
class 
{ 	
_messagesToPublish 
. 
Add "
(" #
message# *
)* +
;+ ,
}   	
public"" 
void"" 
Send"" 
<"" 
T"" 
>"" 
("" 
T"" 
message"" %
)""% &
where## 
T## 
:## 
class## 
{$$ 	
_messagesToSend%% 
.%% 
Add%% 
(%%  
new%%  #
MessageToSend%%$ 1
(%%1 2
message%%2 9
,%%9 :
null%%; ?
)%%? @
)%%@ A
;%%A B
}&& 	
public(( 
void(( 
Send(( 
<(( 
T(( 
>(( 
((( 
T(( 
message(( %
,((% &
Uri((' *
address((+ 2
)((2 3
where)) 
T)) 
:)) 
class)) 
{** 	
_messagesToSend++ 
.++ 
Add++ 
(++  
new++  #
MessageToSend++$ 1
(++1 2
message++2 9
,++9 :
address++; B
)++B C
)++C D
;++D E
},, 	
public.. 
async.. 
Task.. 
FlushAllAsync.. '
(..' (
CancellationToken..( 9
cancellationToken..: K
=..L M
default..N U
)..U V
{// 	
foreach00 
(00 
var00 
toSend00 
in00  "
_messagesToSend00# 2
)002 3
{11 
if22 
(22 
ConsumeContext22 "
is22# %
not22& )
null22* .
)22. /
{33 
await44 "
SendWithConsumeContext44 0
(440 1
toSend441 7
,447 8
cancellationToken449 J
)44J K
;44K L
}55 
else66 
{77 
await88 !
SendWithNormalContext88 /
(88/ 0
toSend880 6
,886 7
cancellationToken888 I
)88I J
;88J K
}99 
}:: 
_messagesToSend<< 
.<< 
Clear<< !
(<<! "
)<<" #
;<<# $
if>> 
(>> 
ConsumeContext>> 
is>> !
not>>" %
null>>& *
)>>* +
{?? 
await@@ %
PublishWithConsumeContext@@ /
(@@/ 0
cancellationToken@@0 A
)@@A B
;@@B C
}AA 
elseBB 
{CC 
awaitDD $
PublishWithNormalContextDD .
(DD. /
cancellationTokenDD/ @
)DD@ A
;DDA B
}EE 
_messagesToPublishGG 
.GG 
ClearGG $
(GG$ %
)GG% &
;GG& '
}HH 	
privateJJ 
asyncJJ 
TaskJJ "
SendWithConsumeContextJJ 1
(JJ1 2
MessageToSendJJ2 ?
toSendJJ@ F
,JJF G
CancellationTokenJJH Y
cancellationTokenJJZ k
)JJk l
{KK 	
ifLL 
(LL 
toSendLL 
.LL 
AddressLL 
isLL !
nullLL" &
)LL& '
{MM 
awaitNN 
ConsumeContextNN $
!NN$ %
.NN% &
SendNN& *
(NN* +
toSendNN+ 1
.NN1 2
MessageNN2 9
,NN9 :
cancellationTokenNN; L
)NNL M
.NNM N
ConfigureAwaitNNN \
(NN\ ]
falseNN] b
)NNb c
;NNc d
}OO 
elsePP 
{QQ 
varRR 
endpointRR 
=RR 
awaitRR $
ConsumeContextRR% 3
!RR3 4
.RR4 5
GetSendEndpointRR5 D
(RRD E
toSendRRE K
.RRK L
AddressRRL S
)RRS T
.RRT U
ConfigureAwaitRRU c
(RRc d
falseRRd i
)RRi j
;RRj k
awaitSS 
endpointSS 
.SS 
SendSS #
(SS# $
toSendSS$ *
.SS* +
MessageSS+ 2
,SS2 3
cancellationTokenSS4 E
)SSE F
.SSF G
ConfigureAwaitSSG U
(SSU V
falseSSV [
)SS[ \
;SS\ ]
}TT 
}UU 	
privateWW 
asyncWW 
TaskWW !
SendWithNormalContextWW 0
(WW0 1
MessageToSendWW1 >
toSendWW? E
,WWE F
CancellationTokenWWG X
cancellationTokenWWY j
)WWj k
{XX 	
ifYY 
(YY 
toSendYY 
.YY 
AddressYY 
isYY !
nullYY" &
)YY& '
{ZZ 
var[[ 
bus[[ 
=[[ 
_serviceProvider[[ *
.[[* +
GetRequiredService[[+ =
<[[= >
IBus[[> B
>[[B C
([[C D
)[[D E
;[[E F
await\\ 
bus\\ 
.\\ 
Send\\ 
(\\ 
toSend\\ %
.\\% &
Message\\& -
,\\- .
cancellationToken\\/ @
)\\@ A
.\\A B
ConfigureAwait\\B P
(\\P Q
false\\Q V
)\\V W
;\\W X
}]] 
else^^ 
{__ 
var``  
sendEndpointProvider`` (
=``) *
_serviceProvider``+ ;
.``; <
GetRequiredService``< N
<``N O!
ISendEndpointProvider``O d
>``d e
(``e f
)``f g
;``g h
varaa 
endpointaa 
=aa 
awaitaa $ 
sendEndpointProvideraa% 9
.aa9 :
GetSendEndpointaa: I
(aaI J
toSendaaJ P
.aaP Q
AddressaaQ X
)aaX Y
.aaY Z
ConfigureAwaitaaZ h
(aah i
falseaai n
)aan o
;aao p
awaitbb 
endpointbb 
.bb 
Sendbb #
(bb# $
toSendbb$ *
.bb* +
Messagebb+ 2
,bb2 3
cancellationTokenbb4 E
)bbE F
.bbF G
ConfigureAwaitbbG U
(bbU V
falsebbV [
)bb[ \
;bb\ ]
}cc 
}dd 	
privateff 
asyncff 
Taskff %
PublishWithConsumeContextff 4
(ff4 5
CancellationTokenff5 F
cancellationTokenffG X
)ffX Y
{gg 	
awaithh 
ConsumeContexthh  
!hh  !
.hh! "
PublishBatchhh" .
(hh. /
_messagesToPublishhh/ A
,hhA B
cancellationTokenhhC T
)hhT U
.hhU V
ConfigureAwaithhV d
(hhd e
falsehhe j
)hhj k
;hhk l
}ii 	
privatekk 
asynckk 
Taskkk $
PublishWithNormalContextkk 3
(kk3 4
CancellationTokenkk4 E
cancellationTokenkkF W
)kkW X
{ll 	
varmm 
publishEndpointmm 
=mm  !
_serviceProvidermm" 2
.mm2 3
GetRequiredServicemm3 E
<mmE F
IPublishEndpointmmF V
>mmV W
(mmW X
)mmX Y
;mmY Z
awaitoo 
publishEndpointoo !
.oo! "
PublishBatchoo" .
(oo. /
_messagesToPublishoo/ A
,ooA B
cancellationTokenooC T
)ooT U
.ooU V
ConfigureAwaitooV d
(ood e
falseooe j
)ooj k
;ook l
}pp 	
privaterr 
sealedrr 
classrr 
MessageToSendrr *
{ss 	
publictt 
MessageToSendtt  
(tt  !
objecttt! '
messagett( /
,tt/ 0
Uritt1 4
?tt4 5
addresstt6 =
)tt= >
{uu 
Messagevv 
=vv 
messagevv !
;vv! "
Addressww 
=ww 
addressww !
;ww! "
}xx 
publiczz 
objectzz 
Messagezz !
{zz" #
getzz$ '
;zz' (
}zz) *
public{{ 
Uri{{ 
?{{ 
Address{{ 
{{{  !
get{{" %
;{{% &
}{{' (
}|| 	
}}} 
}~~ Ç,
°D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Eventing\IntegrationEventConsumer.cs
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
$str P
,P Q
VersionR Y
=Z [
$str\ a
)a b
]b c
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Eventing@ H
{ 
public 

class $
IntegrationEventConsumer )
<) *
THandler* 2
,2 3
TMessage4 <
>< =
:> ?
	IConsumer@ I
<I J
TMessageJ R
>R S
where 
TMessage 
: 
class 
where 
THandler 
: $
IIntegrationEventHandler 1
<1 2
TMessage2 :
>: ;
{ 
private 
readonly 
IServiceProvider )
_serviceProvider* :
;: ;
private 
readonly 
IUnitOfWork $
_unitOfWork% 0
;0 1
public $
IntegrationEventConsumer '
(' (
IServiceProvider( 8
serviceProvider9 H
,H I
IUnitOfWorkJ U

unitOfWorkV `
)` a
{ 	
_serviceProvider 
= 
serviceProvider .
;. /
_unitOfWork 
= 

unitOfWork $
??% '
throw( -
new. 1!
ArgumentNullException2 G
(G H
nameofH N
(N O

unitOfWorkO Y
)Y Z
)Z [
;[ \
} 	
public 
async 
Task 
Consume !
(! "
ConsumeContext" 0
<0 1
TMessage1 9
>9 :
context; B
)B C
{ 	
var 
eventBus 
= 
_serviceProvider +
.+ ,
GetRequiredService, >
<> ?
MassTransitEventBus? R
>R S
(S T
)T U
;U V
eventBus 
. 
ConsumeContext #
=$ %
context& -
;- .
var   
handler   
=   
_serviceProvider   *
.  * +
GetRequiredService  + =
<  = >
THandler  > F
>  F G
(  G H
)  H I
;  I J
using(( 
((( 
var(( 
transaction(( "
=((# $
new((% (
TransactionScope(() 9
(((9 :"
TransactionScopeOption((: P
.((P Q
Required((Q Y
,((Y Z
new)) 
TransactionOptions)) &
{))' (
IsolationLevel))) 7
=))8 9
IsolationLevel)): H
.))H I
ReadCommitted))I V
}))W X
,))X Y+
TransactionScopeAsyncFlowOption))Z y
.))y z
Enabled	))z Å
)
))Å Ç
)
))Ç É
{** 
await++ 
handler++ 
.++ 
HandleAsync++ )
(++) *
context++* 1
.++1 2
Message++2 9
,++9 :
context++; B
.++B C
CancellationToken++C T
)++T U
;++U V
await00 
_unitOfWork00 !
.00! "
SaveChangesAsync00" 2
(002 3
context003 :
.00: ;
CancellationToken00; L
)00L M
;00M N
transaction44 
.44 
Complete44 $
(44$ %
)44% &
;44& '
}55 
await66 
eventBus66 
.66 
FlushAllAsync66 (
(66( )
context66) 0
.660 1
CancellationToken661 B
)66B C
;66C D
}77 	
}88 
public:: 

class:: .
"IntegrationEventConsumerDefinition:: 3
<::3 4
THandler::4 <
,::< =
TMessage::> F
>::F G
:::H I
ConsumerDefinition::J \
<::\ ]$
IntegrationEventConsumer::] u
<::u v
THandler::v ~
,::~ 
TMessage
::Ä à
>
::à â
>
::â ä
where;; 
TMessage;; 
:;; 
class;; 
where<< 
THandler<< 
:<< $
IIntegrationEventHandler<< 1
<<<1 2
TMessage<<2 :
><<: ;
{== 
	protected>> 
override>> 
void>> 
ConfigureConsumer>>  1
(>>1 2(
IReceiveEndpointConfigurator?? ( 
endpointConfigurator??) =
,??= >!
IConsumerConfigurator@@ !
<@@! "$
IntegrationEventConsumer@@" :
<@@: ;
THandler@@; C
,@@C D
TMessage@@E M
>@@M N
>@@N O 
consumerConfigurator@@P d
,@@d e 
IRegistrationContextAA  
contextAA! (
)AA( )
{BB 	 
endpointConfiguratorCC  
.CC  !"
UseInMemoryInboxOutboxCC! 7
(CC7 8
contextCC8 ?
)CC? @
;CC@ A
}DD 	
}EE 
}FF Ñ=
ìD:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\DependencyInjection.cs
[ 
assembly 	
:	 
 
DefaultIntentManaged 
(  
Mode  $
.$ %
Fully% *
)* +
]+ ,
[ 
assembly 	
:	 

IntentTemplate 
( 
$str Y
,Y Z
Version[ b
=c d
$stre j
)j k
]k l
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
{ 
public 

static 
class 
DependencyInjection +
{ 
public 
static 
IServiceCollection (
AddInfrastructure) :
(: ;
this; ?
IServiceCollection@ R
servicesS [
,[ \
IConfiguration] k
configurationl y
)y z
{ 	
services   
.   
AddDbContext   !
<  ! " 
ApplicationDbContext  " 6
>  6 7
(  7 8
(  8 9
sp  9 ;
,  ; <
options  = D
)  D E
=>  F H
{!! 
options"" 
."" 
UseInMemoryDatabase"" +
(""+ ,
$str"", ?
)""? @
;""@ A
options## 
.## !
UseLazyLoadingProxies## -
(##- .
)##. /
;##/ 0
}$$ 
)$$ 
;$$ 
services%% 
.%% 
	AddScoped%% 
<%% 
IUnitOfWork%% *
>%%* +
(%%+ ,
provider%%, 4
=>%%5 7
provider%%8 @
.%%@ A
GetRequiredService%%A S
<%%S T 
ApplicationDbContext%%T h
>%%h i
(%%i j
)%%j k
)%%k l
;%%l m
services&& 
.&& 
AddTransient&& !
<&&! "
IBasicRepository&&" 2
,&&2 3
BasicRepository&&4 C
>&&C D
(&&D E
)&&E F
;&&F G
services'' 
.'' 
AddTransient'' !
<''! "
IContractRepository''" 5
,''5 6
ContractRepository''7 I
>''I J
(''J K
)''K L
;''L M
services(( 
.(( 
AddTransient(( !
<((! "1
%ICorporateFuneralCoverQuoteRepository((" G
,((G H0
$CorporateFuneralCoverQuoteRepository((I m
>((m n
(((n o
)((o p
;((p q
services)) 
.)) 
AddTransient)) !
<))! "
ICustomerRepository))" 5
,))5 6
CustomerRepository))7 I
>))I J
())J K
)))K L
;))L M
services** 
.** 
AddTransient** !
<**! "!
IFileUploadRepository**" 7
,**7 8 
FileUploadRepository**9 M
>**M N
(**N O
)**O P
;**P Q
services++ 
.++ 
AddTransient++ !
<++! "(
IFuneralCoverQuoteRepository++" >
,++> ?'
FuneralCoverQuoteRepository++@ [
>++[ \
(++\ ]
)++] ^
;++^ _
services,, 
.,, 
AddTransient,, !
<,,! "
IOptionalRepository,," 5
,,,5 6
OptionalRepository,,7 I
>,,I J
(,,J K
),,K L
;,,L M
services-- 
.-- 
AddTransient-- !
<--! "
IOrderRepository--" 2
,--2 3
OrderRepository--4 C
>--C D
(--D E
)--E F
;--F G
services.. 
... 
AddTransient.. !
<..! "
IPagingTSRepository.." 5
,..5 6
PagingTSRepository..7 I
>..I J
(..J K
)..K L
;..L M
services// 
.// 
AddTransient// !
<//! "
IPersonRepository//" 3
,//3 4
PersonRepository//5 E
>//E F
(//F G
)//G H
;//H I
services00 
.00 
AddTransient00 !
<00! "
IProductRepository00" 4
,004 5
ProductRepository006 G
>00G H
(00H I
)00I J
;00J K
services11 
.11 
AddTransient11 !
<11! "
IQuoteRepository11" 2
,112 3
QuoteRepository114 C
>11C D
(11D E
)11E F
;11F G
services22 
.22 
AddTransient22 !
<22! "
IUserRepository22" 1
,221 2
UserRepository223 A
>22A B
(22B C
)22C D
;22D E
services33 
.33 
AddTransient33 !
<33! " 
IWarehouseRepository33" 6
,336 7
WarehouseRepository338 K
>33K L
(33L M
)33M N
;33N O
services44 
.44 
AddTransient44 !
<44! ",
 IParentWithAnemicChildRepository44" B
,44B C+
ParentWithAnemicChildRepository44D c
>44c d
(44d e
)44e f
;44f g
services55 
.55 
AddTransient55 !
<55! "/
#IClassicDomainServiceTestRepository55" E
,55E F.
"ClassicDomainServiceTestRepository55G i
>55i j
(55j k
)55k l
;55l m
services66 
.66 
AddTransient66 !
<66! "(
IDomainServiceTestRepository66" >
,66> ?'
DomainServiceTestRepository66@ [
>66[ \
(66\ ]
)66] ^
;66^ _
services77 
.77 
AddTransient77 !
<77! ""
IBaseEntityARepository77" 8
,778 9!
BaseEntityARepository77: O
>77O P
(77P Q
)77Q R
;77R S
services88 
.88 
AddTransient88 !
<88! ""
IBaseEntityBRepository88" 8
,888 9!
BaseEntityBRepository88: O
>88O P
(88P Q
)88Q R
;88R S
services99 
.99 
AddTransient99 !
<99! "&
IConcreteEntityARepository99" <
,99< =%
ConcreteEntityARepository99> W
>99W X
(99X Y
)99Y Z
;99Z [
services:: 
.:: 
AddTransient:: !
<::! "&
IConcreteEntityBRepository::" <
,::< =%
ConcreteEntityBRepository::> W
>::W X
(::X Y
)::Y Z
;::Z [
services;; 
.;; 
AddTransient;; !
<;;! "$
IFilteredIndexRepository;;" :
,;;: ;#
FilteredIndexRepository;;< S
>;;S T
(;;T U
);;U V
;;;V W
services<< 
.<< 
AddTransient<< !
<<<! "$
INestingParentRepository<<" :
,<<: ;#
NestingParentRepository<<< S
><<S T
(<<T U
)<<U V
;<<V W
services== 
.== 
	AddScoped== 
<== 
IDomainEventService== 2
,==2 3
DomainEventService==4 F
>==F G
(==G H
)==H I
;==I J
services>> 
.>> '
AddMassTransitConfiguration>> 0
(>>0 1
configuration>>1 >
)>>> ?
;>>? @
services?? 
.?? 
AddHttpClients?? #
(??# $
configuration??$ 1
)??1 2
;??2 3
return@@ 
services@@ 
;@@ 
}AA 	
}BB 
}CC Á*
¶D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Configuration\MassTransitConfiguration.cs
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
.0 1
Infrastructure1 ?
.? @
Configuration@ M
{ 
public 

static 
class $
MassTransitConfiguration 0
{ 
public 
static 
void '
AddMassTransitConfiguration 6
(6 7
this7 ;
IServiceCollection< N
servicesO W
,W X
IConfigurationY g
configurationh u
)u v
{ 	
services 
. 
	AddScoped 
< 
MassTransitEventBus 2
>2 3
(3 4
)4 5
;5 6
services 
. 
	AddScoped 
< 
	IEventBus (
>( )
() *
provider* 2
=>3 5
provider6 >
.> ?
GetRequiredService? Q
<Q R
MassTransitEventBusR e
>e f
(f g
)g h
)h i
;i j
services 
. 
AddMassTransit #
(# $
x$ %
=>& (
{ 
x 
. -
!SetKebabCaseEndpointNameFormatter 3
(3 4
)4 5
;5 6
x 
. 
AddConsumers 
( 
)  
;  !
x 
. 
UsingInMemory 
(  
(  !
context! (
,( )
cfg* -
)- .
=>/ 1
{ 
cfg 
. 
UseMessageRetry '
(' (
r( )
=>* ,
r- .
.. /
Interval/ 7
(7 8
configuration   %
.  % &
GetValue  & .
<  . /
int  / 2
?  2 3
>  3 4
(  4 5
$str  5 [
)  [ \
??  ] _
$num  ` b
,  b c
configuration!! %
.!!% &
GetValue!!& .
<!!. /
TimeSpan!!/ 7
?!!7 8
>!!8 9
(!!9 :
$str!!: ^
)!!^ _
??!!` b
TimeSpan!!c k
.!!k l
FromSeconds!!l w
(!!w x
$num!!x y
)!!y z
)!!z {
)!!{ |
;!!| }
cfg## 
.## 
ConfigureEndpoints## *
(##* +
context##+ 2
)##2 3
;##3 4
cfg$$ 
.$$ 
UseInMemoryOutbox$$ )
($$) *
context$$* 1
)$$1 2
;$$2 3
}%% 
)%% 
;%% 
x&& 
.&& "
AddInMemoryInboxOutbox&& (
(&&( )
)&&) *
;&&* +
}'' 
)'' 
;'' 
}(( 	
private** 
static** 
void** 
AddConsumers** (
(**( )
this**) -%
IRegistrationConfigurator**. G
cfg**H K
)**K L
{++ 	
cfg,, 
.,, 
AddConsumer,, 
<,, $
IntegrationEventConsumer,, 4
<,,4 5$
IIntegrationEventHandler,,5 M
<,,M N
EnumSampleEvent,,N ]
>,,] ^
,,,^ _
EnumSampleEvent,,` o
>,,o p
>,,p q
(,,q r
typeof,,r x
(,,x y/
"IntegrationEventConsumerDefinition	,,y õ
<
,,õ ú&
IIntegrationEventHandler
,,ú ¥
<
,,¥ µ
EnumSampleEvent
,,µ ƒ
>
,,ƒ ≈
,
,,≈ ∆
EnumSampleEvent
,,« ÷
>
,,÷ ◊
)
,,◊ ÿ
)
,,ÿ Ÿ
.
,,Ÿ ⁄
Endpoint
,,⁄ ‚
(
,,‚ „
config
,,„ È
=>
,,Í Ï
config
,,Ì Û
.
,,Û Ù

InstanceId
,,Ù ˛
=
,,ˇ Ä
$str
,,Å ©
)
,,© ™
;
,,™ ´
cfg-- 
.-- 
AddConsumer-- 
<-- $
IntegrationEventConsumer-- 4
<--4 5$
IIntegrationEventHandler--5 M
<--M N(
QuoteCreatedIntegrationEvent--N j
>--j k
,--k l)
QuoteCreatedIntegrationEvent	--m â
>
--â ä
>
--ä ã
(
--ã å
typeof
--å í
(
--í ì0
"IntegrationEventConsumerDefinition
--ì µ
<
--µ ∂&
IIntegrationEventHandler
--∂ Œ
<
--Œ œ*
QuoteCreatedIntegrationEvent
--œ Î
>
--Î Ï
,
--Ï Ì*
QuoteCreatedIntegrationEvent
--Ó ä
>
--ä ã
)
--ã å
)
--å ç
.
--ç é
Endpoint
--é ñ
(
--ñ ó
config
--ó ù
=>
--û †
config
--° ß
.
--ß ®

InstanceId
--® ≤
=
--≥ ¥
$str
--µ ›
)
--› ﬁ
;
--ﬁ ﬂ
}.. 	
}// 
}00 ﬂ#
•D:\Dev\Intent.Modules.NET\Tests\AdvancedMappingCrud.Repositories.Tests\AdvancedMappingCrud.Repositories.Tests.Infrastructure\Configuration\HttpClientConfiguration.cs
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
 R
,

R S
Version

T [
=

\ ]
$str

^ c
)

c d
]

d e
	namespace 	
AdvancedMappingCrud
 
. 
Repositories *
.* +
Tests+ 0
.0 1
Infrastructure1 ?
.? @
Configuration@ M
{ 
public 

static 
class #
HttpClientConfiguration /
{ 
public 
static 
void 
AddHttpClients )
() *
this* .
IServiceCollection/ A
servicesB J
,J K
IConfigurationL Z
configuration[ h
)h i
{ 	
services 
. 
AddHttpClient 
< "
ICustomersServiceProxy 5
,5 6+
CustomersServiceProxyHttpClient7 V
>V W
(W X
httpX \
=>] _
{ 
ApplyAppSettings $
($ %
http% )
,) *
configuration+ 8
,8 9
$str: h
,h i
$str	j Å
)
Å Ç
;
Ç É
} 
) 
; 
services 
. 
AddHttpClient 
< 
IFileUploadsService 2
,2 3(
FileUploadsServiceHttpClient4 P
>P Q
(Q R
httpR V
=>W Y
{ 
ApplyAppSettings $
($ %
http% )
,) *
configuration+ 8
,8 9
$str: k
,k l
$str	m Å
)
Å Ç
;
Ç É
} 
) 
; 
services 
.   
AddHttpClient   
<    
IProductServiceProxy   3
,  3 4)
ProductServiceProxyHttpClient  5 R
>  R S
(  S T
http  T X
=>  Y [
{!! 
ApplyAppSettings"" $
(""$ %
http""% )
,"") *
configuration""+ 8
,""8 9
$str"": h
,""h i
$str""j 
)	"" Ä
;
""Ä Å
}## 
)## 
;## 
}$$ 	
private&& 
static&& 
void&& 
ApplyAppSettings&& ,
(&&, -

HttpClient'' 
client'' 
,'' 
IConfiguration(( 
configuration(( (
,((( )
string)) 
	groupName)) 
,)) 
string** 
serviceName** 
)** 
{++ 	
client,, 
.,, 
BaseAddress,, 
=,,  
configuration,,! .
.,,. /
GetValue,,/ 7
<,,7 8
Uri,,8 ;
>,,; <
(,,< =
$",,= ?
$str,,? K
{,,K L
serviceName,,L W
},,W X
$str,,X \
",,\ ]
),,] ^
??,,_ a
configuration,,b o
.,,o p
GetValue,,p x
<,,x y
Uri,,y |
>,,| }
(,,} ~
$"	,,~ Ä
$str
,,Ä å
{
,,å ç
	groupName
,,ç ñ
}
,,ñ ó
$str
,,ó õ
"
,,õ ú
)
,,ú ù
;
,,ù û
client-- 
.-- 
Timeout-- 
=-- 
configuration-- *
.--* +
GetValue--+ 3
<--3 4
TimeSpan--4 <
?--< =
>--= >
(--> ?
$"--? A
$str--A M
{--M N
serviceName--N Y
}--Y Z
$str--Z b
"--b c
)--c d
??--e g
configuration--h u
.--u v
GetValue--v ~
<--~ 
TimeSpan	-- á
?
--á à
>
--à â
(
--â ä
$"
--ä å
$str
--å ò
{
--ò ô
	groupName
--ô ¢
}
--¢ £
$str
--£ ´
"
--´ ¨
)
--¨ ≠
??
--Æ ∞
TimeSpan
--± π
.
--π ∫
FromSeconds
--∫ ≈
(
--≈ ∆
$num
--∆ …
)
--…  
;
--  À
}.. 	
}// 
}00 