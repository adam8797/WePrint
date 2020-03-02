// Apparently this is what the json-ified C# TimeSpan is supposed to look like
export class TimeModel {
    constructor() {
        this.days = 0;
        this.hours= 0;
        this.milliseconds= 0;
        this.minutes= 0;
        this.seconds= 0;
        this.ticks= 0;
        this.totalDays= 0;
        this.totalHours= 0;
        this.totalMilliseconds= 0;
        this.totalMinutes= 0;
        this.totalSeconds= 0;
    }

    static AllPropertiesEqual(a, b) {
        return a.days == b.days &&
            a.hours == b.hours &&
            a.minutes == b.minutes &&
            a.seconds == b.seconds &&
            a.ticks == b.ticks &&
            a.milliseconds == b.milliseconds &&
            a.totalDays == b.totalDays &&
            a.totalHours == b.totalHours &&
            a.totalMilliseconds == b.totalMilliseconds &&
            a.totalMinutes == b.totalMinutes &&
            a.totalSeconds == b.totalSeconds;
    }
}