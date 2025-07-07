# 모바일 싱글 플레이 게임 웹 서버
모바일 싱글플레이 게임을 위한 RESTful API 서버입니다.  
OAuth 2.0 인증, 세션 관리, Redis 캐시, Auto Scaling 기반 트래픽 분산 구조를 갖춘 실무형 백엔드 서버를 구축했습니다.


## 기술 스택
- Language: C#
- Backend: ASP.NET Core, EF Core
- Database: MySQL (RDS), Redis (ElasticCache)
- Infra/DevOps: AWS (ELB, EC2, Auto Scaling Group), Docker, Github Actions
- Tools: Visual Studio, Git, Postman, JMeter


## 주요 기능
| 기능         | 설명 |
|--------------|------|
| OAuth 2.0 인증 | Google OAuth로 로그인 후 Redis 기반 세션 저장소로 인증 처리 |
| 랭킹 시스템    | 자주 조회되는 데이터를 Redis에 캐싱하여 응답 속도 최적화 |
| 배포 자동화    | GitHub Actions를 통한 무중단 배포 구성 |
| 트래픽 분산    | AWS ELB + ASG 구조로 TPS 400 트래픽 분산 테스트 완료 |
| 계층 구조      | 도메인 기반 폴더 구조로 모듈 분리 및 유지보수 용이 |

## 성능 테스트 결과
| 항목                | 개선 전     | 개선 후     | 향상율       |
| ----------------- | -------- | -------- | --------- |
| 세션 TPS 안정성        | 150\~180 | 210+ | +30%      |
| 랭킹 API 응답속도 (Avg) | 39ms     | 29ms | 25.6% |
| TPS 최대 처리량        | 200      | 400  | +100%     |
| 세션 인증 실패율         | 0.4%     | 0%   | ✅         |



## 문제 해결 사례

###  세션 인증 병목 → Redis 세션 캐시 도입

- **문제**: TPS 증가 시 DB 세션 저장 방식에서 Too many connections 발생
- **해결**: 세션 저장소를 Redis (ElasticCache) 기반 캐시 구조로 전환
- **결과**: TPS 210까지 인증 안정성 확보 + 빠른 응답 속도 유지


###  랭킹 API 병목 → Redis 캐시 적용

- **문제**: 랭킹 조회 트래픽 집중으로 DB 부하 및 응답 지연 발생
- **해결**: 랭킹 데이터를 Redis에 캐싱, 업데이트 주기 조절
- **결과**: 평균 응답 시간 39ms → 29ms (25.6% 개선) + 병목 해소


###  Auto Scaling + RDS 업그레이드

- **문제**: 사용자 증가 시 서버 다운 및 RDS 커넥션 포화
- **해결**: EC2 Auto Scaling + RDS 인스턴스 스펙 업그레이드
- **결과**: TPS 200 → 400 수평 확장 가능, 무중단 트래픽 처리 구조 완성


## 프로젝트를 마치며

실제 서비스 수준의 서버를 직접 구축하며
성능 병목 → 구조 개선 → 분산 설계의 중요성을 실감했습니다.  
단순히 작동하는 API가 아닌 대량 트래픽 상황에서도 견딜 수 있는 서버 아키텍처가 필요하다는 것을 배웠습니다.
