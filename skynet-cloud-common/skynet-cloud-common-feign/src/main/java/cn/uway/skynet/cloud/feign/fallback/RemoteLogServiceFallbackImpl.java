/*
 *  Copyright (c) 2019-2020, magic.s.g.xie (xieshaoguangwww@gmail.com).
 *  <p>
 *  Licensed under the GNU Lesser General Public License 3.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *  <p>
 * https://www.uway.cn
 *  <p>
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package cn.uway.skynet.cloud.feign.fallback;


import cn.uway.skynet.cloud.feign.RemoteLogService;
import cn.uway.skynet.cloud.common.core.util.R;
import cn.uway.skynet.cloud.feign.dto.LogDTO;
import com.alibaba.fastjson.JSONObject;
import lombok.Setter;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

/**
 * @author magic.s.g.xie
 * @date 2019/2/1
 */
@Slf4j
@Component
public class RemoteLogServiceFallbackImpl implements RemoteLogService {
	@Setter
	private Throwable cause;

	/**
	 * 保存日志
	 *
	 * @param sysLog 日志实体
	 * @param from   内部调用标志
	 * @return succes、false
	 */
	@Override
	public R<Boolean> saveLog(LogDTO sysLog, String from) {
		log.error("feign 插入日志失败", cause);
		return null;
	}
}
