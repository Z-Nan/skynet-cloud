/************************************************************************************
* Copyright (c) 2019-07-11 12:08:06 优网科技 All Rights Reserved.
* CLR版本： .Standard 2.x
* 公司名称：优网科技
* 命名空间：UWay.Skynet.Cloud.Uflow.Repository
* 文件名：  UWay.Skynet.Cloud.Uflow.Repository.FormTreemenuStatus.cs
* 版本号：  V1.0.0.0
* 唯一标识：d7ca7a23-640d-431b-a6b6-942260ee1d4a
* 创建人：  magic.s.g.xie
* 电子邮箱：xiesg@uway.cn
* 创建时间：2019-07-11 12:08:06 
* 描述：流程业务 
* 
* 
* =====================================================================
* 修改标记 
* 修改时间：2019-07-11 12:08:06 
* 修改人： 谢韶光
* 版本号： V1.0.0.0
* 描述：
* 
* 
* 
* 
************************************************************************************/



namespace   UWay.Skynet.Cloud.Uflow.Repository
{
   using System;
   using UWay.Skynet.Cloud.Data;

   using  UWay.Skynet.Cloud.Uflow.Entity;
   using System.Linq;
   using System.Collections.Generic;

   /// <summary>
   /// 流程业务仓储类
   /// </summary>
   public class FormTreemenuStatusRepository:ObjectRepository
   {
      public FormTreemenuStatusRepository(IDbContext uow):base(uow)
      {
      }
      /// <summary>
      /// 添加流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public long Add(FormTreemenuStatus  formTreemenuStatus)
      {
         return Add<FormTreemenuStatus>(formTreemenuStatus);
      }
      /// <summary>
      /// 批量添加流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public void Add(IList<FormTreemenuStatus>  formTreemenuStatuss)
      {
         Batch<long, FormTreemenuStatus>(formTreemenuStatuss, (u, v) => u.Insert(v));
      }
      /// <summary>
      /// 更新流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public int Update(FormTreemenuStatus  formTreemenuStatus)
      {
         return Update<FormTreemenuStatus>(formTreemenuStatus);
      }
      /// <summary>
      /// 删除流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public int Delete(string[] idArrays )
      {
         return Delete<FormTreemenuStatus>(p => idArrays.Contains(p.Fid)); 
      }
      /// <summary>
      /// 获取指定的流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public FormTreemenuStatus GetById(string id)
      {
         return GetByID<FormTreemenuStatus>(id);
      }
      /// <summary>
      /// 获取所有的流程业务{流程业务}对象
      /// </summary>
      public IQueryable<FormTreemenuStatus> Query()
      {
         return CreateQuery<FormTreemenuStatus>();
      }
   }
}
