# I. NetAcademia Web Konferencia 2017.12.05 - Sugár
Az Identity Server 4 demo kódtára

[Az előadás diái](https://docs.google.com/presentation/d/12gt9Qnv23NfkOYA-D8IXc0M8kEqkHYRYD_tR1M2Ux2g)

Visszajelzéseket nagy örömmel fogadok a [plesz.gabor@netacademia.hu](mailto:plesz.gabor@netacademia.hu) email címen.

## Bevezetés
Ez tulajdonképpen egy rövid névsorolvasás. Gyorsan felsorolok mindent, ami a prezentációban előfordul, a teljesség igénye nélkül.

### Kulcskezelés
![Kulcskezeés a fizikai világban](/img/atomtitkok01.png)

### Elektronikus kulcsok (biztonsági tokenek)
A biztonsági tokenek védett adatcsomagok. Általában Információt tartalmaznak a kibocsátóról és a kibocsátás céljáról. Alá vannak írva (szimmetrikus vagy asszimetrikus aláírással, így védettek módosítás ellen és a hitelességük ellenőrizhető. Általában tartalmaznak lejárati időpontot (ez lehet rövid is, hogyha ellopják a tokent, akkor ne használhassák sokáig)
#### Security Assertion Markup Language (SAML)
A nagyvállalati világ nehézsúlyú versenyzője, nagyjából mindent tud amit az elektronikus kulcsoknak tudnia kell. Mindenféle szimmetrikus és asszimetrikus titkosítást támogat, és az adatok reprezentációja XML. 

Probléma: Az XML ugyan egy univerzális versenyző, részleges és teljes titkosítással, azonban a feldolgozásához kell az XML stack, ami komoly erőforrást igényel. Ez böngészőben és mobil eszközökben (a feldolgozáshoz szükséges erőforrások szempontjából) drágává teszi a használatát.
#### Simple Web Token (SWT)
Ez egy faék egyszerűségű megoldás, aminek egyszerű és erőforrásilag olcsó a kibocsátása és a feldolgozása. Csak szimmetrikus titkosítást támogat, és az adatok reprezentációja Form/URL alapú.

Probléma: a szimmetrikus aláírás miatt csak a kibocsátó ellenőrizheti a kibocsátott tokent, így ebből komplex rendszerek építése nagyon bonyolult feladat.
#### JSON Web Token (JWT)
Formálódó szabvány a véglegesítés közelében, az IETF dolgozik rajta az OIDC megbízásából. Az Oauth2/OIDC tokenes kommunikációja ezt használja, az előző két megoldás legjobb részeit egyesíti magában. Támogat szimetrikus és asszimetrikus titkosítást, és az adatreprezentációja [JSON](https://www.json.org/) (JavaScript Object Notation) formátumú, ami a JavaScript-tel együtt minden platformon megtalálható, és a kibocsátásához/feldolgozásához nem kell különös erőforrásigény.

### Protokollok
A nagyvállalati világ nehézsúlyú versenyzői (Kerberos, LDAP, AD) ebben a témakörben is régóta rendelkezésre állnak. Azonban használatukhoz az kell, hogy a részt vevő eszközök egy bizalmi körhöz tartozzanak (egy szervezethez, vagy egymással bizalmi viszonyban álló szervezetekhez). Intraneten kiválóak, de az Interneten, ahol nincs minden egy bizalmi körben nem igazán jók.

#### Oauth2
Átadott jogosultság (delegated authorization) kezelésével kapcsolatos
#### OpenID Connect (OIDC)

### Kulcskezelés az elektronikus világban egyszerűen
![Kulcskezeéls a fizikai világban](/img/Atomtitkok02.png)

### Kulcskezelés az elektronikus világban tényszerűen
![Kulcskezelés a fizikai világban](/img/Atomtitkok03.png)

## A kódtár használata
Ennek a kódtárnak a futtatásához telepített [Docker](https://www.docker.com/) és git kliens szükséges.

a kódtár lehúzása 

```
git clone https://github.com/gplesz/webconf201712/
```

után belépve a kódtár könyvtárába (./webconf201712) elindítjuk a szervereinket

```
docker-compose up
```

és ezzel három (az Identity Server és két az Identity Server-t használó) szolgáltatás elérhető a gépen:

Identity Provider: http://localhost:5000

MVC alkalmazás: http://localhost:5001

Resource API: http://localhost:5004

ezek után nézzük a teszteket.

## Tesztelés
Teszteléshez a [Visual Studio Code](https://code.visualstudio.com/) ingyenes, multiplatformos és [nyílt forráskódú](https://github.com/Microsoft/vscode) eszközt használtam a konferencián. Elsősorban azért, mert a [http teszthívásokat](https://github.com/gplesz/webconf201712/blob/master/testcalls.http) egy [kellemes kiegészítésével](https://github.com/Huachao/vscode-restclient) minden eddiginél könnyebben el tudtam végezni. 

### Identity Provider tesztelése
Egy egyszerű teszttel kérünk egy hozzáférési tokent az Identity Provider-től, elküldve a hitelesítési adatokat. A kérés egy HTTP POST hívás segítségével történik, a paramétereket pedig [***x-www-form-urlencoded***](https://www.w3.org/TR/html401/interact/forms.html#h-17.13.4.1) formában adjuk meg, mivel a szabvány kötelezően csak ezt teszi lehetővé.

Az információt ehhez a híváshoz az Identity Server konfigurációjából vesszük, ami a [appsettings.json](https://github.com/gplesz/webconf201712/blob/master/is4inmem/appsettings.json#L46) kliens alkalmazásokat leíró részében található:

```json
  "Clients": [
    {
      "ClientId": "client",
      "ClientName": "Client Credentials Client",

      // 511536EF-F270-4058-80CA-1C89C192F69A
      "ClientSecrets": [ { "Value": "fU7fRb+g6YdlniuSqviOLWNkda1M/MuPtH6zNI9inF8=" } ],
      "AllowedGrantTypes": [ "client_credentials" ],
      "AllowedScopes": [ "api1" ]
    },

    //(...)

  ]
```

tehát a [kérés](https://github.com/gplesz/webconf201712/blob/master/testcalls.http#L1) a következő:
```http
post http://localhost:5000/connect/token 
content-type: application/x-www-form-urlencoded

client_id=client&client_secret=511536EF-F270-4058-80CA-1C89C192F69A&grant_type=client_credentials
```

Ahogy a konfigurációban látszik, az engedélyezett hitelesítési mód (Allowed Grant Type) a **client_credentials**, a kliens alkalmazás azonosítója (ClientId) a **client**, a jelszava a **511536EF-F270-4058-80CA-1C89C192F69A**. Ez a json-ben nem annyira látszik, de ClientSecrets szekcióban a megadott érték: ***fU7fRb+g6YdlniuSqviOLWNkda1M/MuPtH6zNI9inF8=*** az a jelszó sha256-tal képzett értéke, amit az Identity Server használ a jelszavak kezelésére (tárolására, ellenőrzésére).

A lényege, hogy egyirányú transzformáció, az sha256 értékéből (ami fU7fRb+g6YdlniuSqviOLWNkda1M/MuPtH6zNI9inF8=) nem nyerhető vissza a jelszó (511536EF-F270-4058-80CA-1C89C192F69A). Az a javasolt *jelszó tárolási mód*, így, ha véletlenül kompromittálódik a szerver adatbázisa, a jelszavak nem visszanyerhetőek.

erre az Identity Server válasza valami ilyesmi:
```
HTTP/1.1 200 OK
Date: Sat, 09 Dec 2017 17:53:46 GMT
Content-Type: application/json; charset=UTF-8
Server: Kestrel
Cache-Control: no-store, no-cache, max-age=0
Pragma: no-cache
Transfer-Encoding: chunked

{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVjMTc5YzIxMWZmZjFkMDRkYjk5N2QxZWRlM2RlMjU3IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTI4NDIwMjYsImV4cCI6MTUxMjg0NTYyNiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6ImNsaWVudCIsInNjb3BlIjpbImFwaTEiXX0.VNFOJstFSFBJI1HEsf58qJx5GyvVFDa0qurB9oMcEfqQAMsfOceHxGV6x3BGeho0eccbyIk6l-R0fga-TnZQbrJb3wNkVbLyAjkSqW49ssmiT7qghnQWzNPXUnOfHKjJkx3jVZBkrXn4r3ukpael09Ly_aeqfz8aKyJ_0neiL1RwP6OjhKqHvfihiSXh3D0krywTOwylhnTPPLvWdbv5naYKyVQyDnAJA6abKzWrEqLjZ43Jxtz4Kxv3D56cFVxYD_75E0Txf0kKoJXBmxGUHQJmvYGueYsmuOou5S6YbvyU61fJoE0FostC6VXUg1Zepf-fvnc0_J9JQIbbkuXERw",
  "expires_in": 3600,
  "token_type": "Bearer"
}
```

Ebből az Access Token értéke a legérdekesebb, ő maga egy JWT token. Vegyük észre, hogy három Base64 enkódolt részből áll, amit "."-ok választanak el egymástól.

![jwt.io](/img/jwt-io.png)
Ha emberi fogyasztásra alkalmas formába szeretnénk hozni, akkor a legegyszerűbb, ha megnyitjuk a https://jwt.io weboldalt és a baloldali mezőbe másoljuk a tokenünket. Ekkor a jobboldalon megjelenik a token JSON formája, ami a következő:

Header
```
{
  "alg": "RS256",
  "kid": "5c179c211fff1d04db997d1ede3de257",
  "typ": "JWT"
}
```

Payload (DATA)
```json
{
  "nbf": 1512842026,
  "exp": 1512845626,
  "iss": "http://localhost:5000",
  "aud": [
    "http://localhost:5000/resources",
    "api1"
  ],
  "client_id": "client",
  "scope": [
    "api1"
  ]
}
```

### API tesztje

Az API tesztjéhez a kéréshez az API szerver [egyik Action-jére](https://github.com/gplesz/webconf201712/blob/master/testapi/Controllers/ValuesController.cs#L16) hívunk [HTTP GET kéréssel](https://github.com/gplesz/webconf201712/blob/master/testcalls.http#L9):

```
@token = Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjVjMTc5YzIxMWZmZjFkMDRkYjk5N2QxZWRlM2RlMjU3IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTI4NDIwMjYsImV4cCI6MTUxMjg0NTYyNiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6ImNsaWVudCIsInNjb3BlIjpbImFwaTEiXX0.VNFOJstFSFBJI1HEsf58qJx5GyvVFDa0qurB9oMcEfqQAMsfOceHxGV6x3BGeho0eccbyIk6l-R0fga-TnZQbrJb3wNkVbLyAjkSqW49ssmiT7qghnQWzNPXUnOfHKjJkx3jVZBkrXn4r3ukpael09Ly_aeqfz8aKyJ_0neiL1RwP6OjhKqHvfihiSXh3D0krywTOwylhnTPPLvWdbv5naYKyVQyDnAJA6abKzWrEqLjZ43Jxtz4Kxv3D56cFVxYD_75E0Txf0kKoJXBmxGUHQJmvYGueYsmuOou5S6YbvyU61fJoE0FostC6VXUg1Zepf-fvnc0_J9JQIbbkuXERw

get http://localhost:5004/api/Values
Authorization: {{token}}

```

És a válasz **401 Unauthorized**, ha token nélkül kérünk
```
HTTP/1.1 401 Unauthorized
Date: Sat, 09 Dec 2017 18:32:04 GMT
Server: Kestrel
Content-Length: 0
```

Ha pedig tokennel kérjük, kiszolgál az API
```
HTTP/1.1 200 OK
Date: Sat, 09 Dec 2017 18:30:14 GMT
Content-Type: application/json; charset=utf-8
Server: Kestrel
Transfer-Encoding: chunked

[
  "value1",
  "value2"
]
```

### MVC alkalmazás tesztelése

## A példakód felépítése

### Identity Provider 

```
dotnet new is4inmem
```

### Erőforrás (API)

```
dotnet new api
```

```
dotnet add package IdentityServer4.AccessTokenValidation
```

### MVC alkalmazás 


```
dotnet new mvc
```

```
dotnet add package IdentityModel
```

