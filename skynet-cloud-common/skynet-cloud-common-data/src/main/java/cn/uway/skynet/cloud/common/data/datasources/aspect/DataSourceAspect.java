package cn.uway.skynet.cloud.common.data.datasources.aspect;

import cn.uway.skynet.cloud.common.data.datasources.annotation.DataSource;

import cn.uway.skynet.cloud.common.data.datasources.config.DataSourceNames;
import cn.uway.skynet.cloud.common.data.datasources.config.DynamicDataSource;
import lombok.extern.slf4j.Slf4j;

import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.annotation.Pointcut;
import org.springframework.core.Ordered;
import org.springframework.core.annotation.Order;
import org.springframework.stereotype.Component;
import org.aspectj.lang.reflect.MethodSignature;

import java.lang.reflect.Method;

/**
 * 多数据源，切面处理类
 * @author magic.s.g.xie
 * @email xiesg@uway.cn
 * @date 2019/1/16 22:20
 */
@Aspect
@Component
@Slf4j
@Order(Ordered.HIGHEST_PRECEDENCE)
public class DataSourceAspect implements Ordered {


    @Pointcut("@annotation(cn.uway.skynet.cloud.common.data.datasources.annotation.DataSource) " +
            "|| @within(cn.uway.skynet.cloud.common.data.datasources.annotation.DataSource)")
    public void dataSourcePointCut() {

    }

    @Around("dataSourcePointCut()")
    public Object around(ProceedingJoinPoint point) throws Throwable {
        MethodSignature signature = (MethodSignature) point.getSignature();
        Method method = signature.getMethod();

        DataSource ds = method.getAnnotation(DataSource.class);
        if(ds == null){
            DynamicDataSource.setDataSource(DataSourceNames.FIRST);
            log.debug("set datasource is " + DataSourceNames.FIRST);
        }else {
            DynamicDataSource.setDataSource(ds.name());
            log.debug("set datasource is " + ds.name());
        }

        try {
            return point.proceed();
        } finally {
            DynamicDataSource.clearDataSource();
            log.debug("clean datasource");
        }
    }

    @Override
    public int getOrder() {
        return 1;
    }
}