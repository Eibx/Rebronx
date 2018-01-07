# Certificates

To create certificate for the server run the following:

`openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost:21220'`  
`openssl pkcs12 -export -inkey key.pem -in cert.pem -out rebronx.p12 -password pass:rebronx_pass`

Open https://localhost:21220 and make an exception for the untrusted certificate

