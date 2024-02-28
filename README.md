# money-aanalyze

Money forwardから明細をダウンロードできます

## 明細ダウンロード
```
#!/bin/bash
start_date=$(date -d "5 months ago" "+%Y/%m/01")
dotnet PaymentAnalyze.Cli.dll download --mail-address mailaddress --password password --start $start_date --month-count 6 --cache-file-path cache.json --csv-directory
```

## 金融機関連携の更新をリクエストする(更新キューに入るだけなので即時反映ではない)
```
#!/bin/bash
dotnet PaymentAnalyze.Cli.dll update-request --mail-address mailaddress --password password --cache-file-path cache.json
```
![image](https://github.com/aijkl/money-aanalyze/assets/51302983/bc85157b-f27e-491e-903f-7dc473a6c4f1)
  
 ここから派生して飲食店を抽出して行った事がある飲食店を除外して表示できるツールを作りたい
