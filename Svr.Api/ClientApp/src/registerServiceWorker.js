// In production, we register a service worker to serve assets from local cache.
//В производстве мы регистрируем сервисного работника для обслуживания активов из локального кэша.

// This lets the app load faster on subsequent visits in production, and gives
// it offline capabilities. However, it also means that developers (and users)
// will only see deployed updates on the "N+1" visit to a page, since previously
// cached resources are updated in the background.
//Это позволяет приложению быстрее загружаться при последующих посещениях в производстве и дает ему автономные возможности.Однако это также означает, что разработчики(и пользователи) будут видеть развернутые обновления только при посещении страницы "N+1", поскольку ранее кэшированные ресурсы обновляются в фоновом режиме.

// To learn more about the benefits of this model, read https://goo.gl/KwvDNy.
// This link also includes instructions on opting out of this behavior.
//Чтобы узнать больше о преимуществах этой модели, прочитайте https://goo.gl/KwvDNy- Эта ссылка также включает инструкции по отказу от этого поведения.

const isLocalhost = Boolean(
  window.location.hostname === 'localhost' ||
  // [::1] is the IPv6 localhost address.
  window.location.hostname === '[::1]' ||
  // 127.0.0.1/8 is considered localhost for IPv4.
  window.location.hostname.match(
    /^127(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$/
  )
);

export default function register()
{
  if (process.env.NODE_ENV === 'production' && 'serviceWorker' in navigator)
  {
    // The URL constructor is available in all browsers that support SW.
    //Конструктор URL - адрес доступен во всех браузерах, которые поддерживают ГВ.
    const publicUrl = new URL(process.env.PUBLIC_URL, window.location);
    if (publicUrl.origin !== window.location.origin)
    {
      // Our service worker won't work if PUBLIC_URL is on a different origin
      // from what our page is served on. This might happen if a CDN is used to
      // serve assets; see https://github.com/facebookincubator/create-react-app/issues/2374
      //Наш сервисный работник не будет работать, если PUBLIC_URL имеет другое происхождение, чем наша страница.Это может произойти, если CDN используется для обслуживания активов; см.https://github.com/facebookincubator/create-react-app/issues/2374
      return;
    }

    window.addEventListener('load', () =>
    {
      const swUrl = `${process.env.PUBLIC_URL}/service-worker.js`;

      if (isLocalhost)
      {
        // This is running on localhost. Lets check if a service worker still exists or not.
        //Это работает на localhost.Позволяет проверить, существует ли еще service worker или нет.
        checkValidServiceWorker(swUrl);
      } else
      {
        // Is not local host. Just register service worker
        //Не является локальным хостом.Просто зарегистрируйтесь работник сервиса
        registerValidSw(swUrl);
      }
    });
  }
}

function registerValidSw(swUrl)
{
  navigator.serviceWorker
    .register(swUrl)
    .then(registration =>
    {
      registration.onupdatefound = () =>
      {
        const installingWorker = registration.installing;
        installingWorker.onstatechange = () =>
        {
          if (installingWorker.state === 'installed')
          {
            if (navigator.serviceWorker.controller)
            {
              // At this point, the old content will have been purged and
              // the fresh content will have been added to the cache.
              // It's the perfect time to display a "New content is
              // available; please refresh." message in your web app.
              // На этом этапе старое содержимое будет очищено, а новое содержимое будет добавлено в кэш. Это идеальное время для отображения " новый контент доступен; пожалуйста, обновите."сообщение в вашем веб-приложении.
              console.log('New content is available; please refresh.');
            } else
            {
              // At this point, everything has been precached.
              // It's the perfect time to display a
              // "Content is cached for offline use." message.
              //В этот момент, все было проповедано. Это идеальное время для отображения " содержимое кэшируется для использования в автономном режиме." сообщение.
              console.log('Content is cached for offline use.');
            }
          }
        };
      };
    })
    .catch(error =>
    {
      console.error('Error during service worker registration:', error);
    });
}

function checkValidServiceWorker(swUrl)
{
  // Check if the service worker can be found. If it can't reload the page.
  //Проверьте, можно ли найти работника службы. Если он не может перезагрузить страницу.
  fetch(swUrl)
    .then(response =>
    {
      // Ensure service worker exists, and that we really are getting a JS file.
      //Убедитесь, что service worker существует и что мы действительно получаем JS - файл.
      if (
        response.status === 404 ||
        response.headers.get('content-type').indexOf('javascript') === -1
      )
      {
        // No service worker found. Probably a different app. Reload the page.
        //Работник службы не найден.Возможно, другое приложение.Перезагрузить страницу.
        navigator.serviceWorker.ready.then(registration =>
        {
          registration.UnRegister().then(() =>
          {
            window.location.reload();
          });
        });
      } else
      {
        // Service worker found. Proceed as normal.
        //Работник службы найден.Действуйте как обычно.
        registerValidSw(swUrl);
      }
    })
    .catch(() =>
    {
      console.log(
        //'No internet connection found. App is running in offline mode.'
        'Подключение к интернету не найдено. Приложение работает в автономном режиме.'
      );
    });
}

export function UnRegister()
{
  if ('serviceWorker' in navigator)
  {
    navigator.serviceWorker.ready.then(registration =>
    {
      registration.UnRegister();
    });
  }
}