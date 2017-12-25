﻿using System;
using System.IO;

namespace TsMap
{
    public class TsSector
    {
        public string FilePath { get; }
        public TsMapper Mapper { get; }

        private bool _empty;

        public byte[] Stream { get; private set; }

        public TsSector(TsMapper mapper, string filePath)
        {
            Mapper = mapper;
            FilePath = filePath;
            if (!File.Exists(FilePath))
            {
                _empty = true;
                return;
            }

            Stream = File.ReadAllBytes(FilePath);
        }

        public void Parse()
        {
            var itemCount = BitConverter.ToUInt32(Stream, 0x10);
            if (itemCount == 0) _empty = true;
            if (_empty) return;

            var lastOffset = 0x14;

            for (var i = 0; i < itemCount; i++)
            {
                var type = (TsItemType)BitConverter.ToUInt32(Stream, lastOffset);
                
                switch (type)
                {
                    case TsItemType.Road:
                    {
                        var item = new TsRoadItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        if (item.Valid) Mapper.Roads.Add(item);
                        break;
                    }
                    case TsItemType.Prefab:
                    {
                        var item = new TsPrefabItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        if (item.Valid) Mapper.Prefabs.Add(item);
                        break;
                    }
                    case TsItemType.Company:
                    {
                        var item = new TsCompanyItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.Service:
                    {
                        var item = new TsServiceItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.CutPlane:
                    {
                        var item = new TsCutPlaneItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.City:
                    {
                        var item = new TsCityItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        if (item.Valid) Mapper.Cities.Add(item); break;
                    }
                    case TsItemType.MapOverlay:
                    {
                        var item = new TsMapOverlayItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        if (item.Valid) Mapper.MapOverlays.Add(item); break;
                    }
                    case TsItemType.Ferry:
                    {
                        var item = new TsFerryItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        if (item.Valid) Mapper.Ferries.Add(item); break;
                    }
                    case TsItemType.Garage:
                    {
                        var item = new TsGarageItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.Trigger:
                    {
                        var item = new TsTriggerItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.FuelPump:
                    {
                        var item = new TsFuelPumpItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.RoadSideItem:
                    {
                        var item = new TsRoadSideItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.BusStop:
                    {
                        var item = new TsBusStopItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.TrafficRule:
                    {
                        var item = new TsTrafficRuleItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.TrajectoryItem:
                    {
                        var item = new TsTrajectoryItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    case TsItemType.MapArea:
                    {
                        var item = new TsMapAreaItem(this, lastOffset);
                        lastOffset += item.BlockSize;
                        break;
                    }
                    default:
                    {
                        Log.Msg($"Unknown Type: {type} in {Path.GetFileName(FilePath)} @ {lastOffset}");
                        break;
                    }
                }
            }

            var nodeCount = BitConverter.ToInt32(Stream, lastOffset);
            for (var i = 0; i < nodeCount; i++)
            {
                TsNode node = new TsNode(this, lastOffset += 0x04);
                if (!Mapper.Nodes.ContainsKey(node.Uid))
                    Mapper.Nodes.Add(node.Uid, node);
                lastOffset += 0x34;
            }
        }

        public void ClearFileData()
        {
            Stream = null;
        }
    }
}