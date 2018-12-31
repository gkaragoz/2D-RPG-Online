const container = require('markdown-it-container')

const ogprefix = 'og: http://ogp.me/ns#'
const title = 'ShiftServer Documentation'
const description = 'ShiftServer Api Documentation'
const color = '#2F80ED'
const author = 'ManaShift Studios'
const url = 'http://192.168.1.6:8080'

module.exports = {
  head: [
    ['link', { rel: 'icon', href: `/rocket.png` }],
    ['meta', { name: 'theme-color', content: color }],
    ['meta', { prefix: ogprefix, property: 'og:title', content: title }],
    ['meta', { prefix: ogprefix, property: 'twitter:title', content: title }],
    ['meta', { prefix: ogprefix, property: 'og:type', content: 'article' }],
    ['meta', { prefix: ogprefix, property: 'og:url', content: url }],
    ['meta', { prefix: ogprefix, property: 'og:description', content: description }],
    ['meta', { prefix: ogprefix, property: 'og:image', content: `${url}rocket.png` }],
    ['meta', { prefix: ogprefix, property: 'og:article:author', content: author }],
    ['meta', { name: 'apple-mobile-web-app-capable', content: 'yes' }],
    ['meta', { name: 'apple-mobile-web-app-status-bar-style', content: 'black' }],
    // ['link', { rel: 'apple-touch-icon', href: `/assets/apple-touch-icon.png` }],
    // ['link', { rel: 'mask-icon', href: '/assets/safari-pinned-tab.svg', color: color }],
    ['meta', { name: 'msapplication-TileImage', content: '/rocket.png' }],
    ['meta', { name: 'msapplication-TileColor', content: color }],
  ],
  markdown: {
    anchor: {
      permalink: true,
    },
    config: md => {
      md
        .use(require('markdown-it-decorate'))
        .use(...createContainer('intro'))
        .use(...createContainer('note'))
    }
  },
  title,
  description,
  base: '/documentation/',
  ga: '',
  themeConfig: {
    versions: [
      ['Version 1.x.x', '/1.x.x/'],
    ],
    repo: 'gkaragoz/2D-RPG-Online',
    website: 'https://manashift.io',
    docsDir: 'docs',
    editLinks: true,
    editLinkText: 'Improve this page',
    serviceWorker: true,
    hiddenLinks: [

    ],
    sidebar: {
      '/1.x.x/': [
        // {
          // collapsable: false,
          // title: '🚀 Getting started',
          // children: [
            // '/3.x.x/getting-started/installation',
            // '/3.x.x/getting-started/quick-start',
            // '/3.x.x/concepts/concepts',
          // ],
        // },
        {
          collapsable: true,
          title: '🔌 Event Api Usage',
          children: [
            '/1.x.x/protobuf-api/server-events',
            '/1.x.x/protobuf-api/player-events'
          ],
        },
      ],
    },
  },
};

function createContainer(className) {
  return [container, className, {
    render(tokens, idx) {
      const token = tokens[idx]
      if (token.nesting === 1) {
        return `<div class="${className} custom-block">\n`
      } else {
        return `</div>\n`
      }
    }
  }]
}
