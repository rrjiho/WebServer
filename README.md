# 모바일 싱글 플레이 게임 웹 서버
모바일 싱글플레이 게임을 위한 RESTful API 서버입니다.  
OAuth 2.0 인증, 세션 관리, Redis 캐시, Auto Scaling 기반 트래픽 분산 구조를 갖춘 실무형 백엔드 서버를 구축했습니다.

## 기술 스택
- Language: C#
- Backend: ASP.NET Core, EF Core
- Database: MySQL, Redis
- Infra/DevOps: AWS (ELB, EC2, RDS, ElasticCache), Docker, Github Actions
- Tools: Visual Studio, Git, Postman, JMeter

## 주요 기능
- OAuth 2.0 기반 로그인 + Redis 세션 인증 처리
- 랭킹 시스템 API Redis 캐시 적용 (응답 속도 개선)
- AWS ELB + ASG 구조로 TPS 400 트래픽 분산 테스트 완료
- Github Actions 자동 배포 연동 (무중단 배포)
- 도메인 기반 폴더 구조로 모듈 분리 및 유지보수 용이

## 성능 테스트 결과
| 항목                | 개선 전     | 개선 후     | 향상율       |
| ----------------- | -------- | -------- | --------- |
| 세션 TPS 안정성        | 150\~180 | **210+** | +30%      |
| 랭킹 API 응답속도 (Avg) | 39ms     | **29ms** | **25.6%** |
| TPS 최대 처리량        | 200      | **400**  | +100%     |
| 세션 인증 실패율         | 0.4%     | **0%**   | ✅         |

## 아키텍처
![AWS 도식화 최종](https://github.com/user-attachments/assets/b1ef8359-e24d-475c-8d26-5e62ec997af0)

클라이언트 -> ELB -> EC2 (Auto Scaling) -> RDS, ElasticCache

## 프로젝트를 마치며
단순한 API 기능 구현을 넘어서 실제 서비스 환경에서 발생할 수 있는 병목과 장애 상황을 체험하고  
이를 구조적으로 개선하는 과정에 중점을 두었습니다.  
트래픽 분산, 캐시 도입, 자동화 배포 등 실무와 유사한 환경을 직접 구축하고 실험하면서  
성능 개선과 구조적 사고의 중요성을 깊이 체감할 수 있었습니다.
