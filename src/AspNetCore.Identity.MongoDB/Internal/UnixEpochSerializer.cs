using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AspNetCore.Identity.MongoDB.Internal
{
    /// <remarks>
    /// see also https://github.com/mongodb/mongo-csharp-driver/blob/aec938641c4d7fa5d812b13420efafbba63430e9/src/MongoDB.Bson/Serialization/Serializers/Int64Serializer.cs
    /// see also https://github.com/tugberkugurlu/f1-graph/blob/94dd147d2dab6eb074c2d812821c521c8274aa60/src/f1-domain/ZonedDateTimeSerializer.cs
    /// </remarks>
    internal abstract class UnixEpochSerializer<T> : SerializerBase<T>
        where T : class
    {
        private readonly Func<T, DateTime> _dateTimeProvider;
        private readonly Func<DateTime, T> _constructor;
        private readonly Int64Serializer _int64Serializer = new Int64Serializer();

        protected UnixEpochSerializer(Func<T, DateTime> dateTimeProvider, Func<DateTime, T> constructor)
        {
            if (dateTimeProvider == null) throw new ArgumentNullException(nameof(dateTimeProvider));
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            
            _dateTimeProvider = dateTimeProvider;
            _constructor = constructor;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            var bsonWriter = context.Writer;
            if (value != null)
            {
                bsonWriter.WriteInt64(BsonUtils.ToMillisecondsSinceEpoch(_dateTimeProvider(value)));
            }
            else
            {
                bsonWriter.WriteNull();
            }
        }

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            BsonType bsonType = bsonReader.GetCurrentBsonType();

            switch (bsonType)
            {
                case BsonType.Int64:
                    var epoch = _int64Serializer.Deserialize(context);
                    var dateTime = BsonUtils.ToDateTimeFromMillisecondsSinceEpoch(epoch);
                    return _constructor(dateTime);

                case BsonType.Null:
                    return null;

                default:
                    throw CreateCannotDeserializeFromBsonTypeException(bsonType);
            }
        }
    }
}