# Yunanistan Vize
Yunanistan vize başvurusu için uygun randevu tarihlerini tarayan ve uygun tarihi bulması durumunda sms atan uygulamadır.
 
 ## Kurulum
 1. İlk adım olarak kosmos sitesinden başvuru formunun doldurulmuş olması gereklidir. => https://kosmosvize.com.tr
 
 2. https://twilio.com hesabından ücretsiz bir hesap açılıp (15$ bakiye vermektedir) ```appsettings.json``` dosyasındaki "AccountSid", "AuthToken", "From", "To" alanlarına girilir.
 
 3. ```appsettings.json``` dosyasındaki diğer bilgileri doldurun. Örn; "NationalityNumber" (Tc Kimlik No), "DealerId" ( İzmir=5, Istanbul=1, Adana=1014, Ankara=1014), "AppointmentTypeId" (Standart=16, Vip=18, Call-Standart=196, Mobile=310), Month (Ocak=1, Şubat=2, ...).
 
 4. Projeyi docker container olarak çalıştırmak için ;
    ```
    docker build -t imagename .
    docker run -d  --name containername imagename .
