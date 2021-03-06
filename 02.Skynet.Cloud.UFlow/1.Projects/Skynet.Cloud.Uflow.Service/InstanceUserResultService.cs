/************************************************************************************
* Copyright (c) 2019-07-11 12:02:45 优网科技 All Rights Reserved.
* CLR版本： .Standard 2.x
* 公司名称：优网科技
* 命名空间：UWay.Skynet.Cloud.Uflow.Service.Interface
* 文件名：  UWay.Skynet.Cloud.Uflow.Service.Interface.InstanceUserResult.cs
* 版本号：  V1.0.0.0
* 唯一标识：2afe3dcc-0825-47b3-80fe-7a26278daff6
* 创建人：  magic.s.g.xie
* 电子邮箱：xiesg@uway.cn
* 创建时间：2019-07-11 12:02:45 
* 描述：人员处理结果表 
* 
* 
* =====================================================================
* 修改标记 
* 修改时间：2019-07-11 12:02:45 
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
   /// 人员处理结果表服务实现类
   /// </summary>
   public class InstanceUserResultService: IInstanceUserResultService
   {
      /// <summary>
      /// 添加人员处理结果表{人员处理结果表}对象(即:一条记录
      /// </summary>
      public long Add(InstanceUserResult  instanceUserResult)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new InstanceUserResultRepository(dbContext).Add(instanceUserResult);
         }
      }
      /// <summary>
      /// 添加人员处理结果表{人员处理结果表}对象(即:一条记录
      /// </summary>
      public void Add(IList<InstanceUserResult>  instanceUserResults)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            new InstanceUserResultRepository(dbContext).Add(instanceUserResults);
         }
      }
      /// <summary>
      /// 更新人员处理结果表{人员处理结果表}对象(即:一条记录
      /// </summary>
      public int Update(InstanceUserResult  instanceUserResult)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new InstanceUserResultRepository(dbContext).Update(instanceUserResult);
         }
      }
      /// <summary>
      /// 删除人员处理结果表{人员处理结果表}对象(即:一条记录
      /// </summary>
      public int Delete(string[] idArrays )
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new InstanceUserResultRepository(dbContext).Delete(idArrays);
         }
      }
      /// <summary>
      /// 获取指定的人员处理结果表{人员处理结果表}对象(即:一条记录
      /// </summary>
      public InstanceUserResult GetById(string id)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new InstanceUserResultRepository(dbContext).GetById(id);
         }
      }
      /// <summary>
      /// 获取所有的人员处理结果表{人员处理结果表}对象(即:一条记录
      /// </summary>
      public DataSourceResult Page(DataSourceRequest request)
      {
         using(var dbContext = UnitOfWork.Get(Unity.ContainerName))
         {
            return new InstanceUserResultRepository(dbContext).Query().ToDataSourceResult(request);
         }
      }
   }
}
