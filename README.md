# BazaGljivara WASM

Projekt je primjer Blazor WebAssembly primjene u web developmentu.

Aplikacija dozvoljava registraciju i pristup korisnika koji mogu dodati lokacije na kojima je viđena gljiva smrčak unošenjem 
geografske pozicije ( lat i lng )

Većina koda je stvorena uz pomoć tutoriala :
https://jasonwatmore.com/post/2020/11/09/blazor-webassembly-user-registration-and-login-example-tutorial

##Frontend 
Izbildan u WASM - HTML + Bootstrap


##Backend
Lažni backend servis(FakeBackendHandler) koji sprema podatke u Local Storage preglednika


toDo : 	-validacija inputa za Lat i Lng bi trebala filtrirati samo float/decimal ne tekst
 	-baza lokacija bi mogla imati vise polja (napomena, npr)
	-preimenovanje BazaGljivara.Client (ovo .Client je ostalo iz drugog tutoriala ali nisam znao kako da automatski refaktoriram cijeli namespace projekta :) )
    -izbaciti van fakebackend service i zamijeniti ga sa relacijskim bazama sql

