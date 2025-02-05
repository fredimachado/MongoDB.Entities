﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace MongoDB.Entities;

/// <summary>
/// Represents a 2D geographical coordinate consisting of longitude and latitude
/// </summary>
public class Coordinates2D
{
    [BsonElement("type")]
    public string Type { get; set; } = "Point";

    [BsonElement("coordinates")]
    public double[] Coordinates { get; set; } = Array.Empty<double>();

    /// <summary>
    /// Instantiate a new Coordinates2D instance with default values
    /// </summary>
    public Coordinates2D() { }

    /// <summary>
    /// Instantiate a new Coordinates2D instance with the supplied longtitude and latitude
    /// </summary>
    public Coordinates2D(double longitude, double latitude)
    {
        Type = "Point";
        Coordinates = new[] { longitude, latitude };
    }

    /// <summary>
    /// Converts a Coordinates2D instance to a GeoJsonPoint of GeoJson2DGeographicCoordinates 
    /// </summary>
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> ToGeoJsonPoint()
    {
        return GeoJson.Point(GeoJson.Geographic(Coordinates[0], Coordinates[1]));
    }

    /// <summary>
    /// Create a GeoJsonPoint of GeoJson2DGeographicCoordinates with supplied longitude and latitude
    /// </summary>
    public static GeoJsonPoint<GeoJson2DGeographicCoordinates> GeoJsonPoint(double longitude, double latitude)
    {
        return GeoJson.Point(GeoJson.Geographic(longitude, latitude));
    }
}

/// <summary>
/// Fluent aggregation pipeline builder for GeoNear
/// </summary>
/// <typeparam name="T">The type of entity</typeparam>
public class GeoNear<T> where T : IEntity
{
#pragma warning disable IDE1006
    public Coordinates2D? near { get; set; }
    public string? distanceField { get; set; }
    public bool spherical { get; set; }
    [BsonIgnoreIfNull] public int? limit { get; set; }
    [BsonIgnoreIfNull] public double? maxDistance { get; set; }
    [BsonIgnoreIfNull] public BsonDocument? query { get; set; }
    [BsonIgnoreIfNull] public double? distanceMultiplier { get; set; }
    [BsonIgnoreIfNull] public string? includeLocs { get; set; }
    [BsonIgnoreIfNull] public double? minDistance { get; set; }
    [BsonIgnoreIfNull] public string? key { get; set; }

    internal IAggregateFluent<T> ToFluent(AggregateOptions? options = null, IClientSessionHandle? session = null)
    {
        var stage = new BsonDocument { { "$geoNear", this.ToBsonDocument() } };

        return session == null
                ? DB.Collection<T>().Aggregate(options).AppendStage<T>(stage)
                : DB.Collection<T>().Aggregate(session, options).AppendStage<T>(stage);
    }
}
