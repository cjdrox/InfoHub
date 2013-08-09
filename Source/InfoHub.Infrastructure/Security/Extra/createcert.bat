makecert -r -pe -n "CN=InfoHub" -b 01/01/2007 -e 01/01/2010 -sky exchange C:\InfoHub.cer -sv C:\InfoHub.pvk
pvk2pfx.exe -pvk C:\InfoHub.pvk -spc C:\InfoHub.cer -pfx C:\InfoHub.pfx
