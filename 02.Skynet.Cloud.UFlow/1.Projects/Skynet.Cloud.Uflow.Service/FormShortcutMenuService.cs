/************************************************************************************
* Copyright (c) 2019-07-11 12:02:51 优网科技 All Rights Reserved.
* CLR版本： .Standard 2.x
* 公司名称：优网科技
* 命名空间：UWay.Skynet.Cloud.Uflow.Service.Interface
* 文件名：  UWay.Skynet.Cloud.Uflow.Service.Interface.FormShortcutMenu.cs
* 版本号：  V1.0.0.0
* 唯一标识：217a12ec-07f1-4ae3-b0a7-15a1a41c95e6
* 创建人：  magic.s.g.xie
* 电子邮箱：xiesg@uway.cn
* 创建时间：2019-07-11 12:02:51 
* 描述：流程业务 
* 
* 
* =====================================================================
* 修改标记 
* 修改时间：2019-07-11 12:02:51 
* 修改人： 谢韶光
* 版本号： V1.0.0.0
* 描述：
* 
* 
* 
* 
************************************************************************************/



namespace   UWay.Skynet.Cloud.Uflow.Service
{
   using System;
   using UWay.Skynet.Cloud.Data;

   using UWay.Skynet.Cloud.Request;
   using UWay.Skynet.Cloud.Uflow.Entity;
   using System.Collections.Generic;

   using System.Linq;
   using UWay.Skynet.Cloud.Uflow.Service.Interface;
   using UWay.Skynet.Cloud.Uflow.Repository;
   using UWay.Skynet.Cloud.Linq;
   using UWay.Skynet.Cloud;

   /// <summary>
   /// 流程业务服务实现类
   /// </summary>
   public class FormShortcutMenuService: IFormShortcutMenuService
   {
      /// <summary>
      /// 添加流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public long Add(FormShortcutMenu  formShortcutMenu)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new FormShortcutMenuRepository(dbContext).Add(formShortcutMenu);
         }
      }
      /// <summary>
      /// 添加流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public void Add(IList<FormShortcutMenu>  formShortcutMenus)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            new FormShortcutMenuRepository(dbContext).Add(formShortcutMenus);
         }
      }
      /// <summary>
      /// 更新流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public int Update(FormShortcutMenu  formShortcutMenu)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new FormShortcutMenuRepository(dbContext).Update(formShortcutMenu);
         }
      }
      /// <summary>
      /// 删除流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public int Delete(string[] idArrays )
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new FormShortcutMenuRepository(dbContext).Delete(idArrays);
         }
      }
      /// <summary>
      /// 获取指定的流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public FormShortcutMenu GetById(string id)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new FormShortcutMenuRepository(dbContext).GetById(id);
         }
      }
      /// <summary>
      /// 获取所有的流程业务{流程业务}对象(即:一条记录
      /// </summary>
      public DataSourceResult Page(DataSourceRequest request)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new FormShortcutMenuRepository(dbContext).Query().ToDataSourceResult(request);
         }
      }
   }
}
