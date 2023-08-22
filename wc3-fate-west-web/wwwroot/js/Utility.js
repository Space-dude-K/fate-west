var numberUtil = {
    formatDecimal: function (decimal) {
        return Number(decimal.toFixed(0)).toLocaleString("en");
    },
    formatNumber: function (number) {
        return number.toLocaleString("en");
    }
}