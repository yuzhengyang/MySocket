/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/
using System;
using System.Text;
using System.Web;
using System.Web.Caching;

/// <summary>
/// 缓存相关的操作类
/// </summary>
public class DataCache
{
    protected static volatile System.Web.Caching.Cache webCache = System.Web.HttpRuntime.Cache;

    protected static int _timeOut = 720; // 设置缓存存活期为720分钟(12小时)

    /// <summary>
    /// 设置到期相对时间[单位：／分钟] 
    /// </summary>
    //public int TimeOut
    //{
    //    set { _timeOut = value > 0 ? value : 6000; }
    //    get { return _timeOut > 0 ? _timeOut : 6000; }
    //}

    /// <summary>
    /// 设置当前应用程序指定CacheKey的Cache值
    /// </summary>
    /// <param name="CacheKey"></param>
    /// <param name="objObject"></param>
    public static void SetCache(string CacheKey, object objObject)
    {
        if (CacheKey == null || CacheKey.Length == 0 || objObject == null)
        {
            return;
        }
        webCache.Insert(CacheKey, objObject, null, DateTime.UtcNow.AddMinutes(_timeOut), System.Web.Caching.Cache.NoSlidingExpiration);
    }
    /// <summary>
    /// 更新相应Key的缓存对象
    /// </summary>
    /// <param name="CacheKey"></param>
    /// <param name="objObject"></param>
    public static void UpdateCache(object CacheKey, object objObject)
    {
        if (CacheKey == null || objObject == null)
        {
            return;
        }
        if (GetCache(CacheKey.ToString()) != null)
        {
            RemoveCache(CacheKey.ToString());
        }
        string key = CacheKey.ToString();
        SetCache(key, objObject);
    }

    /// <summary>
    /// 设置当前应用程序指定CacheKey的Cache值
    /// </summary>
    /// <param name="CacheKey">cache的key值</param>
    /// <param name="objObject">插入cache的对象</param>
    /// <param name="absoluteExpiration">绝对到期时间，即从缓存创建开始算起</param>
    /// <param name="slidingExpiration">弹性过期时间，即从最后一次访问该缓存对象算起</param>
    public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
    {
        System.Web.Caching.Cache objCache = HttpRuntime.Cache;
        objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
    }

    /// <summary>
    /// 获取当前应用程序指定CacheKey的Cache值
    /// </summary>
    /// <param name="CacheKey"></param>
    /// <returns></returns>
    public static object GetCache(string CacheKey)
    {
        if (CacheKey == null || CacheKey.Length == 0)
        {
            return null;
        }
        return webCache.Get(CacheKey);
    }
    /// <summary>
    /// 建立回调委托的一个实例
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="reason"></param>
    public void onRemove(string key, object val, CacheItemRemovedReason reason)
    {
        switch (reason)
        {
            case CacheItemRemovedReason.DependencyChanged:
                break;
            case CacheItemRemovedReason.Expired:
                {
                    break;
                }
            case CacheItemRemovedReason.Removed:
                {
                    break;
                }
            case CacheItemRemovedReason.Underused:
                {
                    break;
                }
            default: break;
        }

        //如需要使用缓存日志,则需要使用下面代码
        //myLogVisitor.WriteLog(this,key,val,reason);
    }
    /// <summary>
    /// 从当前cache中移除缓存对象
    /// </summary>
    /// <param name="CacheKey"></param>
    public static void RemoveCache(string CacheKey)
    {
        if (CacheKey == null || CacheKey.Length == 0)
        {
            return;
        }
        webCache.Remove(CacheKey);
    }
}
