# AKStream Mongodb 版
 ----
 ## 前述
 * 该版本是在[AKStream](https://github.com/chatop2020/AKStream)的基础上做的更新，此版本主要更新是将使用的LiteDb替换成Mongodb，以此来提升稳定性（在使用一段时间后遇到了[LitDb问题](https://github.com/chatop2020/AKStream/issues/51),然后和作者沟通后，他的建议是在观察观察，接着我又在AKStream的qq群里看到有网友使用ak录像功能也出现了litedb造成的问题，群里作者建议替换成mongodb，考虑到我们项目的稳定性，于是便进行了改造
 * 此版本仅对替换为Mongodb部分的代码做更新，不定时拉取AKStream代码
 * 如有人使用中出现问题，可发邮件：yangjiecheng1995@163.com
 * 从本入口进来的朋友，可关注使用[AKStream](https://github.com/chatop2020)来满足大家的监控安防需求
   
 
 ## 使用说明
 - 搭建MongoDb环境（百度、谷歌、cnbing都行）
 - 创建mdb超级管理员、集合、集合对应的管理员
 - 修改`AKStream/Config/AKStreamWeb.json`的MongodbConnStr字段信息，将其替换成自己的环境信息
 ```json
 {
  "MediaServerFirstToRestart": true,
  "DbType": "MySql",
  "OrmConnStr": "Data Source=192.168.3.15;Port=3306;User ID=root;Password=cdtnb...; Initial Catalog=AKStream;Charset=utf8; SslMode=none;Min pool size=1;",
  "MongodbConnStr": "mongodb://{your_account}:{your_password}@{your_host}:{your_port}/{your_collection}",
  "WebApiPort": 5800,
  "AccessKey": "047I4WS1-U51UBO6W-1J4BT21P-MF17IT99-92J8WIHU-944Q4KIW",
  "HttpClientTimeoutSec": 5,
  "WaitEventTimeOutMSec": 10000,
  "WaitSipRequestTimeOutMSec": 5000,
  "DeletedRecordsExpiredDays": 30,
  "EnableGB28181Client": true,
  "ZlmFlvPrefix": "live"
}

 ```
 <font color="red">特别注意：如果使用mdb的超级管理员账号，配置信息末尾需要加`?authSource=admin`例如：</font>
 
 ```json
 "MongodbConnStr": "mongodb://root:root@127.0.0.1:27017/AKStream?authSource=admin",
 ```