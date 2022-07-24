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
    public static class VideoHelper
    {
        /// <summary>
        ///  清空实时流
        /// </summary>
        public static void ClearVideoOnlineChannels()
        {
            var videoOnlineMediaInfo = GCommon.MongoDb.GetColletion<VideoChannelMediaInfo>("video_online_media_info");
            Expression<Func<VideoChannelMediaInfo, bool>> lanbda = (m => 1 == 1);
            //var items = videoOnlineMediaInfo.Find(new BsonDocument()).ToList();
            var items = videoOnlineMediaInfo.Find(lanbda).ToList();
            if (items.Count > 0)
            {
                videoOnlineMediaInfo.DeleteMany(lanbda);
            }

            GCommon.MongoDb.VideoOnlineInfo = videoOnlineMediaInfo;
        }
    }
}