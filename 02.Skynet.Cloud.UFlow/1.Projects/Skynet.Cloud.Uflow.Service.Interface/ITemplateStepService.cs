/************************************************************************************
* Copyright (c) 2019-07-11 12:02:28 优网科技 All Rights Reserved.
* CLR版本： .Standard 2.x
* 公司名称：优网科技
* 命名空间：UWay.Skynet.Cloud.Uflow.Service.Interface
* 文件名：  UWay.Skynet.Cloud.Uflow.Service.Interface.TemplateStep.cs
* 版本号：  V1.0.0.0
* 唯一标识：ae69744e-5873-47c9-9a4a-04fd70ac8b1d
* 创建人：  magic.s.g.xie
* 电子邮箱：xiesg@uway.cn
* 创建时间：2019-07-11 12:02:28 
* 描述：流程步骤表 
* 
* 
* =====================================================================
* 修改标记 
* 修改时间：2019-07-11 12:02:28 
* 修改人： 谢韶光
* 版本号： V1.0.0.0
* 描述：
* 
* 
* 
* 
************************************************************************************/



namespace   UWay.Skynet.Cloud.Uflow.Service.Interface
{
   using System;
   using UWay.Skynet.Cloud.Data;

   using UWay.Skynet.Cloud.Request;
   using UWay.Skynet.Cloud.Uflow.Entity;
   using System.Collections.Generic;
   /// <summary>
   /// 流程步骤表服务接口类
   /// </summary>
   public interface ITemplateStepService
   {
      /// <summary>
      /// 添加流程步骤表{流程步骤表}对象(即:一条记录
      /// </summary>
      long Add(TemplateStep  templateStep);
      /// <summary>
      /// 添加流程步骤表{流程步骤表}对象(即:一条记录
      /// </summary>
      void Add(IList<TemplateStep>  templateSteps);
      /// <summary>
      /// 更新流程步骤表{流程步骤表}对象(即:一条记录
      /// </summary>
      int Update(TemplateStep  templateStep);
      /// <summary>
      /// 删除流程步骤表{流程步骤表}对象(即:一条记录
      /// </summary>
      int Delete(string[] idArrays );
      /// <summary>
      /// 获取指定的流程步骤表{流程步骤表}对象(即:一条记录
      /// </summary>
      TemplateStep GetById(string id);
      /// <summary>
      /// 获取所有的流程步骤表{流程步骤表}对象(即:一条记录
      /// </summary>
      DataSourceResult Page(DataSourceRequest request);
   }
}
