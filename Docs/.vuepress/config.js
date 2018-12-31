const container = require('markdown-it-container')

const ogprefix = 'og: http://ogp.me/ns#'
const title = 'ShiftServer Documentation'
const description = 'ShiftServer Protobuf Api Documentation'
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
    repo: 'strapi/strapi',
    website: 'https://strapi.io',
    docsDir: 'docs',
    editLinks: true,
    editLinkText: 'Improve this page',
    serviceWorker: true,
    hiddenLinks: [
      '/1.x.x/cli/CLI.html',
      '/1.x.x/api-reference/reference.html',
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
          title: '💡 Guides',
          children: [
            '/1.x.x/guides/api-documentation',
            '/1.x.x/guides/authentication',
            '/1.x.x/configurations/configurations',
            '/1.x.x/guides/controllers',
            '/1.x.x/guides/deployment',
            '/1.x.x/guides/email',
            '/1.x.x/guides/upload',
            '/1.x.x/guides/filters',
            '/1.x.x/guides/graphql',
            '/1.x.x/guides/i18n',
            '/1.x.x/guides/models',
            '/1.x.x/guides/policies',
            '/1.x.x/guides/public-assets',
            '/1.x.x/guides/requests',
            '/1.x.x/guides/responses',
            '/1.x.x/guides/routing',
            '/1.x.x/guides/services',
            '/1.x.x/guides/webhooks',
          ],
        },
        // {
          // collapsable: true,
          // title: '⚙️️ Advanced',
          // children: [
            // '/1.x.x/advanced/customize-admin',
            // '/1.x.x/advanced/hooks',
            // '/1.x.x/advanced/logging',
            // '/1.x.x/advanced/middlewares',
            // '/1.x.x/advanced/usage-tracking',
          // ],
        // },
        // {
          // collapsable: true,
          // title: '🔌 Plugin Development',
          // children: [
            // '/1.x.x/plugin-development/quick-start',
            // '/1.x.x/plugin-development/plugin-architecture',
            // '/1.x.x/plugin-development/backend-development',
            // '/1.x.x/plugin-development/frontend-development',
            // '/1.x.x/plugin-development/frontend-use-cases',
            // '/1.x.x/plugin-development/utils',
            // // '/3.x.x/plugin-development/ui-components', TODO: Add this file
          // ],
        // },
        // {
          // collapsable: true,
          // title: '💻 Command Line Interface',
          // children: [
            // '/3.x.x/cli/CLI',
          // ],
        // },
        {
          collapsable: true,
          title: '🏗 API Reference',
          children: [
            '/1.x.x/api-reference/reference',
          ],
        },
        // {
          // collapsable: false,
          // title: '📚 Resources',
          // children: [
            // ['https://github.com/strapi/strapi/blob/master/CONTRIBUTING.md', 'Contributing guide'],
            // '/3.x.x/migration-guide/',
            // '/3.x.x/tutorials/',
          // ],
        // },
      ],
    },
  },
}

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
