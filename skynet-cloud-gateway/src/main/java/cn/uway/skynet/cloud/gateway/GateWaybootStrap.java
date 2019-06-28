package cn.uway.skynet.cloud.gateway;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.client.discovery.EnableDiscoveryClient;
import org.springframework.context.annotation.ComponentScan;
@ComponentScan(basePackages = {"cn.uway.skynet.cloud"})
@SpringBootApplication
@EnableDiscoveryClient
public class GateWaybootStrap {
    public static void main(String[] args) {
        SpringApplication.run(GateWaybootStrap.class, args);
    }
}
