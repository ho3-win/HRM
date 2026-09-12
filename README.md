# HRM

Docker را روشن کن
sudo systemctl start docker

برای اینکه بعداً با روشن شدن سیستم خودش اجرا شود:

sudo systemctl enable docker

2.  ببین SQL Server هنوز وجود دارد
    sudo docker ps -a

اگر sqlserver را دیدی، اجراش کن:

sudo docker start sqlserver

بعد:

sudo docker ps

باید چیزی مثل این ببینی:

sqlserver ... 0.0.0.0:1433->1433/tcp
