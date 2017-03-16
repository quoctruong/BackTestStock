function createTooltipHtml(low, open, close, high) {
    return '<div style="display: inline-block; width: 70px">' +
        '<span>Low: ' + low + '</span><br/>' +
        '<span>Open: ' + open + '</span><br/>' +
        '<span>Close: ' + close + '</span><br/>' +
        '<span>High: ' + high + '</span></div>';
}

var ChartService = function ($http) {
    this.drawChart = function (ticker) {
        console.log('Here');
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Date');
        data.addColumn('number', 'Low');
        data.addColumn('number', 'Open');
        data.addColumn('number', 'Close');
        data.addColumn('number', 'High');
        data.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
        var options = {
            seriesType: 'candlesticks',
            legend: 'none',
            candlestick: {
                hollowIsRising: true,
                fallingColor: {
                    strokeWidth: 2,
                    fill: '#A52714'
                },
                risingColor: {
                    strokeWidth: 2,
                    fill: '#0F9D58'
                }
            },
            hAxis: {
                gridlines: {
                    count: -1
                }
            },
            vAxis: {
                gridlines: {
                    count: -1,
                }
            },
            tooltip: { isHtml: true }
        };
        console.log(document.getElementById('base_chart_div'));
        var chart = new google.visualization.CandlestickChart(document.getElementById('base_chart_div'));
        var link = 'stock/' + ticker;
        $http.get(link).then(function (response) {
            var monthNames = ["January", "February", "March", "April", "May", "June",
              "July", "August", "September", "October", "November", "December"
            ];
            var json = response.data;
            var firstDate = new Date(json[0]["Date"]);
            console.log(firstDate);
            var currentMonth = firstDate.getMonth();
            var currentYear = firstDate.getYear();
            for (var i = 0; i < json.length; i++) {
                var tick = json[i];
                var fullDate = new Date(tick["Date"]);
                var date = '';
                if (i != 0) {
                    if (fullDate.getYear() != currentYear) {
                        currentYear = fullDate.getYear();
                        date = currentYear;
                    } else if (fullDate.getMonth() != currentMonth) {
                        currentMonth = fullDate.getMonth();
                        date = monthNames[currentMonth];
                    } else {
                        date = fullDate.getDate();
                    }
                }
                else {
                    date = fullDate.getDate();
                }
                date = date.toString();
                var open = tick["Open"];
                var high = tick["High"];
                var low = tick["Low"];
                var close = tick["Close"];
                var volume = tick["Volume"];
                data.addRow([date, low, open, close, high, createTooltipHtml(low, open, close, high)]);
            }
            chart.draw(data, options);
        }, function (response) {
            console.log(response);
        });
        var link = 'stock/' + ticker + '/SMA/200/daily';
        $http.get(link).then(function (response) {
            console.log(response);
            var json = response.data;
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Date');
            data.addColumn('number', 'SMA');

            for (var i = 0; i < json.length; i++) {
                var tick = json[i];
                var date = tick["Date"];
                var sma = tick["SMA"];
                data.addRow([date, sma]);
            }
            var smaChart = new google.visualization.LineChart(document.getElementById('overlay_chart_div'));
            console.log('drawing');
            smaChart.draw(data, {});
        }, function (response) {
        });
    }
}

ChartService.$inject = ['$http'];
