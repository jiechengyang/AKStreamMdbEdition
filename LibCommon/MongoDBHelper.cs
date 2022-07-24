using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using LibCommon.Enums;
using LibCommon.Structs;

namespace LibCommon
{
    /// <summary>
    /// MongoDb 数据库操作类
    /// </summary>
    public class MongoDBHelper
    {

        /// <summary>
        /// 数据库对象
        /// </summary>
        private IMongoDatabase _dataBase;

        /// <summary>
        ///  为减少修改，需要维护这个数据
        /// </summary>
        public IMongoCollection<VideoChannelMediaInfo> VideoOnlineInfo { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conStrMdb">连接字符串</param>
        public MongoDBHelper(string conStrMdb)
        {
            _dataBase = GetDb(conStrMdb);
        }

        /// <summary>
        /// 若没有，根据传入的数据库名字来生成对应的数据库名，否则，返回db
        /// </summary>
        /// <param name="conStrMdb">数据库连接</param>
        /// <returns></returns>
        private IMongoDatabase GetDb(string conStrMdb)
        {
            var db = new MongoClient(conStrMdb).GetDatabase(new MongoUrlBuilder(conStrMdb).DatabaseName);
            return db;
        }

        /// <summary>
        /// 创建集合对象
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <typeparam name="T">集合对象</typeparam>
        /// <returns></returns>
        public IMongoCollection<T> GetColletion<T>(string collName)
        {
            return _dataBase.GetCollection<T>(collName);
        }

        /// <summary>
        /// 获取指定数据库集合中的所有的文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<T> FindAll<T>(string tableName)
        {
            List<T> list = new List<T>();
            try
            {
                var collection = GetColletion<T>(collName: tableName);
                FilterDefinition<T> filter = Builders<T>.Filter.Empty;
                list = collection.Find<T>(filter).ToList<T>();
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="document">插入的对象</param>
        /// <returns>异常返回-101</returns>
        public DbMessage Insert<T>(string collName, T document)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                coll.InsertOne(document);
                return new DbMessage { Ex = string.Empty, iFlg = 1 };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="documents">要插入的对象集合</param>
        /// <returns>异常返回-101</returns>
        public DbMessage InsertMany<T>(string collName, List<T> documents)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                coll.InsertMany(documents);
                return new DbMessage { Ex = string.Empty, iFlg = documents.Count };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 修改文档
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="filter">修改条件</param>
        /// <param name="update">修改结果</param>
        /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
        /// <returns>修改影响文档数,异常返回-101</returns>
        public DbMessage Update<T>(string collName, Expression<Func<T, Boolean>> filter, UpdateDefinition<T> update, Boolean upsert = false)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                var result = coll.UpdateMany(filter, update, new UpdateOptions { IsUpsert = upsert });
                return new DbMessage { Ex = string.Empty, iFlg = result.ModifiedCount };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 用新对象替换新文档
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="filter">修改条件</param>
        /// <param name="t">新对象</param>
        /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
        /// <returns>修改影响文档数,异常返回-101</returns>
        public DbMessage Update<T>(string collName, Expression<Func<T, Boolean>> filter, T t, Boolean upsert = false)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                BsonDocument document = t.ToBsonDocument<T>();
                document.Remove("_id");
                // UpdateDocument update = new UpdateDocument("$set", document);
                // UpdateDocument update = new Builders<BsonDocument>.Update("$set", document);
                var update = Builders<T>.Update.Set("$set", document);
                var result = coll.UpdateMany(filter, update, new UpdateOptions { IsUpsert = upsert });
                return new DbMessage { Ex = string.Empty, iFlg = result.ModifiedCount };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 按BsonDocument条件删除
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="document">文档</param>
        /// <returns>异常返回-101</returns>
        public DbMessage Delete<T>(string collName, BsonDocument document)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                var result = coll.DeleteOne(document);
                return new DbMessage { Ex = string.Empty, iFlg = result.DeletedCount };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 按条件表达式删除
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>异常返回-101</returns>
        public DbMessage Delete<T>(string collName, Expression<Func<T, Boolean>> predicate)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                var result = coll.DeleteOne(predicate);
                return new DbMessage { Ex = string.Empty, iFlg = result.DeletedCount };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 按检索条件删除（建议用Builders-T构建复杂的查询条件）
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public DbMessage Delete<T>(string collName, FilterDefinition<T> filter)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                var result = coll.DeleteOne(filter);
                return new DbMessage { Ex = string.Empty, iFlg = result.DeletedCount };
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return new DbMessage { Ex = ex.Message, iFlg = -101 };
            }
        }

        /// <summary>
        /// 查询，复杂查询直接用Linq处理
        /// </summary>
        /// <param name="collName">集合名称</param>
        /// <returns>要查询的对象</returns>
        public IQueryable<T> GetQueryable<T>(string collName)
        {
            try
            {
                var coll = GetColletion<T>(collName);
                return coll.AsQueryable<T>();
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(ex.Message);
                return null;
            }
        }
    }

    public class DbMessage
    {
        /// <summary>
        /// 反馈数量
        /// </summary>
        public long iFlg { get; set; }
        /// <summary>
        /// 反馈文字描述
        /// </summary>
        public string Ex { get; set; }
    }
}