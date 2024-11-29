﻿using System;
using System.Collections;
using EFT;
using EFT.UI;
using EFT.Visual;
using EFTReflection;
using UnityEngine;

namespace HideDress.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.FieldRef<PlayerBody, object> RefSlotViews;

        public readonly RefHelper.FieldRef<object, IDictionary> RefSlotDic;
        public readonly RefHelper.FieldRef<object, IList> RefSlotList;

        public readonly RefHelper.FieldRef<object, Dress[]> RefDresses;

        public readonly RefHelper.FieldRef<Dress, Renderer[]> RefRenderers;

        public readonly RefHelper.HookRef PlayerModelViewShow;

        private ReflectionModel()
        {
            RefSlotViews = RefHelper.FieldRef<PlayerBody, object>.Create("SlotViews");
            RefSlotDic = RefHelper.FieldRef<object, IDictionary>.Create(RefSlotViews.FieldType, "dictionary_0");
            RefSlotList = RefHelper.FieldRef<object, IList>.Create(RefSlotViews.FieldType, "list_0");
            RefDresses =
                RefHelper.FieldRef<object, Dress[]>.Create(RefSlotList.FieldType.GetGenericArguments()[0], "Dresses");
            RefRenderers = RefHelper.FieldRef<Dress, Renderer[]>.Create("Renderers");

            PlayerModelViewShow = RefHelper.HookRef.Create(typeof(PlayerModelView),
                x => x.Name == "Show" && x.GetParameters()[0].ParameterType != typeof(Profile));
        }
    }
}