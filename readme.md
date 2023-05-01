# Example EntityAutoMapper

This example allows to create dynamic entities including mapping for the Entity Framework Core while the entities are only interfaces.

The (not perfect) benchmark shows a small overhead due to additional methods and much more casts.


## Benchmark result

| Name                           |       Iteration |          Global |           Count |
|--------------------------------|-----------------|-----------------|-----------------|
| TestEfCore                     |       17,431 ms |    87155,744 ms |           5.000 |
| TestSam                        |       18,082 ms |    90411,889 ms |           5.000 |


