# Vizsga-Project

A Storage Manager egy raktárkezelő alkalmazás, amely többek között különböző termékek kezelését teszi lehetővé bármilyen távolságból, nemcsak helyhez kötött irodákból, hanem útközben, mobil eszközökön keresztül is. A felhasználó választhat, hogy a kezelést webes felületről, vagy asztali alkalmazásból végzi. Ennek során a kérések a backend alkalmazásba futnak be. Ez feldolgozza az utasításokat, továbbítja a központi adatbázisba. Itt az utasítások végrehajtásra kerülnek, ami vagy az adatbázis olvasását, vagy annak módosítását eredményezi. Ezt követően a backend alkalmazás a lekért adatokat illetve válaszokat elküldi a felhasználónak. 

Felhasznált technológiák:
- a webes frontend alkalmazás a népszerű React könyvtárral lett kiegészítve 
- az asztali frontend alkalmazás a bevált WPF technológiára alapul
-	a backend alkalmazás WCF technológiára épül
-	a frontend és backend közötti kommunikáció HTTP protokollon keresztül zajlik, RESTful architektúrát alkalmazva
-	a tárolt adatok kezelésére HTTP metódusokat használunk (GET, POST, PUT, DELETE), ezek biztosítják az adatok megtekintését, kiegészítését, módosítását vagy törlését
-	az adatbázis nyilvántartása MySQL adatbázis-kezelő rendszerben történik
