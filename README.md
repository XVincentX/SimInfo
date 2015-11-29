# SimInfo


## What's up?
SimInfo was a free Windows Phone 8 application that let you manage your carrier offer in a simply and easy way.

It was working with following network operators:
- Wind
- Vodafone
- Tim
- H3G
- Coop Voce
- Tiscali Mobile
- Noverca
- Fastweb Mobile

It was able to monitor costantly your carrier data and, using push notification, notice you when you were under limits
using a Toast message. It was localised in both Italian and English, and Spanish was planned.

![Nasino](https://scontent.xx.fbcdn.net/hphotos-xfl1/v/t1.0-9/1604661_232581790198879_2085626837_n.png?oh=29263c00b3b63fde8cb8df2fd529acfe&oe=56F932DD)

## But I can't find it on the store!

After more or less 2 years of working, Italian carriers started to block my requestes (I was using AppHarbor for hosting).
As a workaround, I tried to route all the traffic using TOR but, given that public final IPs are [public](https://www.dan.me.uk/tornodes)
it was easy to block me again after 2-3 weeks.

As final attemp, I moved all the logic back to the client itself (push notification feature was closed) but then suddenly
(I am really wondering how, after two years), Microsoft noticed me that the **approved** application was rejected, and
thous, removed.

Given that I can't work on it anymore (for now), I decided to release the sources.
I would love to rework on this again, perhaps in a cross platform way (react-native or ionic), but I can't make any plan now.


- Some build notes are [on my blog](http://vncz.js.org/building-siminfo/), but nothing really useful to be honest.
- Basically the thing is organised in two directories: the client resides in **WindInfo**, the server is in **WindAuth**
- Carrier specific code are located in [Code](https://github.com/XVincentX/SimInfo/tree/master/WindAuth/Code) directory.
  Those may actually be totally outdated.
  
## Disclaimer
- We're not the code we write. This is quite old stuff and the overall thing **is** a bit shitty.
  but still, it was a valuable product for really a lot of people.
- It started as an experiment to solve my father's problem, and then evolved to be a killing application.
